using ServiceContracts;
using ServiceContracts.DTO;
using System;
using System.Collections.Generic;
using Entities;
using Services.Helpers;
using RepositoryContracts;
using Microsoft.Extensions.Logging;
using Serilog;
using SerilogTimings;
using Exceptions;

namespace Services
{
    public class PersonsAdderService : IPersonsAdderService
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<IPersonsAdderService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;
        public PersonsAdderService(IPersonsRepository personsRepository, ILogger<IPersonsAdderService> logger, IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

       /* /// <summary>
        /// As person class doesn't contain any Country property but personResponse is expecting the value of Country,
        /// there for before convering Person to person response we have to call GetCountryByID method from CountryService
        /// so to avoid calling both function every time ,we can just call this method to Supply PersonResponse obeject which
        /// also take care of intializing the country property of PersonResponse DTO class
        /// </summary>
        /// <param name="person">Person object to convert</param>
        /// <returns>PersonResponse object</returns>
        private PersonResponse ConvertPersonToPersonResponse(Person person)
        {
            PersonResponse personResponse = person.ToPersonResponse();
            //personResponse.Country = _CountriesService.GetCountryByCountryID(personResponse.CountryID)?.CountryName;
            personResponse.Country = person.Country?.CountryName;
            return personResponse;
        }
    */
        public async Task<PersonResponse> AddPerson(PersonAddRequest? PersonAddRequest)
        {
            //check id person request is not null
            if (PersonAddRequest == null)
                throw new ArgumentNullException(nameof(PersonAddRequest));
            Person person = PersonAddRequest.ToPerson();

            //Model Validation
            ValidationHelper.ModelValidation(PersonAddRequest);

            //generate new personID
            person.PersonId = Guid.NewGuid();
            Person? person_recieved = await _personsRepository.AddPerson(person);
            //int rowsEffected = _db.sp_InsertPerson(person);

            //return PersonResponse object
            return person.ToPersonResponse();
        }
    }
}
