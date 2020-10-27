using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_4Point1.Models;
using MVC_4Point1.Models.Exceptions;
using DotNet_React.Models.DTO;

namespace MVC_4Point1.Controllers
{


    public partial class PersonControllerBLL : Controller
    {
        // Do NOT do this in practices, have a separate create for phone numbers.
        // The only reason I did this is we scaffolded the phone number table and I didn't want to cause conflicts.

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        // id is not necessary as it is now AUTO_INCREMENT in the database, and is generated thereby.
        public IActionResult Create(string firstName, string lastName, string phone)
        {
            // When this Action gets called, there are 3 likely states:
            // 1) First page load, no data has been provided (initial state).
            // 2) Partial data has been provided (error state).
            // 3) Complete data has been provided (submit state).

            // A request has come in that has some data stored in the query (GET or POST).

            if (Request.Query.Count > 0)
            {
                try
                {
                    CreatePerson(firstName, lastName, phone);

                    ViewBag.Success = "Successfully added the person to the list.";
                }
                catch (PersonValidationException e)
                {
                    // All expected data not provided, so this will be our error state.
                    ViewBag.Exception = e;

                    // Store our data to re-add to the form.
                    ViewBag.FirstName = firstName != null ? firstName.Trim() : null;
                    ViewBag.LastName = lastName != null ? lastName.Trim() : null;
                    ViewBag.Phone = phone != null ? phone.Trim() : null;
                }
            }

            return View();
        }
        public IActionResult List(string filter)
        {

            // Slightly different from practice as you'll be calling methods and not using a "using" in your action.
            if (filter == "on")
            {
                ViewBag.People = GetPeopleWithMultiplePhoneNumbers();
                ViewBag.Filter = filter;
            }
            else
            {
                ViewBag.People = GetPeople();
            }



            return View();
        }
        public IActionResult Details(string id, string delete)
        {
            IActionResult result;
            if (delete != null)
            {
                DeletePersonByID(int.Parse(id));
                result = RedirectToAction("List");
            }
            else
            {
                ViewBag.Person = GetPersonByID(int.Parse(id));
                result = View();
            }

            return result;
        }

        public int CreatePerson(string firstName, string lastName, string phone)
        {
            firstName = firstName != null ? firstName.Trim() : null;
            lastName = lastName != null ? lastName.Trim() : null;
            phone = phone != null ? phone.Trim() : null;
            int createdID;

            PersonValidationException exception = new PersonValidationException();
            // Be a little more specific than "== null" because that doesn't account for whitespace.
            if (string.IsNullOrWhiteSpace(firstName))
            {
                exception.SubExceptions.Add(new ArgumentNullException(nameof(firstName), "First name was not provided."));
            }
            else
            {
                if (firstName.Any(x => char.IsDigit(x)))
                {
                    exception.SubExceptions.Add(new ArgumentException(nameof(firstName), "First name cannot contain numbers."));
                }
                if (firstName.Length > 50)
                {
                    exception.SubExceptions.Add(new ArgumentOutOfRangeException(nameof(firstName), "First name cannot be more than 50 characters long."));
                }
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                exception.SubExceptions.Add(new ArgumentNullException(nameof(lastName), "Last name was not provided."));
            }
            else
            {
                if (lastName.Any(x => char.IsDigit(x)))
                {
                    exception.SubExceptions.Add(new ArgumentException(nameof(lastName), "Last name cannot contain numbers."));
                }
                if (lastName.Length > 50)
                {
                    exception.SubExceptions.Add(new ArgumentOutOfRangeException(nameof(lastName), "Last name cannot be more than 50 characters long."));
                }
            }

            if (string.IsNullOrWhiteSpace(phone))
            {
                exception.SubExceptions.Add(new ArgumentNullException(nameof(phone), "Phone number was not provided."));
            }
            else
            {
                // Check for phone number formatting (feel free to use RegEx or any other method).
                // Has to be in the else branch to avoid null reference exceptions.
                int temp;
                string[] phoneParts = phone.Split('-');
                if (!(
                    phoneParts[0].Length == 3 &&
                    int.TryParse(phoneParts[0], out temp) &&
                    phoneParts[1].Length == 3 &&
                    int.TryParse(phoneParts[1], out temp) &&
                    phoneParts[2].Length == 4 &&
                    int.TryParse(phoneParts[2], out temp)
                    ))
                {
                    exception.SubExceptions.Add(new ArgumentException(nameof(phone), "Phone number was not in a valid format."));
                }
            }

            // If any exceptions have been generated by any validation, throw them as one bundled exception.
            if (exception.SubExceptions.Count > 0)
            {
                throw exception;
            }

            // If we're at this point, we have no exceptions, as nothing got thrown.
            // At this point, ID is 0.
            Person newPerson = new Person()
            {
                FirstName = firstName,
                LastName = lastName
            };
            PhoneNumber newPhoneNumber = new PhoneNumber()
            {
                Number = phone,
                Person = newPerson
            };
            // Add the new model instances to the database.
            using (PersonContext context = new PersonContext())
            {
                // By adding our object to the context, we're queueing it to receive an AUTO_INCREMENT ID once saved.
                context.People.Add(newPerson);
                context.PhoneNumbers.Add(newPhoneNumber);
                // When we save it, the object and all references thereto are updated with the new ID.
                context.SaveChanges();
                // Which makes it very simple to then get the new ID to return.
                createdID = newPerson.ID;
            }
            return createdID;
        }

