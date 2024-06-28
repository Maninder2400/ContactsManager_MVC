using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class PersonsRepository : IPersonsRepository
    {
        ApplicationDbContext _db;
        private readonly ILogger<IPersonsRepository> _logger;
        public PersonsRepository(ApplicationDbContext db,ILogger<IPersonsRepository> logger) 
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Person> AddPerson(Person person)
        {
            _db.Persons.Add(person);
            await _db.SaveChangesAsync();
            return person;
        }

        public async Task<bool> DeletePersonByPersonID(Guid personID)
        {
            _db.Persons.RemoveRange(_db.Persons.Where(temp => temp.PersonId == personID));
            int rowsDeleted = await _db.SaveChangesAsync();
            return rowsDeleted > 0;
        }

        public async Task<List<Person>> GetAllPersons()
        {
            return await _db.Persons.Include("Country").ToListAsync();
        }

        public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
        {
            _logger.LogInformation("GetFilteredPersons of Persons Repository");
            return await _db.Persons.Include("Country").Where(predicate).ToListAsync();
        }

        public async Task<Person?> GetPersonByPersonId(Guid personID)
        {
            return await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonId == personID);
        }

        public async Task<Person> UpdatePerson(Person person)
        {
           Person? matchingPerson =  await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonId == person.PersonId);

            if (matchingPerson == null) return person;

            matchingPerson.PersonName = person.PersonName;
            matchingPerson.Gender = person.Gender;
            matchingPerson.Email = person.Email ;
            matchingPerson.Country = person.Country ;
            matchingPerson.CountryID = person.CountryID ;
            matchingPerson.RecieveNewsLetter = person.RecieveNewsLetter ;
            matchingPerson.DateOfBirth = person.DateOfBirth ;

            //Optional
            int countUpdate = await _db.SaveChangesAsync();

            return matchingPerson; 
        }
    }
}
