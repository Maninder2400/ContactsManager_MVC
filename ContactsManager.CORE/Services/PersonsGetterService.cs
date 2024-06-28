using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;
using OfficeOpenXml;
using RepositoryContracts;
using Microsoft.Extensions.Logging;
using Serilog;
using SerilogTimings;
using System;

namespace Services
{
    public class PersonsGetterService : IPersonsGetterService
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<IPersonsGetterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;
        public PersonsGetterService(IPersonsRepository personsRepository, ILogger<IPersonsGetterService> logger, IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

       /* /// <summary>
        /// As person class doesn't contain any Country property but personResponse is expecting the value of Country,
        /// there for before convering Person to person response we have to call GetCountryByID method from CountryService
        /// so to avoid calling both function every time ,we can just call this method to Supply PersonResponse obeject which
        /// also take care of intializing the country property of PersonResponse DTO class
        /// </summary>
        /// <param name="person">Person object to convert</param>
        /// <returns>PersonResponse object</returns>
        private PersonResponse ConvertPersonToPersonResponse(Person person)
        {
            PersonResponse personResponse = person.ToPersonResponse();
            //personResponse.Country = _CountriesService.GetCountryByCountryID(personResponse.CountryID)?.CountryName;
            personResponse.Country = person.Country?.CountryName;
            return personResponse;
        }
    */
        public virtual async Task<List<PersonResponse>> GetAllPersons()
        {
            var persons = await _personsRepository.GetAllPersons();
            return persons.Select(temp => temp.ToPersonResponse()).ToList();
            //return _db.sp_GetAllPersons().Select(temp => ConvertPersonToPersonResponse(temp)).ToList();
        }

        public virtual async Task<PersonResponse?> GetPersonByPersonID(Guid? personID)
        {
            if (personID == null)
                return null;

            Person? person = await _personsRepository.GetPersonByPersonId(personID.Value);

            if (person == null)
                return null;

            return person.ToPersonResponse();
        }

        public virtual async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
        {
            _logger.LogInformation("GetFilteredPersons of Persons service");
            List<Person>? personResponses = null;
            using (Operation.Time("Time for Get Filtered person from database"))
            {
                 personResponses = searchBy switch
                {
                    nameof(PersonResponse.PersonName) => await
                        _personsRepository.GetFilteredPersons(temp => temp.PersonName.Contains(searchString)),
                    nameof(PersonResponse.Email) => await
                        _personsRepository.GetFilteredPersons(temp => temp.Email.Contains(searchString)),

                    nameof(PersonResponse.DateOfBrith) => await
                    _personsRepository.GetFilteredPersons(temp => temp.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString)),
                    nameof(PersonResponse.Address) => await
                    _personsRepository.GetFilteredPersons(temp => temp.Address.Contains(searchString)),
                    nameof(PersonResponse.Gender) => await
                        _personsRepository.GetFilteredPersons(temp => temp.Gender.Contains(searchString)),
                    nameof(PersonResponse.CountryID) => await
                                _personsRepository.GetFilteredPersons(temp => temp.Country.CountryName.Contains(searchString)),
                    _ => await _personsRepository.GetAllPersons(),
                };
            }
            _diagnosticContext.Set("Persons", personResponses); 
            return personResponses.Select(temp => temp.ToPersonResponse()).ToList();
        }
        public virtual async Task<MemoryStream> GetPersonsCSV()
        {
            MemoryStream memoryStream = new MemoryStream();
            //Stream writer writes the content in the MemoryStream
            StreamWriter streamWriter = new StreamWriter(memoryStream);

            CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);
            CsvWriter csvWriter = new(streamWriter, csvConfiguration);

            /*
            //Csv writer will write in SteamWriter
            //CultureInfo is to recogonize punctuation marks (, , . etc) .For any language we can create a custom object of CultureInfo class and Supply our own PuncMarks based on specfic language , InvariantCulture is default
            //LeaveOpen : After writing in order to convert written stream into 'file' we have to start from begining, for that it should be opened
            CsvWriter csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture, leaveOpen : true);


            csvWriter.WriteHeader<PersonResponse>(); // It will write a Header row based on Passed type like PersonName,PeronsId ...*/

