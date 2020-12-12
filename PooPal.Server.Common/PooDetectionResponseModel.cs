using System;
using System.Text.Json.Serialization;

namespace PooPal.Server.Common
{
    public class PooDetectionResponseModel
    {
        [JsonPropertyName("pooStart")]
        public DateTime PooStart { get; set; }

        [JsonPropertyName("pooDuration")]
        public int PooDuration { get; set; }
    }
}
