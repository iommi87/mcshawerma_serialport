using System.Text.Json.Serialization;

namespace McShawermaSerialPort.Models
{
    public class PrintResponseModel
    {
        [JsonPropertyName("status")]
        public bool Status { get; set; }

        [JsonPropertyName("exception")]
        public string Exception { get; set; }
    }
}
