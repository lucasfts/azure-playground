using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TableSample.Models;
using TableSample.Utilities;

namespace TableSample.Filters
{
    public class ValidationFilter : IPageFilter
    {
        public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
           
        }

        public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new MessageModel
                    {
                        Level = MessageLevel.Danger,
                        Message = x.Value.Errors.First().ErrorMessage
                    }).ToArray();

                ((PageModel)context.HandlerInstance).TempData.Put("errors", errors);

                var name = context.ActionDescriptor.Endpoint.DisplayName;
                context.Result = new RedirectToPageResult(context.ActionDescriptor.Endpoint.DisplayName, new { });
            }
        }

        public void OnPageHandlerSelected(PageHandlerSelectedContext context)
        {
            
        }
    }
}
