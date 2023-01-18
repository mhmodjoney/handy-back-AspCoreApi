
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using model_api.Models;
using model_api.Services;
using Stripe;


namespace model_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebHookController : ControllerBase
    {


        private readonly AppSettings _appSettings;
        private readonly CustomerDbContext _customerDbContext;

    public WebHookController(CustomerDbContext customerDbContext, IOptions<AppSettings> appSettings)
    {
            StripeConfiguration.ApiKey = "sk_test_51MJbrPG7Y5eTJXxU70XSO3JaAylPd85hnWDx25p10qcALvNJCROLatvtC1IheR7mrMxMuM8mNFzxEhCGkX5jJIsv009S06xLLi";
            _customerDbContext = customerDbContext;
            _appSettings = appSettings.Value;
        }


        // This is your Stripe CLI webhook secret for testing your endpoint locally.
     //   const string endpointSecret = "whsec_pFjob8Qla6cpaIWOp0JYPFgn9r2UUr6P";
        //  const string endpointSecret = "we_1MMA1bG7Y5eTJXxUQ86uDFZl";


        [HttpPost]
        public async Task<IActionResult> Index()
        {
       
            var json = await new System.IO.StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {

                var stripeEvent = EventUtility.ConstructEvent(json,
                 Request.Headers["Stripe-Signature"], _appSettings.StraipeWebHookKey);




                // Handle the event
                if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

                   var pyament_info = await _customerDbContext.Pyament_infos.FindAsync(Convert.ToInt32( paymentIntent.Metadata["payid"]));
                 //   var pyament_info = await _customerDbContext.Pyament_infos.FindAsync(10);
                    pyament_info.state = stripeEvent.Type;
                    pyament_info.Date = DateTime.Now;


                    _customerDbContext.Pyament_infos.Update(pyament_info);
                    await _customerDbContext.SaveChangesAsync();
                }

          

                return Ok();
            }
            catch (Exception e)
            {
              
                return BadRequest(e);
            }
        }
    }
}
