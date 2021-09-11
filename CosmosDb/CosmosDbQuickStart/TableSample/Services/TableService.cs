using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TableSample.Entities;
using TableSample.Models;

namespace TableSample.Services
{
    public class TableService
    {
        private string[] EXCLUDE_TABLE_ENTITY_KEYS = { "PartitionKey", "RowKey", "odata.etag", "Timestamp" };
        private readonly TableClient tableClient;

        public TableService(TableClient tableClient)
        {
            this.tableClient = tableClient;
        }

        public IEnumerable<WeatherDataModel> GetAllRows()
        {
            var entities = tableClient.Query<TableEntity>();

            return entities.Select(x => MapTableEntityToWeatherDataModel(x));
        }


        public IEnumerable<WeatherDataModel> GetFilteredRows(FilterResultsInputModel inputModel)
        {
            var filters = new List<string>();

            if (!string.IsNullOrEmpty(inputModel.PartitionKey))
                filters.Add($"PartitionKey eq '{inputModel.PartitionKey}'");
            if (!String.IsNullOrEmpty(inputModel.RowKeyDateStart) && !String.IsNullOrEmpty(inputModel.RowKeyTimeStart))
                filters.Add($"RowKey ge '{inputModel.RowKeyDateStart} {inputModel.RowKeyTimeStart}'");
            if (!String.IsNullOrEmpty(inputModel.RowKeyDateEnd) && !String.IsNullOrEmpty(inputModel.RowKeyTimeEnd))
                filters.Add($"RowKey le '{inputModel.RowKeyDateEnd} {inputModel.RowKeyTimeEnd}'");
            if (inputModel.MinTemperature.HasValue)
                filters.Add($"Temperature ge {inputModel.MinTemperature.Value}");
            if (inputModel.MaxTemperature.HasValue)
                filters.Add($"Temperature le {inputModel.MaxTemperature.Value}");
            if (inputModel.MinPrecipitation.HasValue)
                filters.Add($"Precipitation ge {inputModel.MinTemperature.Value}");
            if (inputModel.MaxPrecipitation.HasValue)
                filters.Add($"Precipitation le {inputModel.MaxTemperature.Value}");

            var filter = String.Join(" and ", filters);
            var entities = tableClient.Query<TableEntity>(filter);

            return entities.Select(x => MapTableEntityToWeatherDataModel(x));
        }


        public WeatherDataModel MapTableEntityToWeatherDataModel(TableEntity entity)
        {
            var wheater = new WeatherDataModel();
            wheater.StationName = entity.PartitionKey;
            wheater.ObservationDate = entity.RowKey;
            wheater.Timestamp = entity.Timestamp;
            wheater.Etag = entity.ETag.ToString();

            var tableProperties = entity.Keys.Where(x => !EXCLUDE_TABLE_ENTITY_KEYS.Contains(x));

            foreach (var property in tableProperties)
                wheater[property] = entity[property];

            return wheater;
        }

        public void InsertTableEntity(WeatherInputModel model)
        {
            var entity = new TableEntity();
            entity.PartitionKey = model.StationName;
            entity.RowKey = $"{model.ObservationDate} {model.ObservationTime}";

            entity["Temperature"] = model.Temperature;
            entity["Humidity"] = model.Humidity;
            entity["Barometer"] = model.Barometer;
            entity["WindDirection"] = model.WindDirection;
            entity["WindSpeed"] = model.WindSpeed;
            entity["Precipitation"] = model.Precipitation;

            tableClient.AddEntity(entity);
        }


        public void UpsertTableEntity(WeatherInputModel model)
        {
            TableEntity entity = new TableEntity();
            entity.PartitionKey = model.StationName;
            entity.RowKey = $"{model.ObservationDate} {model.ObservationTime}";

            // The other values are added like a items to a dictionary
            entity["Temperature"] = model.Temperature;
            entity["Humidity"] = model.Humidity;
            entity["Barometer"] = model.Barometer;
            entity["WindDirection"] = model.WindDirection;
            entity["WindSpeed"] = model.WindSpeed;
            entity["Precipitation"] = model.Precipitation;

            tableClient.UpsertEntity(entity);
        }


        public void InsertExpandableData(ExpandableWeatherObject weatherObject)
        {
            var entity = new TableEntity();
            entity.PartitionKey = weatherObject.StationName;
            entity.RowKey = weatherObject.ObservationDate;

            foreach (var propertyName in weatherObject.PropertyNames)
            {
                var propertyValue = weatherObject[propertyName];
                entity[propertyName] = propertyValue;
            }

            tableClient.AddEntity(entity);
        }


        public void UpsertExpandableData(ExpandableWeatherObject weatherObject)
        {
            var entity = new TableEntity();
            entity.PartitionKey = weatherObject.StationName;
            entity.RowKey = weatherObject.ObservationDate;

            foreach (var propertyName in weatherObject.PropertyNames)
            {
                var propertyValue = weatherObject[propertyName];
                entity[propertyName] = propertyValue;
            }

            tableClient.UpsertEntity(entity);
        }

        public void RemoveEntity(string partitionKey, string rowKey)
        {
            tableClient.DeleteEntity(partitionKey, rowKey);
        }


        public void UpdateEntity(UpdateWeatherObject weatherObject)
        {
            var partitionKey = weatherObject.StationName;
            var rowKey = weatherObject.ObservationDate;

            var entity = tableClient.GetEntity<TableEntity>(partitionKey, rowKey).Value;

            foreach (var propertyName in weatherObject.PropertyNames)
            {
                var propertyValue = weatherObject[propertyName];
                entity[propertyName] = propertyValue;
            }

            tableClient.UpdateEntity(entity, new ETag(weatherObject.Etag));
        }
    }
}
