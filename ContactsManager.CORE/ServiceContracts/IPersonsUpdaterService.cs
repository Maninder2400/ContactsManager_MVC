using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manupulating Person Entity   
    /// </summary>
    public interface IPersonsUpdaterService
    {
        /// <summary>
        /// Updates the specified person details based on the given personID
        /// </summary>
        /// <param name="PersonUpdateRequest">Person details to update including personID</param>
        /// <returns>returns the person object after updation</returns>
        Task<PersonResponse> UpdatePerson(PersonUpdateRequest? PersonUpdateRequest);
    }
}
