using Entities;

namespace RepositoryContracts
{
    /// <summary>
    /// Represents Data access logic for Managing Country entity
    /// </summary>
    public interface ICountriesRepository
    {
        /// <summary>
        /// Adds a new Country object to the data store
        /// </summary>
        /// <param name="country">Conntry object to add</param>
        /// <returns>Returns the country object after adding it to the data store</returns>
        Task<Country> AddCountry(Country country);

        /// <summary>
        /// Returns all the countries in the data store
        /// </summary>
        /// <returns>All the countries from the table </returns>
        Task<List<Country>> GetAllCountries();

        /// <summary>
        /// Returns a country object based on the given country id; otherwise,it returns null
        /// </summary>
        /// <param name="id">CountryID to search</param>
        /// <returns>Matching country or null</returns>
        Task<Country?> GetCountryByCountryId(Guid countryID);

        /// <summary>
        /// Return a country object based on the country name; otherwise, it returns null
        /// </summary>
        /// <param name="countryName">Name of the country to search</param>
        /// <returns>Matching country or null</returns>
        Task<Country?> GetCountryByCountryName(string countryName);
    }
}
