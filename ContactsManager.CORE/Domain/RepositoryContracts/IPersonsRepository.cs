using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts
{
    /// <summary>
    /// Represents Data access logic for Managing Person entity
    /// </summary>
    public interface IPersonsRepository
    {
        /// <summary>
        /// Adds a person object to the data store
        /// </summary>
        /// <param name="person">Person object to add</param>
        /// <returns>Returns the person object after adding it to the table</returns>
        Task<Person> AddPerson(Person person);

        /// <summary>
        /// Returns all persons in the data store
        /// </summary>
        /// <returns>List of person objects in the table</returns>
        Task<List<Person>> GetAllPersons();

        /// <summary>
        /// Returns person object based on the given person id
        /// </summary>
        /// <param name="id">PersonID(GUID) to search</param>
        /// <returns>A matching person or null</returns>
        Task<Person?> GetPersonByPersonId(Guid id);

        /// <summary>
        /// Returns all the person objects based on the given expression
        /// </summary>
        /// <param name="predicate">LINQ expression to check</param>
        /// <returns>All matching persons with given condition</returns>
        Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate);

        /// <summary>
        /// Deletes a person object based on the given person id
        /// </summary>
        /// <param name="personID">True,if deletion is successful ; otherwise false</param>
        /// <returns></returns>
        Task<bool> DeletePersonByPersonID(Guid personID);

        /// <summary>
        /// Updates an person object (person name and other details) based the given personID
        /// </summary>
        /// <param name="person">Person object to update</param>
        /// <returns>Returns the updated person object</returns>
        Task<Person> UpdatePerson(Person person);
    }
}
