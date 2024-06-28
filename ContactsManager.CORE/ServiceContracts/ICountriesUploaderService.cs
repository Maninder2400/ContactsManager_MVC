using ServiceContracts.DTO;
using Microsoft.AspNetCore.Http;

namespace ServiceContracts
{
    /// <summary>
    ///Represents bussiness logic for manupulating Country entity
    /// </summary>
    public interface ICountriesUploaderService
    {
        /// <summary>
        /// Upload Countries from excel into database
        /// </summary>
        /// <param name="formFile">Excel file with list of countries</param>
        /// <returns>returns number of countries added</returns>
        Task<int> UploadCountriesFromExcelFile(IFormFile formFile);
    }
}
