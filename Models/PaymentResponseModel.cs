using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace McShawermaSerialPort.Models
{
    public class PaymentResponseModel
    {
        [JsonPropertyName("dt")]
        public DateTime? dt { get; set; }

        [JsonPropertyName("status_id")]
        public int StatusId { get; set; }

        [JsonPropertyName("status_message")]
        public string StatusMessage { get; set; }

        [JsonPropertyName("response_code_host")]
        public string ResponseCodeHost { get; set; }

        [JsonPropertyName("authorization_code")]
        public string AuthorizationCode { get; set; }

        [JsonPropertyName("rfn")]
        public string Rrn { get; set; }

        [JsonIgnore]
        public List<string> Receipts { get; set; }

        [JsonPropertyName("raw_data")]
        public string RawData
        {
            get
            {
                if (Receipts != null && Receipts.Any())
                    return Receipts.ElementAt(0);
                else
                {
                    return null;
                }
            }
        }
    }
}