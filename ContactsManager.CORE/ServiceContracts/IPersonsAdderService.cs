using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manupulating Person Entity   
    /// </summary>
    public interface IPersonsAdderService
    {
        /// <summary>
        /// Adds the new Person into the list of persons
        /// </summary>
        /// <param name="PersonAddRequest">Person to Add</param>
        /// <returns>Returns the new person details along with the newly generated personID</returns>
        Task<PersonResponse> AddPerson(PersonAddRequest? PersonAddRequest);

    }
}
