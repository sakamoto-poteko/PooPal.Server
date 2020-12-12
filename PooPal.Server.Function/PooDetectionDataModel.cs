using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace PooPal.Server.Function
{
    public class PooDetectionDataModel
    {
        [JsonPropertyName("start")]
        public int Start { get; set; }

        [JsonPropertyName("elapsed")]
        public int Elapsed { get; set; }
    }
}
