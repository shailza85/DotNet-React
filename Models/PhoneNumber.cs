using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_4Point1.Models
{
    [Table("phonenumber")]
    public partial class PhoneNumber
    {
        [Key]
        [Column("ID", TypeName = "int(10)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        [Column(TypeName = "char(12)")]
        public string Number { get; set; }
        [Column("PersonID", TypeName = "int(10)")]
        public int PersonID { get; set; }

        // This attribute specifies which database field is the foreign key. Typically in the child (many side of the 1-many).
        [ForeignKey(nameof(PersonID))]

        // InverseProperty links the two virtual properties together.
        [InverseProperty(nameof(Models.Person.PhoneNumbers))]
        public virtual Person Person { get; set; }
    }
}