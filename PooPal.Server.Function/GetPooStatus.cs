using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Linq;
using PooPal.Server.Common;

namespace PooPal.Server.Function
{
    public static class GetPooStatus
    {
        [FunctionName("GetPooStatus")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [Table(PooDetectionDataEntity.TableName, Connection = "AzureWebJobsStorage")] CloudTable cloudTable,
            ILogger log)
        {
            string deviceId = req.Query["deviceId"];
            string startTimeStr = req.Query["from"];
            string endTimeStr = req.Query["to"];

            if (string.IsNullOrWhiteSpace(deviceId))
            {
                return new BadRequestResult();
            }

            DateTime startTime, endTime;

            bool hasStartTime = startTimeStr != StringValues.Empty;
            bool hasEndTime = endTimeStr != StringValues.Empty;
            bool startTimeValid = DateTime.TryParse(startTimeStr, out startTime);
            bool endTimeValid = DateTime.TryParse(endTimeStr, out endTime);

            if ((hasStartTime && !startTimeValid) || (hasEndTime && !endTimeValid))
            {
                return new BadRequestResult();
            }


            string queryCondition = TableQuery.GenerateFilterCondition(nameof(PooDetectionDataEntity.PartitionKey), QueryComparisons.Equal, PooDetectionDataEntity.GetPartitionKey(deviceId));

            if (hasStartTime || hasEndTime)
            {
                string dateQuery;
                var startTimeQuery = TableQuery.GenerateFilterConditionForDate(nameof(PooDetectionDataEntity.Started), QueryComparisons.GreaterThanOrEqual, startTime);
                var endTimeQuery = TableQuery.GenerateFilterConditionForDate(nameof(PooDetectionDataEntity.Started), QueryComparisons.LessThanOrEqual, endTime);
                if (hasStartTime && hasEndTime)
                {
                    dateQuery = TableQuery.CombineFilters(startTimeQuery, TableOperators.And, endTimeQuery);
                }
                else if (hasStartTime)
                {
                    dateQuery = startTimeQuery;
                }
                else
                {
                    dateQuery = endTimeQuery;
                }

                queryCondition = TableQuery.CombineFilters(queryCondition, TableOperators.And, dateQuery);
            }

            TableQuery<PooDetectionDataEntity> query = new TableQuery<PooDetectionDataEntity>().Where(queryCondition);

            IEnumerable<PooDetectionDataEntity> queryResult = cloudTable.ExecuteQuery<PooDetectionDataEntity>(query);


            return new OkObjectResult(queryResult.Select(entity => new PooDetectionResponseModel
            {
                PooStart = entity.Started,
                PooDuration = entity.Duration
            }));
        }
    }
}
