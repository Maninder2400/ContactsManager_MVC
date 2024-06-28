using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ActionFilters
{
    /*
    //Using the ActionFilterAttribute class,it does not support dependency injection,but offers shorter syntax while applying it to the method or class
    public class ResponseHeaderActionFilter : ActionFilterAttribute
    {
        private readonly string _key;
        private readonly string _value;
        //parameterized action filter
        public ResponseHeaderActionFilter(string key, string value, int order)
        {
            _key = key;
            _value = value;
            Order = order;
        }
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //before 
            
            await next();//calls the subsequent filter or action method if no action filter remains in the pipeline ,if this next delegate is not used it will short circuit the filter pipeline

            //after
            context.HttpContext.Response.Headers[_key] = _value;
        }
    }*/
    public class ResponseHeaderFilterFactoryAttribute : Attribute, IFilterFactory
    {
        public bool IsReusable => false;
        private string? _key {  get; set; }
        private string? _value {  get; set; }
        private int Order {  get; set; }

        public ResponseHeaderFilterFactoryAttribute(string? key, string? value, int order)
        {
            _key = key;
            _value = value;
            Order = order;
        }

        //Controller invokes -> Filter Factory invokes -> Filter
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            //returns filter object
            var filter = serviceProvider.GetRequiredService<ResponseHeaderActionFilter>();
            filter.Key = _key;
            filter.Value = _value;
            filter.Order = Order;
            return filter;
        }
    }
    public class ResponseHeaderActionFilter : IAsyncActionFilter,IOrderedFilter
    {
        public string? Key { get; set; }
        public  string? Value { get; set; }
        public int Order { get; set; }
        
        private readonly ILogger<ResponseHeaderActionFilter> _logger;
        
        //parameterized action filter
        //as the object of this class is getting created by service provider we can use dependency injection
        public ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger) 
        { 
        _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //before 
            _logger.LogInformation("Before logic - ResponseHeaderActionFilter");
            await next();//calls the subsequent filter or action method if no action filter remains in the pipeline ,if this next delegate is not used it will short circuit the filter pipeline

            //after
            context.HttpContext.Response.Headers[Key] = Value;
            _logger.LogInformation("After logic - ResponseHeaderActionFilter");
        }
    }
}
