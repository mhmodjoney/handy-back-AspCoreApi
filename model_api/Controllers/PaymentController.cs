using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using model_api.Models;
using model_api.Services;
using Stripe;
using Stripe.Checkout;

namespace model_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly CustomerDbContext _customerDbContext;

        public PaymentController(CustomerDbContext customerDbContext, IOptions<AppSettings> appSettings)
        {
            _customerDbContext = customerDbContext;


            _appSettings = appSettings.Value;

        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetbyidAsync(int id)
        {
            if (id == 1) return Redirect("https://handy-7x8p.onrender.com/#/message?text=Successfully_paid&style=success&next=/payment-history");
            else
                return Redirect("https://handy-7x8p.onrender.com/#/message?text=not_Successfully_paid&style=danger&next=/payment-history");

        }
        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody]Payment_info pyament_Info)
        {
            try
            {
                var tokeserver = new Services.Token(_appSettings);
                var id = tokeserver.ValidatedToken(HttpContext.Request.Headers["Authorization"]);
                var customer = await _customerDbContext.Customers.FindAsync(id);
                if ((customer == null)) return StatusCode(400);


                //SAVE PAYMENTINFO WITH unknown STATE
                //  var pyament_Info = new Pyament_info() { product_id = 1, type = "bill", description = "ddddas", amount = 32,name="dd" };
                pyament_Info.Customer_ID = id;
                pyament_Info.state = "unknown";
                pyament_Info.Date = DateTime.Now;
                pyament_Info.Payment_method = "Stripe/Card";

                try
                {
                    await _customerDbContext.Pyament_infos.AddAsync(pyament_Info);
                    await _customerDbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return StatusCode(409, ex + " *****dd");
                }
                int pay_id = pyament_Info.Id;
                var domain = "http://maraw425-001-site1.atempurl.com/api/Payment";
                var options = new SessionCreateOptions
                {
                    LineItems = new List<SessionLineItemOptions>
            {
              new SessionLineItemOptions
              {
                 PriceData = new SessionLineItemPriceDataOptions
      {
        Currency = "usd",
        UnitAmount =pyament_Info.amount,

        ProductData = new SessionLineItemPriceDataProductDataOptions
        {
          Name = pyament_Info.name
        }
      },
      Quantity =  pyament_Info.Quantity,
              },
            },

                    PaymentIntentData = new SessionPaymentIntentDataOptions
                    {
                        Metadata = new Dictionary<string, string>
    {
        { "user_id", customer.Id.ToString() },{"type",pyament_Info.type},{"description",pyament_Info.description}
                            ,{ "payid",pay_id.ToString() },
    }
                    },

                    Mode = "payment",

                    SuccessUrl = domain +"/1",
                    //+ "/message?text=Successfully paid " + "&style=success&next=/",
                    
                    CancelUrl = domain + "/0",
                    //+ "/message?text=not Successfully paid " + "&style=danger&next=/",
                    //AutomaticTax = new SessionAutomaticTaxOptions { Enabled = true },
                };

                var service = new SessionService();
                Session session = service.Create(options);

                Response.Headers.Add("Location", session.Url);
                return Ok(session.Url);

            }
            catch(Exception e)
            {
                return BadRequest(e);
            }
        }
        






        }
    }
