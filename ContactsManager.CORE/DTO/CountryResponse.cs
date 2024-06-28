using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Entities;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// DTO class that is used as return type for most of the CountryService Methods
    /// </summary>
    public class CountryResponse
    {
        public Guid CountryID { get; set; }
        public string? CountryName { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            if(obj.GetType() != typeof(CountryResponse))
                return false;

            CountryResponse country_response = (CountryResponse)obj;

            return  CountryID == country_response.CountryID && CountryName == country_response.CountryName;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
    public static class CountryExtensions
    {
        public static CountryResponse ToCountryResponse(this Country country)
        {
            return new CountryResponse { CountryID = country.CountryId, CountryName = country.CountryName };
        }
    }
}
