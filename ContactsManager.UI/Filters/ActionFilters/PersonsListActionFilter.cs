using CRUDExample.Controllers;
using Entities;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDExample.Filters.ActionFilters
{
    public class PersonsListActionFilter : IActionFilter
    {
        private readonly ILogger<PersonsListActionFilter> _logger;

        public PersonsListActionFilter(ILogger<PersonsListActionFilter> logger)
        {
            _logger = logger;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            //This method executes AFTER the action method of a particular route in the request pipeline completed its execution but has not retured anything yet
            _logger.LogInformation("{FillterName}.{MethodName} method",nameof(PersonsListActionFilter),nameof(OnActionExecuted));
            PersonsController personController = ((PersonsController)context.Controller);

            Dictionary<string, object?>? parameters = (Dictionary<string, object?>?)context.HttpContext.Items["arguments"];

            if (parameters != null)
            {
                if (parameters.ContainsKey("searchBy"))
                    personController.ViewData["CurrentSearchBy"] = Convert.ToString(parameters["searchBy"]);
                if (parameters.ContainsKey("searchString"))
                    personController.ViewData["CurrentSearchString"] = Convert.ToString(parameters["searchString"]);
                if (parameters.ContainsKey("sortBy"))
                    personController.ViewData["CurrentSortBy"] = Convert.ToString(parameters["sortBy"]);
                else
                    personController.ViewData["CurrentSortBy"] = nameof(PersonResponse.PersonName);
                if (parameters.ContainsKey("sortOrder"))
                    personController.ViewData["CurrentSortOrder"] = Convert.ToString(parameters["sortOrder"]);
                else
                    personController.ViewData["CurrentSortOrder"] = nameof(SortOrderOptions.ASC);
            }

            personController.ViewData["SearchFields"] = new Dictionary<string, string>()
            {
                { nameof(PersonResponse.PersonName) , "Person Name"},
                { nameof(PersonResponse.Email) , "Email"},
                { nameof(PersonResponse.Gender) , "Gender"},
                { nameof(PersonResponse.CountryID) , "Country"},
                { nameof(PersonResponse.Address) , "Address"},
                { nameof(PersonResponse.DateOfBrith) , "Date of Birth"},
            };
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            //This method executes BEFORE the action method of a particular route in the request pipeline completed its execution
            _logger.LogInformation("{FillterName}.{MethodName} method",nameof(PersonsListActionFilter),nameof(OnActionExecuting));

            context.HttpContext.Items["arguments"] = context.ActionArguments;
             
            if (context.ActionArguments.ContainsKey("searchBy"))
            {
                string? searchBy = context.ActionArguments["searchBy"]?.ToString();

                //Validate if user has supplied searchBy parameter value other than the available values (directly from url bar) than set the value to someother string 
                if (!string.IsNullOrEmpty(searchBy))
                {
                    var searchByOptions = new List<string>() {
                    nameof(PersonResponse.PersonName),
                    nameof(PersonResponse.Email),
                    nameof(PersonResponse.Gender),
                    nameof(PersonResponse.Address),
                    nameof(PersonResponse.DateOfBrith),
                    nameof(PersonResponse.CountryID),
                    };

                    if (!searchByOptions.Any(t => t == searchBy)) {
                        _logger.LogInformation("searchBy actual parameter {searchBy}", searchBy);

                        context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName);
                    }
                }
            }
        }
    }
}
