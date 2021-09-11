﻿using TableSample.Data;
using TableSample.Entities;
using TableSample.Models;
using TableSample.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TableSample.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private TableService _tablesService;


        public string[] EXCLUDE_FORM_KEYS = { "stationName", "observationDate", "observationTime", "etag", "__RequestVerificationToken" };


        public IndexModel(ILogger<IndexModel> logger, TableService tablesService)
        {
            _logger = logger;
            _tablesService = tablesService;
        }


        public IEnumerable<string> FieldNames { get; set; }
        public IEnumerable<WeatherDataModel> WeatherObservations { get; set; }


        public void OnGet()
        {
            WeatherObservations = _tablesService.GetAllRows();

            FieldNames = WeatherObservations.SelectMany(e => e.PropertyNames).Distinct();           
        }


        public IActionResult OnPostInsertTableEntity(WeatherInputModel model)
        {
            _tablesService.InsertTableEntity(model);

            return RedirectToPage("index", "Get");
        }

        public IActionResult OnPostUpsertTableEntity(WeatherInputModel model)
        {
            _tablesService.UpsertTableEntity(model);

            return RedirectToPage("index", "Get");
        }


        public IActionResult OnPostInsertExpandableData(ExpandableWeatherInputModel model)
        {
            ExpandableWeatherObject weatherObject = new ExpandableWeatherObject();
            weatherObject.StationName = model.StationName;
            weatherObject.ObservationDate = $"{model.ObservationDate} {model.ObservationTime}";

            // The rest of the properties and values are in the form.  But we want to exclude the elements we
            // already have from the model and the __RequestVerificationToken when we build our dictionary
            var propertyNames = Request.Form.Keys.Where(key => !EXCLUDE_FORM_KEYS.Contains(key));
            foreach (string name in propertyNames)
            {
                string value = Request.Form[name].First();

                if (Double.TryParse(value, out double number))
                    weatherObject[name] = number;
                else
                    weatherObject[name] = value;
            }

            _tablesService.InsertExpandableData(weatherObject);

            return RedirectToPage("index", "Get");
        }


        public IActionResult OnPostUpsertExpandableData(ExpandableWeatherInputModel model)
        {
            ExpandableWeatherObject weatherObject = new ExpandableWeatherObject();
            weatherObject.StationName = model.StationName;
            weatherObject.ObservationDate = $"{model.ObservationDate} {model.ObservationTime}";

            // The rest of the properties and values are in the form.  But we want to exclude the elements we
            // already have from the model and the __RequestVerificationToken when we build our dictionary
            var propertyNames = Request.Form.Keys.Where(key => !EXCLUDE_FORM_KEYS.Contains(key));
            foreach (string name in propertyNames)
            {
                string value = Request.Form[name].First();

                if (Double.TryParse(value, out double number))
                    weatherObject[name] = number;
                else
                    weatherObject[name] = value;
            }

            _tablesService.UpsertExpandableData(weatherObject);

            return RedirectToPage("index", "Get");
        }


        public IActionResult OnPostRemoveEntity(string stationName, string observationDate)
        {
            _tablesService.RemoveEntity(stationName, observationDate);            

            return RedirectToPage("index", "Get");
        }


        public IActionResult OnPostInsertSampleData(string units, string city)
        {
            var bulkData = SampleWeatherData.GetSampleData(units, city);

            foreach (var item in bulkData)
                _tablesService.UpsertTableEntity(item);

            return RedirectToPage("index", "Get");
        }


        public IActionResult OnPostUpdateEntity(string stationName, string observationDate, string etag)
        {
            UpdateWeatherObject weatherObject = new UpdateWeatherObject();
            weatherObject.StationName = stationName;
            weatherObject.ObservationDate = observationDate;
            weatherObject.Etag = etag;

            // The rest of the properties and values are in the form.  But we want to exclude the elements we
            // already have from the model and the __RequestVerificationToken when we build our dictionary
            var propertyNames = Request.Form.Keys.Where(key => !EXCLUDE_FORM_KEYS.Contains(key));
            foreach (string name in propertyNames)
            {
                string value = Request.Form[name].First();

                if (Double.TryParse(value, out double number))
                    weatherObject[name] = number;
                else
                    weatherObject[name] = value;
            }

            _tablesService.UpdateEntity(weatherObject);

            return RedirectToPage("index", "Get");
        }


    }
}
