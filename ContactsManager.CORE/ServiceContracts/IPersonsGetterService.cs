using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manupulating Person Entity   
    /// </summary>
    public interface IPersonsGetterService
    {
        /// <summary>
        /// Returns all Person
        /// </summary>
        /// <returns>Returns the list of objects of PersonResponse type</returns>
        Task<List<PersonResponse>> GetAllPersons();

        /// <summary>
        /// Return the pero object based on the given person id
        /// </summary>
        /// <param name="personID">personID to search</param>
        /// <returns>Matching person object</returns>
        Task<PersonResponse?> GetPersonByPersonID(Guid? personID);
        /// <summary>
        /// Returns all the Person objects that matches with the given search field and search string
        /// </summary>
        /// <param name="searchBy">Field to be searched</param>
        /// <param name="searchString">Search string to search</param>
        /// <returns>List of personResponse objects based on the given search feild and search string</returns>
        Task<List<PersonResponse>> GetFilteredPersons(string searchBy,string? searchString);

        /// <summary>
        /// Returns persons as CSV 
        /// </summary>
        /// <returns>Returns the memory stream with csv data</returns>
        Task<MemoryStream> GetPersonsCSV();

        /// <summary>
        /// Returns persons as Excel
        /// </summary>
        /// <returns></returns>
        Task<MemoryStream> GetPersonsExcel();
    }
}