        // A PUT request, semantically, overwrites an entire entity and does not update a specific field.
        public void UpdatePerson(string id, string firstName, string lastName)
        {
            id = id != null ? id.Trim() : null;
            firstName = firstName != null ? firstName.Trim() : null;
            lastName = lastName != null ? lastName.Trim() : null;
            int idParsed = 0;

            using (PersonContext context = new PersonContext())
            {
                PersonValidationException exception = new PersonValidationException();

                if (string.IsNullOrWhiteSpace(id))
                {
                    exception.SubExceptions.Add(new ArgumentNullException(nameof(id), "ID was not provided."));
                }
                else
                {
                    if (!int.TryParse(id, out idParsed))
                    {
                        exception.SubExceptions.Add(new ArgumentException(nameof(id), "ID was not valid."));
                    }
                    else
                    {
                        if (context.People.Where(x => x.ID == idParsed).Count() != 1)
                        {
                            exception.SubExceptions.Add(new NullReferenceException("Person with that ID does not exist."));
                        }
                    }
                }
                if (string.IsNullOrWhiteSpace(firstName))
                {
                    exception.SubExceptions.Add(new ArgumentNullException(nameof(firstName), "First name was not provided."));
                }
                else
                {
                    if (firstName.Any(x => char.IsDigit(x)))
                    {
                        exception.SubExceptions.Add(new ArgumentException(nameof(firstName), "First name cannot contain numbers."));
                    }
                    if (firstName.Length > 50)
                    {
                        exception.SubExceptions.Add(new ArgumentOutOfRangeException(nameof(firstName), "First name cannot be more than 50 characters long."));
                    }
                }

                if (string.IsNullOrWhiteSpace(lastName))
                {
                    exception.SubExceptions.Add(new ArgumentNullException(nameof(lastName), "Last name was not provided."));
                }
                else
                {
                    if (lastName.Any(x => char.IsDigit(x)))
                    {
                        exception.SubExceptions.Add(new ArgumentException(nameof(lastName), "Last name cannot contain numbers."));
                    }
                    if (lastName.Length > 50)
                    {
                        exception.SubExceptions.Add(new ArgumentOutOfRangeException(nameof(lastName), "Last name cannot be more than 50 characters long."));
                    }
                }

                // If any exceptions have been generated by any validation, throw them as one bundled exception.
                if (exception.SubExceptions.Count > 0)
                {
                    throw exception;
                }

                // If we're at this point, we have no exceptions, as nothing got thrown.
                Person target = context.People.Where(x => x.ID == idParsed).Single();
                target.FirstName = firstName;
                target.LastName = lastName;
                context.SaveChanges();
            }
        }

        public List<PersonWithPhone> GetPeople()
        {
            using (PersonContext context = new PersonContext())
            {
                // Just returning People.Include(x => x.PhoneNumbers) results in circular references when we try to convert to JSON.
                // Person has a phone number, which has a person, which has a phone number, which has a person, forever.
                return context.People.Include(x => x.PhoneNumbers).Select(x => new PersonWithPhone
                {
                    ID = x.ID,
                    FullName = x.FirstName + " " + x.LastName,
                    PhoneNumbers = x.PhoneNumbers.Select(y => y.Number).ToList()
                }).ToList();
            }
        }
        public List<Person> GetPeopleStartingWith(string startChar)
        {
            using (PersonContext context = new PersonContext())
            {
                return context.People.Where(x => x.FirstName.StartsWith(startChar)).ToList();
            }

        }
        public List<Person> GetPeopleWithMultiplePhoneNumbers()
        {
            using (PersonContext context = new PersonContext())
            {
                return context.People.Where(x => x.PhoneNumbers.Count > 1).ToList();
            }
        }
        public Person GetPersonByID(int id)
        {
            Person target;
            // We have to make a separate query for phone numbers, unless we use something called a DTO (Data Transfer Object) to bind them to.
            // Due to time constraints that may or may not be covered, so this is a workaround.
            List<PhoneNumber> phoneNumbers;
            using (PersonContext context = new PersonContext())
            {
                target = context.People.Where(x => x.ID == id).Single();
                phoneNumbers = context.PhoneNumbers.Where(x => x.PersonID == target.ID).ToList();
            }
            // When we initially query to get the "target", EF will only enumerate (retreive) the records from THAT table (by default).
            // This is a workaround to make sure that the phone numbers are assigned to the object that gets returned.
            // There are better and more efficient ways to do this, but this will serve our purposes and is easier to understand at face value.
            target.PhoneNumbers = phoneNumbers;
            return target;
        }
        public void ChangeFirstNameByID(int id, string newName)
        {
            // Make sure that if their name is already the name provided, throw an exception.
            // If they don't exist, throw an exception.

            using (PersonContext context = new PersonContext())
            {
                Person target = context.People.Where(x => x.ID == id).Single();
                if (target.FirstName == newName)
                {
                    throw new Exception("The target's first name is the same as the requested new name.");
                }
                target.FirstName = newName;
                context.SaveChanges();
            }
        }
        public void DeletePersonByID(int id)
        {
            using (PersonContext context = new PersonContext())
            {
                context.People.Remove(context.People.Where(x => x.ID == id).Single());
                context.SaveChanges();
            }


        }
    }
}