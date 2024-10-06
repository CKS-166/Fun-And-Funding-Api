using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using Net.payOS;
using Fun_Funding.Application.ViewModel.WalletDTO;
using Org.BouncyCastle.Utilities;

namespace Fun_Funding.Api.Controllers
{
    [Route("api/PayOS")]
    [ApiController]
    public class PayOSController : ControllerBase
    {
        private readonly PayOS _payOS;

        public PayOSController(PayOS payOS)
        {
            _payOS = payOS;
        }

        [HttpGet("create-payment-link")]
        public async Task<IActionResult> Checkout([FromQuery] WalletRequest walletRequest)
        {
            try
            {
                int orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));
                string description = $"Add money to wallet";
                ItemData item = new ItemData(description, 1, (int)walletRequest.Balance);
                List<ItemData> items = new List<ItemData>();
                items.Add(item);
                PaymentData paymentData = new PaymentData(orderCode, (int)walletRequest.Balance, description, items, "http://localhost:5173/choose-project-plan", "http://localhost:5173/account/wallet");

                CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);

                return Redirect(createPayment.checkoutUrl);
            }
            catch (System.Exception exception)
            {
                //Console.WriteLine(exception);
                //return Redirect("https://localhost:3002/");
                throw new System.Exception(exception.Message);
            }
        }
    }
}
