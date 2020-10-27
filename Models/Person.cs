using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_4Point1.Models
{
    [Table("person")]
    public partial class Person
    {
        // This initializes an empty list so we don't get null reference exceptions for our list.
        public Person()
        {
            PhoneNumbers = new HashSet<PhoneNumber>();
        }

        [Key]
        [Column("ID", TypeName = "int(10)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        [Column(TypeName = "varchar(50)")]
        public string FirstName { get; set; }
        [Required]
        [Column(TypeName = "varchar(50)")]
        public string LastName { get; set; }

        // InverseProperty links the two virtual properties together.
        [InverseProperty(nameof(Models.PhoneNumber.Person))]
        public virtual ICollection<PhoneNumber> PhoneNumbers { get; set; }
    }
}
