using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ResultFilters
{
    public class PersonAlwaysRunResultFilter : IAlwaysRunResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext context)
        {
            if(context.Filters.OfType<SkipFilter>().Any()) { return; }
            //To do after logic
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            if(context.Filters.OfType<SkipFilter>().Any()) { return; }
            //To do before logic
        }
    }
}
