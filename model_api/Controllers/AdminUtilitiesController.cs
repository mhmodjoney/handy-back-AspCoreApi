using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using model_api.Models;
using model_api.Services;

namespace model_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminUtilitiesController : ControllerBase
    {
        private readonly CustomerDbContext _Context;
        IEmailService _emailService = null;
        private readonly AppSettings _appSettings;

        public AdminUtilitiesController(CustomerDbContext customerDbContext, IEmailService emailService, IOptions<AppSettings> appSettings)
        {
            _Context = customerDbContext;
            _emailService = emailService;
            _appSettings = appSettings.Value;

        }
        [HttpPut("customer")]
        public async Task<ActionResult<Customer>> Update(Customer customer)
        {
            try
            {
                var tokeserver = new Token(_appSettings);
                var id1 = tokeserver.ValidatedToken(HttpContext.Request.Headers["Authorization"]);
                var admin_from_token = await _Context.Admins.FindAsync(id1);
                if ((admin_from_token == null)) return StatusCode(400);
              //  if ((admin_from_token.state != "superadmin")) return StatusCode(400);

                var customer1 = _Context.Customers
                         .Where(b => b.Email == customer.Email)
                         .FirstOrDefault();
                if (customer1 == null) return BadRequest();
                customer1.Name = customer.Name;
                customer1.Gender = customer.Gender;
                customer1.BirthDate = customer.BirthDate;

                _Context.Customers.Update(customer1);
                await _Context.SaveChangesAsync();
                return Ok();
            }
            catch
            {
                return BadRequest();

            }
        }

        [HttpGet("payment_info")]
        public async Task<ActionResult<IEnumerable<Payment_info>>> GetPyament_info()
        {
            try
            {
                var tokeserver = new Token(_appSettings);
                var id1 = tokeserver.ValidatedToken(HttpContext.Request.Headers["Authorization"]);
                var admin_from_token = await _Context.Admins.FindAsync(id1);
                if ((admin_from_token == null)) return StatusCode(400);


                return _Context.Pyament_infos.Include(p => p.Customer).ToList();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}