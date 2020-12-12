using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Cosmos.Table;


namespace PooPal.Server.Function
{
    public class PooDetectionDataEntity : TableEntity
    {
        public const string TableName = "PooDetection";

        public PooDetectionDataEntity()
        {
        }

        public PooDetectionDataEntity(string deviceId, Guid id)
        {
            this.PartitionKey = GetPartitionKey(deviceId);
            this.RowKey = GetRowId(deviceId, id);
        }

        public static string GetPartitionKey(string deviceId)
        {
            return deviceId;
        }

        public static string GetRowId(string deviceId, Guid id)
        {
            return $"{deviceId}_{id}";
        }

        public DateTime Started { get; set; }

        public int Duration { get; set; }

        public string DeviceId { get; set; }
    }
}
