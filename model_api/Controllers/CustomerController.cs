using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using model_api.Models;

using System.Security.Cryptography;
using Stripe.Checkout;
using model_api.Services;
using Microsoft.Extensions.Options;

namespace model_api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerDbContext _customerDbContext;
        IEmailService _emailService = null;
        private readonly AppSettings _appSettings;

        public CustomerController (CustomerDbContext customerDbContext, IEmailService emailService, IOptions<AppSettings> appSettings)
        {
            _customerDbContext = customerDbContext;
            _emailService = emailService;
            _appSettings = appSettings.Value;

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetcousmtoerAsync()
        {
            var tokeserver = new Token(_appSettings);
            var id1 = tokeserver.ValidatedToken(HttpContext.Request.Headers["Authorization"]);
            var admin_from_token = await _customerDbContext.Admins.FindAsync(id1);
            if ((admin_from_token == null)) return StatusCode(400);
            if ((admin_from_token.state != "superadmin")) return StatusCode(400);
            return _customerDbContext.Customers;
        }
 

      [HttpGet("{id:int}")]
        public async Task<ActionResult<Customer>> GetbyidAsync(int id)
        {
            var tokeserver = new Token(_appSettings);
            var id1 = tokeserver.ValidatedToken(HttpContext.Request.Headers["Authorization"]);
            var admin_from_token = await _customerDbContext.Admins.FindAsync(id1);
            if ((admin_from_token == null)) return StatusCode(400);
            if ((admin_from_token.state != "superadmin")) return StatusCode(400);
            var customer = await _customerDbContext.Customers.FindAsync(id);
            return customer;
        }




        [HttpPost]
        public async Task<ActionResult<bool>> Creat(Customer customer)
        {
            if (_customerDbContext.Customers.Any(x => x.Email == customer.Email)) return BadRequest();
            // Convert the input string to a byte array and compute the hash.
            Encryption.EncreptpassMD5(customer);
            //genret code to auth generateID();
         
            //make sure stat is unverified until he verfie his email
            customer.state = "unverified";
       
            try
            {
                // save customer
                await _customerDbContext.Customers.AddAsync(customer);
                await _customerDbContext.SaveChangesAsync();
                // save Authcode
                var Authcode = new AuthCode() { code = Encryption.generateID(), ExDate = DateTime.UtcNow.AddHours(2), Customer_ID = customer.Id };
                await _customerDbContext.AuthCodes.AddAsync(Authcode);
                await _customerDbContext.SaveChangesAsync();
                EmailData emailData2 = new EmailData()
                { EmailBody = "pleas clik on the link to verfiy your pass word "+ System.Environment.NewLine + "https://handy-7x8p.onrender.com/#/verifyemail/" + Authcode.code, EmailSubject = "pass verfie", EmailToId = customer.Email, EmailToName = "mm ar" };

                return _emailService.SendEmail(emailData2);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            
        }

    
        [HttpPut]
        public async Task<ActionResult<Customer>> Update(Customer customer)
        {
            try
            {
                var tokeserver = new Token(_appSettings);
                var id = tokeserver.ValidatedToken(HttpContext.Request.Headers["Authorization"]);
                var customer_from_token = await _customerDbContext.Customers.FindAsync(id);
                if ((customer_from_token == null)) return StatusCode(400);

                var customer1 = _customerDbContext.Customers
                         .Where(b => b.Id ==id)
                         .FirstOrDefault();
                if (customer1 == null) return BadRequest();
                customer1.Name = customer.Name;
                customer1.Gender = customer.Gender;
                customer1.BirthDate = customer.BirthDate;

                _customerDbContext.Customers.Update(customer1);
                await _customerDbContext.SaveChangesAsync();
                return Ok();
            }
            catch {
                return BadRequest();

            }
        }


        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Customer>> Delete(int id)
        {
            try
            {
                var tokeserver = new Token(_appSettings);
                var id_from_token = tokeserver.ValidatedToken(HttpContext.Request.Headers["Authorization"]);
                var admin_from_token = await _customerDbContext.Admins.FindAsync(id_from_token);
                if ((admin_from_token == null)) return StatusCode(400);
               // if ((admin_from_token.state != "superadmin")) return StatusCode(400);

                var customer = await _customerDbContext.Customers.FindAsync(id);
                _customerDbContext.Customers.Remove(customer);
                await _customerDbContext.SaveChangesAsync();
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
            }
     

    }
}