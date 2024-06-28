using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using RepositoryContracts;
using Serilog;
using ServiceContracts;
using ServiceContracts.DTO;


namespace Services
{
    public class PersonsGetterServiceChild : PersonsGetterService
    {
        //so if we are inheriting from a class we have call the parent class constructor from child class because when child class's object will be created it will have an parent class members in it which requires the object creation of the parent class,and parent class constructor require some arguments which we have supply
        public PersonsGetterServiceChild(IPersonsRepository personsRepository, ILogger<IPersonsGetterService> logger, IDiagnosticContext diagnosticContext) : base(personsRepository, logger, diagnosticContext)
        {
        }
        public async override Task<MemoryStream> GetPersonsExcel()
        {
            //memory stream can store the data of any type like csv or excel etc
            MemoryStream memoryStream = new MemoryStream();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets.Add("Persons");
                workSheet.Cells["A1"].Value = "Person Name";
                workSheet.Cells["B1"].Value = "Age";
                workSheet.Cells["C1"].Value = "Gender";
                using (ExcelRange headerCells = workSheet.Cells["A1:C1"])
                {
                    headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    headerCells.Style.Font.Bold = true;
                }

                int row = 2;
                List<PersonResponse> persons = await GetAllPersons();

                foreach (PersonResponse person in persons)
                {
                    workSheet.Cells[row, 1].Value = person.PersonName;
                    workSheet.Cells[row, 2].Value = person.Age;
                    workSheet.Cells[row, 3].Value = person.Gender;
                    row++;
                }

                workSheet.Cells[$"A1:C{row}"].AutoFitColumns();

                await excelPackage.SaveAsync();
                memoryStream.Position = 0;
                return memoryStream;
            }
        }
    }
}
