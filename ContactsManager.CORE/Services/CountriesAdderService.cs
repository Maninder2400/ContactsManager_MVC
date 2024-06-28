using Entities;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesAdderService : ICountriesAdderService
    {
        private readonly ICountriesRepository _countriesRepository;
        //Constructor
        public CountriesAdderService(ICountriesRepository countriesRepository) { 
            _countriesRepository = countriesRepository;
        }
        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            //Validation: CountryAddRequest parameter can't be null
            if(countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }

            //Validation: Country Name can't be null
            if(countryAddRequest.CountryName == null)
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }

            //Validiation: Country name Shouldn't be duplicate
            if(await _countriesRepository.GetCountryByCountryName(countryAddRequest.CountryName) != null)
                throw new ArgumentException("Given country name already exists");

            //Convert object from CountryAddRequest to Country type (DTO to Domain Model class)
            Country country = countryAddRequest.ToCountry();

            //Generate countryID
            country.CountryId = Guid.NewGuid();

            //Add country to the countries list
            await _countriesRepository.AddCountry(country);

            //return DTO object via converting Domain Model Object(Country object) to CountryResponse object
            return country.ToCountryResponse();
        }

        //public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
        //{
        //    throw new ArgumentNullException();
        //}
    }
}
