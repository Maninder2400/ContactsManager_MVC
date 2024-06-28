using CRUDExample.Filters;
using CRUDExample.Filters.ActionFilters;
using CRUDExample.Filters.AuthorizationFilters;
using CRUDExample.Filters.ExceptionFilters;
using CRUDExample.Filters.ResourceFilters;
using CRUDExample.Filters.ResultFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDExample.Controllers
{
    [Route("[controller]")]
    //[TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[] { "My-_key-From-Controller-Level", "My-_value-From-Controller-Level",3}, Order = 3)]
    //[ResponseHeaderActionFilter("My-_key-From-Controller-Level", "My-_value-From-Controller-Level", 3)]
    [ResponseHeaderFilterFactory("My-_key-From-Controller-Level", "My-_value-From-Controller-Level", 3)]
    [TypeFilter(typeof(PersonAlwaysRunResultFilter))]
    public class PersonsController : Controller
    {
        private readonly IPersonsGetterService _personsGetterService;
        private readonly IPersonsAdderService _personsAdderService;
        private readonly IPersonsSorterService _personsSorterService;
        private readonly IPersonsUpdaterService _personsUpdaterService;
        private readonly IPersonsDeleterService _personsDeleterService;
        private readonly ICountriesGetterService _countriesGetterService;
        private readonly ILogger<PersonsController> _logger;

        public PersonsController(IPersonsGetterService personsGetterService,IPersonsAdderService personsAdderService,IPersonsDeleterService personsDeleterService,IPersonsSorterService personsSorterService,IPersonsUpdaterService personsUpdaterService ,ICountriesGetterService countriesGetterService,ILogger<PersonsController> ilogger)
        {
            _personsGetterService = personsGetterService;
            _personsAdderService = personsAdderService;
            _personsDeleterService = personsDeleterService;
            _personsSorterService = personsSorterService;
            _personsUpdaterService = personsUpdaterService;
            _countriesGetterService = countriesGetterService;
            _logger = ilogger;
        }
        [Route("[action]")]
        [Route("/")] 
        [ServiceFilter(typeof(PersonsListActionFilter) , Order = 4)]
        //[ResponseHeaderActionFilter("My-_key-From-Action-Method", "My-_value-From-Action-Method",1)]
        [ResponseHeaderFilterFactory("My-_key-From-Action-Method", "My-_value-From-Action-Method",1)]
        [TypeFilter(typeof(PersonsListResultFilter))]
        [TypeFilter(typeof(SkipFilter))]
        public async Task<IActionResult> Index(string searchBy, string? searchString,string sortBy = nameof(PersonResponse.PersonName),SortOrderOptions sortOrder = SortOrderOptions.ASC)
            {
            //for tracking the execution flow path
            _logger.LogInformation("Index action method of person controller");

            //for checking the value of parameters
            _logger.LogDebug($"searchBy:{searchBy} searchString:{searchString} sortBy:{sortBy} sortOrder{sortOrder}");
            //Search
            List<PersonResponse> persons = await _personsGetterService.GetFilteredPersons(searchBy,searchString);

            //ViewBag.CurrentSearchBy = searchBy;
            //ViewBag.CurrentSearchString = searchString;

            //Sort
            List<PersonResponse> sortedPersons = await _personsSorterService.GetSortedPersons(persons, sortBy, sortOrder);
            //ViewBag.CurrentSortBy = sortBy;
            //ViewBag.CurrentSortOrder = sortOrder.ToString();
            return View(sortedPersons);
        }

        [Route("[action]")]
        [HttpGet]
        //[ResponseHeaderActionFilter("My-_key-From-Action-Method", "My-_value-From-Action-Method",4)]
        [ResponseHeaderFilterFactory("My-_key-From-Action-Method", "My-_value-From-Action-Method",4)]
        public async Task<IActionResult> Create()
        {
            List<CountryResponse> countries = await _countriesGetterService.GetCountryList();
            ViewBag.Countries = countries.Select(temp => new SelectListItem { Text = temp.CountryName, Value = temp.CountryID.ToString() }); 
            return View();
        }

		[Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> Create(PersonAddRequest personRequest)
        {
            await _personsAdderService.AddPerson(personRequest);
            return RedirectToAction("Index","Persons"); 
        }

        [Route("[action]")]
        [HttpPost]
        [TypeFilter(typeof(FeatureDisabledResourceFilter),Arguments = new object[] {false})]
        [TypeFilter(typeof(PersonCreateAndEditPostActionFilter),Arguments = new object[] {nameof(Create)})]
        public async Task<IActionResult> PreviewPerson(PersonAddRequest personRequest)
        {
            CountryResponse? countryResponse = await _countriesGetterService.GetCountryByCountryID(personRequest.CountryID);
            ViewBag.Country = countryResponse!.CountryName;

            return View(personRequest);
        }

        [HttpGet]
        [Route("[action]/{personID}")]  //eg:/persons/edit/1
        [TypeFilter(typeof(TokenResultFilter))] 
        public async Task<IActionResult> Edit(Guid personID)
        {
            PersonResponse? personResponse =  await _personsGetterService.GetPersonByPersonID(personID);
            if (personResponse == null)
                return RedirectToAction("Index", "Persons");
            PersonUpdateRequest personUpdateRequest =  personResponse.ToPersonUpdateRequest();

            List<CountryResponse> countries = await _countriesGetterService.GetCountryList();
            ViewBag.Countries = countries.Select(temp => new SelectListItem { Text = temp.CountryName, Value = temp.CountryID.ToString() });

            return View(personUpdateRequest);
        }

        [Route("[action]/{personID}")]  //eg:/persons/edit/1
        [HttpPost]
        [TypeFilter(typeof(PersonCreateAndEditPostActionFilter),Arguments = new object[] {nameof(Edit)})]
        [TypeFilter(typeof(TokenAuthorizationFilter))]
        public async Task<IActionResult> Edit(PersonUpdateRequest personRequest)
        {
            PersonResponse? personResponse = await _personsGetterService.GetPersonByPersonID(personRequest.PersonID);
            if (personResponse == null)
                return RedirectToAction("Index", "Persons");

            //if (ModelState.IsValid)
            //{
                PersonResponse updatedPerson = await _personsUpdaterService.UpdatePerson(personRequest);
                return RedirectToAction("Index"); // optional controller name because we are in the same controller as of redirect view
           // }
           // else
            //{
            //    List<CountryResponse> countries = await _countriesService.GetCountryList();
            //    ViewBag.Countries = countries.Select(temp => new SelectListItem { Text = temp.CountryName, Value = temp.CountryID.ToString() });


            //    ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            //    return View(personResponse.ToPersonUpdateRequest());
            //}
        }

        [HttpGet]
        [Route("[action]/{personID}")]  //eg:/persons/delete/1
        public async Task<IActionResult> Delete(Guid? personID)
        {
            PersonResponse? personResponse = await _personsGetterService.GetPersonByPersonID(personID);
            if(personResponse == null)
                return RedirectToAction("Index");

            return View(personResponse);
        }

        [HttpPost]
        [Route("[action]/{personID}")]  //eg:/persons/delete/1
        public async Task<IActionResult> Delete(PersonUpdateRequest personUpdateRequest)
        {
            PersonResponse? personResponse = await _personsGetterService.GetPersonByPersonID(personUpdateRequest.PersonID);
            if (personResponse == null)
                return RedirectToAction("Index");

            await _personsDeleterService.DeletePerson(personUpdateRequest.PersonID);
            return RedirectToAction("Index");
        }

        [Route("[action]")]
        public async Task<IActionResult> PersonsPDF()
        {
            List<PersonResponse> persons = await _personsGetterService.GetAllPersons();
            return new ViewAsPdf("PersonsPDF", persons, ViewData) { PageMargins = new Rotativa.AspNetCore.Options.Margins(){Top = 20,Right = 20 , Bottom =20 , Left = 20  },
              PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
            };
        }

        [Route("[action]")]
        public async Task<IActionResult> PersonsCSV()
        {
            MemoryStream stream = await _personsGetterService.GetPersonsCSV();
            return File(stream , "application/octet-stream","persons.csv");
            //File(inputFile , Content type for csv file , name of result file)
        }
        
        [Route("[action]")]
        public async Task<IActionResult> PersonsExcel()
        {
            MemoryStream stream = await _personsGetterService.GetPersonsExcel();
            return File(stream , "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "persons.xlsx");
            //File(inputFile , Content type for csv file , name of result file)
        }
    }
}
