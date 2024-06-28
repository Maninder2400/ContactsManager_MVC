using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manupulating Person Entity   
    /// </summary>
    public interface IPersonsDeleterService
    {
        /// <summary>
        /// Deletes person based on the given person id
        /// </summary>
        /// <param name="personID">PersonID to delete that person</param>
        /// <returns>Returns true is deletion is successfull ,otherwise retuns false</returns>
        Task<bool> DeletePerson(Guid? personID);
    }
}
