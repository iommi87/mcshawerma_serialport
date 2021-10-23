using McShawermaSerialPort.Helpers;
using McShawermaSerialPort.Models;
using Microsoft.AspNetCore.Mvc;

namespace McShawermaSerialPort
{
    [ApiController]
    [Route("[controller]")]
    public class MainController : ControllerBase
    {
        public readonly PaymentHelper _paymentHelper;
        public readonly PrintCashHelper _printCashHelper;

        public MainController(PaymentHelper _paymentHelper, PrintCashHelper _printCashHelper)
        {
            this._paymentHelper = _paymentHelper;
            this._printCashHelper = _printCashHelper;
        }
        
        [HttpPost("pay")]
        public IActionResult Pay(PaymentParamModel request)
        {
            return Ok(_paymentHelper.Pay(request));
        }

        [HttpPost("print_cash")]
        public IActionResult PrintCash(PrintCashParamModel request)
        {
            return Ok(_printCashHelper.Print(request));
        }
    }
}