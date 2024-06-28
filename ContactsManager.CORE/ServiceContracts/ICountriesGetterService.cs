using ServiceContracts.DTO;
using Microsoft.AspNetCore.Http;

namespace ServiceContracts
{
    /// <summary>
    ///Represents bussiness logic for manupulating Country entity
    /// </summary>
    public interface ICountriesGetterService
    {
        /// <summary>
        /// Returns all the countries from the list
        /// </summary>
        /// <returns>All the countries from countries List as a List of Counrty Response</returns>
        Task<List<CountryResponse>> GetCountryList();

        /// <summary>
        /// Returns a country object based on given country id
        /// </summary>
        /// <param name="countryID">Country ID (Guid) to search</param>
        /// <returns>Matching Country as CountryResponse object</returns>
        Task<CountryResponse?> GetCountryByCountryID(Guid? countryID);
    }
}