            //o/p: PersonName,Email,DateOfBrith ... 
            csvWriter.WriteField(nameof(PersonResponse.PersonName));
            csvWriter.WriteField(nameof(PersonResponse.Email));
            csvWriter.WriteField(nameof(PersonResponse.DateOfBrith));
            csvWriter.WriteField(nameof(PersonResponse.Age));
            csvWriter.WriteField(nameof(PersonResponse.Gender));
            csvWriter.WriteField(nameof(PersonResponse.Country));
            csvWriter.WriteField(nameof(PersonResponse.Address));
            csvWriter.WriteField(nameof(PersonResponse.RecieveNewsLetter));
            csvWriter.NextRecord(); // from moving to the next line

            //List<PersonResponse> persons = _db.Persons.Include("Country").Select(p => p.ToPersonResponse()).ToList();
            List<PersonResponse> persons = await GetAllPersons();

            foreach( PersonResponse person in persons)
            {
                csvWriter.WriteField(person.PersonName);
                csvWriter.WriteField(person.Email);
                if (person.DateOfBrith.HasValue)
                    csvWriter.WriteField(person.DateOfBrith.Value.ToString("dd-MM-yyyy"));
                else
                    csvWriter.WriteField("");
                csvWriter.WriteField(person.Age);
                csvWriter.WriteField(person.Gender);
                csvWriter.WriteField(person.Country);
                csvWriter.WriteField(person.Address);
                csvWriter.WriteField(person.RecieveNewsLetter);
                csvWriter.NextRecord();
                csvWriter.Flush();//it will write data written into csv writter in memory stream
            }
            /*
            await csvWriter.WriteRecordsAsync(persons); // it will write persons in csv format (Comma saparated) eg:1,Name,...*/

            memoryStream.Position = 0; //after writing csv file cursor will be at the end of file there for in order to bring it to the start of the file(for reading purpose) position = 0;
            return memoryStream;
        }

        public virtual async Task<MemoryStream> GetPersonsExcel()
        {
            //memory stream can store the data of any type like csv or excel etc
            MemoryStream memoryStream = new MemoryStream();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet workSheet =  excelPackage.Workbook.Worksheets.Add("Persons");
                workSheet.Cells["A1"].Value = "Person Name";
                workSheet.Cells["B1"].Value = "Email";
                workSheet.Cells["C1"].Value = "Date of Birth";
                workSheet.Cells["D1"].Value = "Age";
                workSheet.Cells["E1"].Value = "Gender";
                workSheet.Cells["F1"].Value = "Country";
                workSheet.Cells["G1"].Value = "Address";
                workSheet.Cells["H1"].Value = "Recieve News Letter";
                using (ExcelRange headerCells = workSheet.Cells["A1:H1"])
                {
                    headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    headerCells.Style.Font.Bold = true;
                }

                int row = 2;
                List<PersonResponse> persons = await GetAllPersons();

                foreach(PersonResponse person in persons)
                {
                    workSheet.Cells[row, 1].Value = person.PersonName;
                    workSheet.Cells[row, 2].Value = person.Email;
                    if(person.DateOfBrith.HasValue)
                        workSheet.Cells[row, 3].Value = person.DateOfBrith.Value.ToString("dd-MM-yyyy");
                    workSheet.Cells[row, 4].Value = person.Age;
                    workSheet.Cells[row, 5].Value = person.Gender;
                    workSheet.Cells[row, 6].Value = person.Country;
                    workSheet.Cells[row, 7].Value = person.Address;
                    workSheet.Cells[row, 8].Value = person.RecieveNewsLetter;
                    row++;
                }

                workSheet.Cells[$"A1:H{row}"].AutoFitColumns();

                await excelPackage.SaveAsync();
                memoryStream.Position = 0;
                return memoryStream;
            }
        }
    }
}
