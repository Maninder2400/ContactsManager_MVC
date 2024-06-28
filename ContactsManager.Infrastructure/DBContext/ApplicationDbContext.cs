using System;
using System.Collections.Generic;
using ContactsManager.Core.Domain.IdentityEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace Entities
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,Guid>       //represents Person Database (Code first approach) and IdentityDbContext offers predefined dbsets for Identity class and ofcourse we can create our own db sets also
    {
        public ApplicationDbContext(DbContextOptions options) : base(options){}
        public virtual DbSet<Country> Countries { get;set; }        //Represents Countries table in person database
        public virtual DbSet<Person> Persons { get;set; }        //Represents Persons table in person database

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapping Dbset to table in database
            modelBuilder.Entity<Country>().ToTable("Countries");  
            modelBuilder.Entity<Person>().ToTable("Persons");

            //Seed to Countries table
            /* modelBuilder.Entity<Country>().HasData(new Country()
             {
                 CountryId = new Guid() , CountryName = "USA"
             }); */

            //instead of seeding data one by one do the following 
            //we have data in the form of json file
            //firstly read it in a string
            string countryJson = System.IO.File.ReadAllText("countries.json");

            //deserialize to list of counties
            List<Country>? countries  =  System.Text.Json.JsonSerializer.Deserialize<List<Country>>(countryJson);

            //seeding extracted data using loop
            foreach(Country country in countries)
            {
                modelBuilder.Entity<Country>().HasData(country);
            }

            //Seed persons
            string personsJson = System.IO.File.ReadAllText("persons.json");

            //deserialize to list of counties
            List<Person> persons = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(personsJson);

            //seeding extracted data using loop
            foreach (Person person in persons)
            {
                modelBuilder.Entity<Person>().HasData(person);
            }

            //Fluent API
            modelBuilder.Entity<Person>().Property(temp => temp.TIN).HasColumnName("TaxIdentificationNumber").HasColumnType("varchar(8)").HasDefaultValue("abc12345");

            modelBuilder.Entity<Person>().HasCheckConstraint("CHK_TIN", "len([TaxIdentificationNumber]) = 8");

            /*modelBuilder.Entity<Person>(entity =>
            {
                entity.HasOne<Country>(entity => entity.Country).WithMany(entity => entity.Persons).HasForeignKey(p => p.CountryID);
            });*/

        }

        public List<Person> sp_GetAllPersons()
        {
            return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
        }

        public int sp_InsertPerson(Person person)
        {
            SqlParameter[] sqlParameters = new SqlParameter[] { 
                new SqlParameter("PersonID",person.PersonId),
                new SqlParameter("PersonName",person.PersonName),
                new SqlParameter("Email",person.Email),
                new SqlParameter("DateOfBirth",person.DateOfBirth),
                new SqlParameter("Gender",person.Gender),
                new SqlParameter("CountryID",person.CountryID),
                new SqlParameter("Address",person.Address),
                new SqlParameter("RecieveNewsLetter",person.RecieveNewsLetter),
            };

            return Database.ExecuteSqlRaw("EXECUTE [dbo].[InsertPerson]@PersonId,@PersonName,@Email,@DateOfBirth,@Gender,@CountryID,@Address,@RecieveNewsLetter",sqlParameters);
        }
    }
}
