using Entities;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// Represents the DTO class that contains the person details to update
    /// </summary>
    public class PersonUpdateRequest
    {
        [Required(ErrorMessage = "PersonID can't be Blank")]
        public Guid PersonID { get; set; }

        [Required(ErrorMessage = "Person name can't be null")]
        public string? PersonName { get; set; }
        [Required(ErrorMessage = "Email Address can't be null")]
        [EmailAddress(ErrorMessage = "Invalid email fromat")]
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public GenderOptions? Gender { get; set; }

        public Guid? CountryID { get; set; }

        public string? Address { get; set; }

        public bool RecieveNewsLetter { get; set; }

        /// <summary>
        /// Converts the current object of the PersonUpdateRequest into new object of Person type
        /// </summary>
        /// <returns>Returns person object</returns>
        public Person ToPerson()
        {
            return new Person()
            {
                PersonId = PersonID,
                PersonName = PersonName,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Gender = Gender.ToString(),
                CountryID = CountryID,
                Address = Address,
                RecieveNewsLetter = RecieveNewsLetter,
            };
        }
    }
}
