using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class Person
    {
        [Key]
        public Guid PersonId { get; set; }
        //nvarchar(max) per value 2billon char length therefore it will be too big
        [StringLength(40)]  //nvarchar(40)
        public string? PersonName { get; set; }
        [StringLength(50)]
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        [StringLength(10)]
        public string? Gender { get; set; }

        //data type uniqueidentifier in sql
        public Guid? CountryID { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }
        //bit
        public bool RecieveNewsLetter { get; set; }
        public string? TIN { get; set; }

        //Navigation Property
        [ForeignKey("CountryID")]
        public virtual Country? Country { get; set; }

        public override string ToString()
        {
            return $"Person ID:{PersonId}, Person Name:{PersonName}, Email:{Email}, Gender:{Gender}, Country Name:{Country!.CountryName}, Address:{RecieveNewsLetter}, TIN:{TIN}";
        }

    }
}
