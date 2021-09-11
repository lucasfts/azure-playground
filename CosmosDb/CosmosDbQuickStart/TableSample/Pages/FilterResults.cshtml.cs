using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TableSample.Models;
using TableSample.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace TableSample.Pages
{
    public class FilterResultsModel : PageModel
    {
        private readonly ILogger<FilterResultsModel> _logger;
        private TableService _tablesService;

        public IEnumerable<string> ColumnNames { get; set; }
        public IEnumerable<WeatherDataModel> WeatherObservations { get; set; }


        public FilterResultsModel(ILogger<FilterResultsModel> logger, TableService tablesService)
        {
            _logger = logger;
            _tablesService = tablesService;
        }


        public void OnGet(FilterResultsInputModel inputModel)
        {
            WeatherObservations = _tablesService.GetFilteredRows(inputModel);

            ColumnNames = WeatherObservations.SelectMany(e => e.PropertyNames).Distinct();
        }




    }
}
