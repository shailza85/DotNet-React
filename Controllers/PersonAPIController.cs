using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MVC_4Point1.Models;
using MVC_4Point1.Models.Exceptions;
using DotNet_React.Models.DTO;

namespace MVC_4Point1.Controllers
{
    // GET: Read / Query - Get some data.
    // POST: Create - Add some data.
    // PUT: Update (Overwrite) - Replace some data.
    // PATCH: Update (Modify) - Modify some data.
    // DELETE: Delete - Remove some data.


    // Common Status Codes:
    // 200: Ok - Everything's good.
    // 400: Bad Request - Invalid data types / syntax / etc.
    // 404: Not Found - No item with that ID, etc exists.
    // 409: Conflict - Breaks a business logic rule, etc.

    // Given:
    // [Route("API/[controller]") and [HttpGet("People/Test")]
    // Our path should be: https://localhost:PORT/API/PersonAPI/People/Test

    // This defines the controller as an API controller.
    [Route("Person")]
    [ApiController]
    // Our class name (sans 'Controller') is substituted into [controller] in the Route annotation.
    public partial class PersonAPIController : ControllerBase
    {
        // This determines the second segment of the path.
        [HttpGet("API/All")]

        // This is the return type of the request. The method name is irrelevant as far as the clients are concerned.
        public ActionResult<IEnumerable<PersonWithPhone>> AllPeople_GET()
        {
            // This is what we are returning. It gets serialized as JSON if we return an object.
            return new PersonControllerBLL().GetPeople();
        }

        // This determines the second segment of the path.
        [HttpGet("API/MultiplePhones")]

        // This is the return type of the request. The method name is irrelevant as far as the clients are concerned.
        public ActionResult<IEnumerable<Person>> MultiplePhonesPeople_GET()
        {
            return new PersonControllerBLL().GetPeopleWithMultiplePhoneNumbers();
        }

        // This determines the second segment of the path.
        [HttpGet("API/StartsWith/{startChar:alpha}")]

        // This is the return type of the request. The method name is irrelevant as far as the clients are concerned.
        public ActionResult<IEnumerable<Person>> NameStartsWithPeople_GET_URL(string startChar)
        {
            // Assuming we aren't using the controller again, we might as well just instantiate it where we need it the one time.
            return new PersonControllerBLL().GetPeopleStartingWith(startChar);
        }

        // This determines the second segment of the path.
        [HttpGet("API/StartsWith")]

        // This is the return type of the request. The method name is irrelevant as far as the clients are concerned.
        public ActionResult<IEnumerable<Person>> NameStartsWithPeople_GET_QueryString(string startChar)
        {
            // Assuming we aren't using the controller again, we might as well just instantiate it where we need it the one time.
            return new PersonControllerBLL().GetPeopleStartingWith(startChar);
        }


        // This method of GET parameters will retrieve the argument from the URL.
        [HttpGet("API/ID/{id}")]
        // This is the return type of the request. The method name is irrelevant as far as the clients are concerned.
        public ActionResult<object> PersonID_GET_URL(int id)
        {
            ActionResult<object> response;
            // This is what we are returning. It gets serialized as JSON if we return an object.
            try
            {
                Person person = new PersonControllerBLL().GetPersonByID(id);

                // This is "kind of" a DTO. We're putting the fields we care about into another object that is not the database model.
                // They help get around errors like the circular references, and (if you use them in the context) the missing virtual properties.
                response = new
                {
                    id = person.ID,
                    firstName = person.FirstName,
                    lastName = person.LastName,
                    phoneNumbers = person.PhoneNumbers.Select(x => x.Number)
                };
            }
            catch
            {
                response = NotFound(new { error = $"No person at ID {id} could be found." });
            }

            return response;
        }

        // This method of GET parameters will retrieve the argument from the query string (after the ? in the URL).
        [HttpGet("API/ID")]
        public ActionResult<object> PersonID_GET_QueryString(int id)
        {
            ActionResult<object> response;
            // This is what we are returning. It gets serialized as JSON if we return an object.
            try
            {
                Person person = new PersonControllerBLL().GetPersonByID(id);

                // This is "kind of" a DTO. We're putting the fields we care about into another object that is not the database model.
                // They help get around errors like the circular references, and (if you use them in the context) the missing virtual properties.
                response = new
                {
                    id = person.ID,
                    firstName = person.FirstName,
                    lastName = person.LastName,
                    phoneNumbers = person.PhoneNumbers.Select(x => x.Number)
                };
            }
            catch
            {
                response = NotFound(new { error = $"No person at ID {id} could be found." });
            }

            return response;
        }

