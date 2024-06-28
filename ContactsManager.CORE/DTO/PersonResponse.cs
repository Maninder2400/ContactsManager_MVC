using Entities;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// Used a DTO class that is used as return type for most of the Persons Service 
    /// </summary>
    public class PersonResponse
    {
        public string? PersonName { get; set; }
        
        public Guid PersonId { get; set; }

        public string? Email {  get; set; }
        public DateTime? DateOfBrith {  get; set; }

        public string? Address {  get; set; }
        public string? Gender {  get; set; }
        public string? Country {  get; set; }
        public Guid? CountryID {  get; set; }

        public bool RecieveNewsLetter { get; set; }
        public double? Age {  get; set; }

        /// <summary>
        /// Compares the cuurent objects data with the parameter object (because by default equals methods compare the references of objects thats why we have to override it) 
        /// </summary>
        /// <param name="obj">PersonResponse Object to compare</param>
        /// <returns>Ture or False, indicating weather all the person details are matched with the specified parameter object</returns>
        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            if(obj.GetType() != typeof(PersonResponse))
                return false;

            PersonResponse person = (PersonResponse)obj;

            return person.PersonId == PersonId && person.PersonName == PersonName && person.Email == Email && person.RecieveNewsLetter == RecieveNewsLetter && person.Address == Address && person.Age == Age && person.Country == Country && person.CountryID == CountryID ;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"PersonName:{PersonName},PersonID:{PersonId},Email:{Email},DateOfBirth:{DateOfBrith?.ToString("dd mm yyyy")},Address:{Address},Gender:{Gender},Country:{Country},CountryID:{CountryID},RecieveNewsLetter:{RecieveNewsLetter},Age:{Age}";
        }

        public PersonUpdateRequest ToPersonUpdateRequest()
        {
            return new PersonUpdateRequest()
            {
                PersonID = PersonId,
                PersonName = PersonName,
                Email = Email,
                Address = Address,
                Gender = (GenderOptions)Enum.Parse(typeof(GenderOptions), Gender!,true),
                RecieveNewsLetter = RecieveNewsLetter,
                DateOfBirth = DateOfBrith,
                CountryID = CountryID,
            };
        }

    }

    public static class PersonExtension
    {
        /// <summary>
        /// An extension method to convert Person(Domain Model) class object to PersonResponse type(DTO)
        /// </summary>
        /// <param name="person">The Person object to convert</param>
        /// <returns>Returns the converted PersonResponse object</returns>
        public static PersonResponse ToPersonResponse(this Person person)
        {
            return new PersonResponse()
            {
                PersonId = person.PersonId,
                PersonName = person.PersonName,
                Email = person.Email,
                Address = person.Address,
                Gender = person.Gender,
                RecieveNewsLetter = person.RecieveNewsLetter,
                DateOfBrith = person.DateOfBirth,
                Age = (person.DateOfBirth != null) ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25) : null,
                CountryID = person.CountryID,Country = person.Country?.CountryName
        };
        }
    }
}
