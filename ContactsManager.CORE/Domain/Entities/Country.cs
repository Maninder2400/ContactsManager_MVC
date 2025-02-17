﻿using System.ComponentModel.DataAnnotations;

namespace Entities
{
    /// <summary>
    /// Domain Model for Country
    /// </summary>
    public class Country
    {
        [Key]
        public Guid CountryId { get; set; }
        public string? CountryName { get; set; }

        //Navigation property
        public virtual ICollection<Person>? Persons {get; set;}

    }
}