        // Patches can either be in this format below, where an endpoint does one action, or in a format that specifies the "instructions" in the query.
        /*
         For example:
         id: 9,
         action: update,
         variable: FirstName,
         value: John
        or
          id: 20,
          action: add,
          variable: Count,
          value: 5
        */
        [HttpPatch("API/FirstName")]
        public ActionResult ChangePersonFirstName_PATCH(int id, string newName)
        {
            ActionResult response;
            try
            {
                new PersonControllerBLL().ChangeFirstNameByID(id, newName);
                response = Ok(new { message = $"Successfully renamed person {id} to {newName}." });
            }
            catch (InvalidOperationException)
            {
                response = NotFound(new { error = $"The requested person at ID {id} does not exist." });
            }
            catch (Exception e)
            {
                response = Conflict(new { error = e.Message });
            }


            return response;
        }

        [HttpPost("API/Create")]
        public ActionResult CreatePerson_POST(string firstName, string lastName, string phone)
        {
            ActionResult response;
            try
            {
                int newID = new PersonControllerBLL().CreatePerson(firstName, lastName, phone);

                // Just for fun:
                // (It's also an example of how to throw a code that doesn't have a method built-in)
                if (firstName.Trim().ToUpper() == "TEAPOT" && lastName.Trim().ToUpper() == "COFFEE")
                {
                    response = StatusCode(418, new { message = $"Successfully created teapot but it does not want to brew coffee. It has the phone number {phone}." });
                }
                else
                {
                    // This should really be a Create() that provides the API endpoint for the GET to retrieve the created object.
                    response = Created($"API/PersonAPI/People/ID/{newID}", new { message = $"Successfully created person {firstName} {lastName} with the phone number {phone} at ID {newID}." });
                }
            }
            catch (PersonValidationException e)
            {
                response = UnprocessableEntity(new { errors = e.SubExceptions.Select(x => x.Message) });
            }


            return response;
        }

        [HttpPut("API/Update")]
        public ActionResult UpdatePerson_PUT(string id, string firstName, string lastName)
        {
            ActionResult response;
            try
            {
                new PersonControllerBLL().UpdatePerson(id, firstName, lastName);

                // Semantically, we should be including a copy of the object (or at least a DTO rendering of it) in the Ok response.
                // For our purposes, a message with the fields will suffice.
                response = Ok(new { message = $"Successfully updated person at ID {id} to be {firstName} {lastName}." });
            }
            catch (PersonValidationException e)
            {
                // If it couldn't find the entity to update, that's the primary concern, so discard the other subexceptions and just return NotFound().
                if (e.SubExceptions.Any(x => x.GetType() == typeof(NullReferenceException)))
                {
                    response = NotFound(new { error = $"No entity exists at ID {id}." });
                }
                // If there's no NullReferenceException, but there's still an exception, return the list of problems.
                else
                {
                    response = UnprocessableEntity(new { errors = e.SubExceptions.Select(x => x.Message) });
                }
            }


            return response;
        }

        [HttpDelete("API/Delete")]
        public ActionResult DeletePerson_DELETE(string id)
        {
            ActionResult response;

            // This logic should probably be in the DeletePersonByID() method, but if I change the parameter type to string now, I'll have to fix compiler errors in the Views.
            int idParsed;
            if (string.IsNullOrWhiteSpace(id))
            {
                response = Conflict(new { error = "ID was not provided." });
            }
            else
            {
                if (!int.TryParse(id, out idParsed))
                {
                    response = Conflict(new { error = "The provided ID is invalid." });
                }
                else
                {
                    try
                    {
                        new PersonControllerBLL().DeletePersonByID(idParsed);
                        response = Ok(new { message = $"Successfully deleted the person with ID {idParsed}." });
                    }
                    catch
                    {
                        response = NotFound(new { error = $"No person at ID {idParsed} could be found." });
                    }
                }
            }
            return response;
        }
    }
}