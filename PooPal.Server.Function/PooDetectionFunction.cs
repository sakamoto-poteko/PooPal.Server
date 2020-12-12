using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

namespace PooPal.Server.Function
{
    public static class PooDetectionFunction
    {
        [FunctionName("PooDetectionFunction")]
        public static void Run(
            [IoTHubTrigger("messages/events", Connection = "AzureIoTHub")]EventData message,
            [Table(PooDetectionDataEntity.TableName, Connection = "AzureWebJobsStorage")] CloudTable cloudTable,
            ILogger log)
        {
            string devId = (string)message.SystemProperties["iothub-connection-device-id"];
            var msg = Encoding.UTF8.GetString(message.Body.Array);

            try
            {
                var model = JsonSerializer.Deserialize<PooDetectionDataModel>(msg);
                log.LogInformation($"C# IoT Hub trigger function processed a message: {msg}. From {devId}");

                var guid = Guid.NewGuid();
                var entity = new PooDetectionDataEntity(devId, guid)
                {
                    Started = DateTimeOffset.FromUnixTimeSeconds(model.Start).DateTime,
                    DeviceId = devId,
                    Duration = model.Elapsed
                };

                TableOperation insertOp = TableOperation.InsertOrMerge(entity);
                cloudTable.Execute(insertOp);
            }
            catch (JsonException)
            {
                log.LogError("invalid json");
            }
        }
    }
}