using McShawermaSerialPort.Helpers;
using McShawermaSerialPort.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace McShawermaSerialPort
{
    [ApiController]
    [Route("[controller]")]
    public class MainController : ControllerBase
    {
        private readonly IHostApplicationLifetime _hostLifetime;

        public readonly PaymentHelper _paymentHelper;
        public readonly PrintCashHelper _printCashHelper;

        public MainController(PaymentHelper _paymentHelper, PrintCashHelper _printCashHelper, IHostApplicationLifetime _hostLifetime)
        {
            this._hostLifetime = _hostLifetime;

            this._paymentHelper = _paymentHelper;
            this._printCashHelper = _printCashHelper;
        }

        [HttpGet("stop_app")]
        public IActionResult StopApp()
        {
            _hostLifetime.StopApplication();
            return Ok();
        }

        [HttpPost("pay")]
        public IActionResult Pay(PaymentParamModel request)
        {
            return Ok(_paymentHelper.Pay(request));
        }

        [HttpPost("print_cash")]
        public IActionResult PrintCash(PrintCashParamModel request)
        {
            var res = _printCashHelper.Print(request);
            return Ok(new PrintCashResponseModel 
            {
                Status = res.Key,
                Exception = res.Value
            });
        }

        [HttpPost("print_x_bill")]
        public IActionResult PrintXBill(PrintCashParamModel request)
        {
            var res = _printCashHelper.PrintXReport(request);
            return Ok(new PrintCashResponseModel
            {
                Status = res.Key,
                Exception = res.Value
            });
        }

        [HttpPost("print_z_bill")]
        public IActionResult PrintzBill(PrintCashParamModel request)
        {
            var res = _printCashHelper.PrintZReport(request);
            return Ok(new PrintCashResponseModel
            {
                Status = res.Key,
                Exception = res.Value
            });
        }
    }
}