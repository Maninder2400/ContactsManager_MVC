using Entities;
using System;
using System.Collections.Generic;
using ServiceContracts.Enums;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO
{
    public class PersonAddRequest
    {
        [Required ( ErrorMessage = "Person name can't be null")]
        public string? PersonName { get; set; }
        [Required ( ErrorMessage = "Email Address can't be null")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        public GenderOptions? Gender { get; set; }

        [Required(ErrorMessage = "Please select a country")]
        public Guid? CountryID { get; set; }

        public string? Address { get; set; }

        public bool RecieveNewsLetter { get; set; }

        /// <summary>
        /// Converts the current object of the PersonAddRequest into new object of Person type
        /// </summary>
        /// <returns></returns>
        public Person ToPerson()
        {
            return new Person()
            {
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
