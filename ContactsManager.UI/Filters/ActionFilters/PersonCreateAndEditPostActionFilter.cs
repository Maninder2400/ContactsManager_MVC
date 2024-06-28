using CRUDExample.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;

namespace CRUDExample.Filters.ActionFilters
{
    public class PersonCreateAndEditPostActionFilter : IAsyncActionFilter
    {
        private readonly ICountriesGetterService _countriesGetterService;
        private readonly string _viewName;
        private readonly ILogger<PersonCreateAndEditPostActionFilter> _logger;

        public PersonCreateAndEditPostActionFilter(ICountriesGetterService countriesGetterService,string viewName, ILogger<PersonCreateAndEditPostActionFilter> logger)
        {
            _countriesGetterService = countriesGetterService;
            _viewName = viewName;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.Controller is PersonsController personsController)
            {
                if (!personsController.ModelState.IsValid)
                {
                    List<CountryResponse> countries = await _countriesGetterService.GetCountryList();
                    personsController.ViewBag.Countries = countries.Select(temp => new SelectListItem { Text = temp.CountryName, Value = temp.CountryID.ToString() });


                    personsController.ViewBag.Errors = personsController.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    var personRequest = context.ActionArguments["personRequest"];
                    context.Result = personsController.View($"{_viewName}",personRequest);//short circuits or skips the subsequent action filters and action method
                }
                else
                    await next();//invokes the subsiquent filter or action method
            }
            else
                await next();

            //after logic
            _logger.LogInformation("In the after logic of Person create and edit action filter");
        }
    }
}
