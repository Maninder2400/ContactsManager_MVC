using ServiceContracts;
using ServiceContracts.DTO;
using System;
using System.Collections.Generic;
using Entities;
using System.ComponentModel.DataAnnotations;
using Services.Helpers;
using RepositoryContracts;
using Microsoft.Extensions.Logging;
using Serilog;
using SerilogTimings;
using Exceptions;

namespace Services
{
    public class PersonsUpdaterService : IPersonsUpdaterService
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<IPersonsUpdaterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;
        public PersonsUpdaterService(IPersonsRepository personsRepository, ILogger<IPersonsUpdaterService> logger, IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if(personUpdateRequest == null)
                throw new ArgumentNullException(nameof(personUpdateRequest));

            if (personUpdateRequest.PersonID == Guid.Empty)
                throw new ArgumentException("Invalid personID");

            //Validaions
            ValidationHelper.ModelValidation(personUpdateRequest);

            //Getting person object to update
            Person? matchingPerson = await _personsRepository.GetPersonByPersonId(personUpdateRequest.PersonID);

            if (matchingPerson == null)
                throw new InvalidPersonIDException("Given person id does not exists");

            //Update all details
            Person updatedPerson = await _personsRepository.UpdatePerson(personUpdateRequest.ToPerson());
            return updatedPerson.ToPersonResponse();    
        }
    }
}
