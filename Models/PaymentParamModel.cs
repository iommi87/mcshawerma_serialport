using System.Text.Json.Serialization;

namespace McShawermaSerialPort.Models
{
    public class PaymentParamModel
    {
        [JsonPropertyName("terminal_alias")]
        public string TerminalAlias { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("has_printer")]
        public bool HasPrinter { get; set; }
    }
}