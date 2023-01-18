using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using model_api.Models;
using model_api.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace model_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly CustomerDbContext _customerDbContext;
        IEmailService _emailService = null;

        public AuthController(CustomerDbContext customerDbContext, IOptions<AppSettings> appSettings, IEmailService emailService)
        {
            _customerDbContext = customerDbContext;
            _emailService = emailService;
            _appSettings = appSettings.Value;

        }

        //[HttpPost]
        //public ActionResult<Customer> LoginAsync(Customer customer)
        //{
        //    if (!_customerDbContext.Customers.Any(x => x.Email == customer.Email)) return BadRequest("--");

        //    var customer1 = _customerDbContext.Customers
        //               .Where(b => b.Email == customer.Email)
        //               .FirstOrDefault();

        //    if (!(customer.Password == customer1.Password)) return BadRequest("--");
        //    return Ok(customer1.Id);
        //}



        [HttpPost("login")]
        public async Task<IActionResult> AuthenticateAsync([FromBody]Customer customerParam)
        {
            var Customerlog = new CustomerLogs() { Date = DateTime.UtcNow , state = "failed"};
            try
            {
                var customer1 = _customerDbContext.Customers
                           .Where(b => b.Email == customerParam.Email)
                           .FirstOrDefault();
                if (customer1== null) return BadRequest();
                Customerlog.Customer_ID = customer1.Id;
                Encryption.EncreptpassMD5(customerParam);
                // && customer1.state== "verified"
                if (!(customerParam.Password == customer1.Password)) throw new Exception("ddw");

                // authentication successful so generate jwt token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.NameIdentifier, customer1.Id.ToString()),

                    }),
                    Expires = DateTime.UtcNow.AddDays(3),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var Token = tokenHandler.WriteToken(token);

                // remove password before returning
                // customer1.Password = null;
                var str = "{\"token\":\"" + Token + "\"}";
                dynamic json = JsonConvert.DeserializeObject(str);
                Customerlog.state = "sucsses";
                await _customerDbContext.Customerlogs.AddAsync(Customerlog);
                await _customerDbContext.SaveChangesAsync();
                customer1.Password = Token;
                return Ok(customer1);
            }
            catch (Exception e)
            {
        
                await _customerDbContext.Customerlogs.AddAsync(Customerlog);
                await _customerDbContext.SaveChangesAsync();
            
                return BadRequest();
            }
        }

        [HttpPost("vladateToken")]
        public async Task<IActionResult> LoginAsync([FromBody]Customer userParam)
        {

            try
            {
                var tokeserver = new Token(_appSettings);
                var id = tokeserver.ValidatedToken(HttpContext.Request.Headers["Authorization"]);
                var customer = await _customerDbContext.Customers.FindAsync(id);
                if ((customer == null)) return StatusCode(400);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(400);
            }
        }


    

        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetbyidAsync(string id)
        {
            var authCode = _customerDbContext.AuthCodes.Where(x => x.code == id && x.ExDate> DateTime.UtcNow).FirstOrDefault();
            if (authCode == null) return BadRequest("code expaired");
            var customer = await _customerDbContext.Customers.FindAsync(authCode.Customer_ID);
            if (customer == null) return BadRequest("ni expaired");
            customer.state = "verified";
            _customerDbContext.Customers.Update(customer);
            await _customerDbContext.SaveChangesAsync();
            return Ok();

          
        }

        [HttpGet("pass/{id}")]
        public async Task<ActionResult<Customer>> GetbyidAsync2(string id)
        {
            var authCode = _customerDbContext.AuthCodes.Where(x => x.code == id && x.ExDate > DateTime.UtcNow).FirstOrDefault();
            if (authCode == null) return BadRequest("code expaired");
            var customer = await _customerDbContext.Customers.FindAsync(authCode.Customer_ID);
            if (customer == null) return BadRequest("ni expaired");
            return Ok();

        }

        [HttpPost("forgotPass")]
        public async Task<ActionResult<bool>> forgotPass([FromBody]Customer customer)
        {

            try
            {
                var customer1 = _customerDbContext.Customers
                      .Where(b => b.Email == customer.Email)
                      .FirstOrDefault();
                if (customer1 == null) return BadRequest();
                var Authcode = new AuthCode() { code = Encryption.generateID(), ExDate = DateTime.UtcNow.AddHours(2), Customer_ID = customer1.Id };
                await _customerDbContext.AuthCodes.AddAsync(Authcode);
                await _customerDbContext.SaveChangesAsync();
                EmailData emailData2 = new EmailData()
                { EmailBody = "pleas clik on the link to crert new your pass word /n" + "https://handy-7x8p.onrender.com/#/forgotpassword/" + Authcode.code, EmailSubject = "new pass verfie", EmailToId = customer.Email, EmailToName = customer.Name };

                return _emailService.SendEmail(emailData2);
            }
            catch (Exception e)
            {
                return StatusCode(400);
            }
        }

        [HttpPost("UpdatePass/{code}")]
        public async Task<ActionResult<bool>> UpdatePass(string code , [FromBody] Customer customerparam)
        {

            try
            {
                var authCode = _customerDbContext.AuthCodes.Where(x => x.code == code && x.ExDate > DateTime.UtcNow).FirstOrDefault();
                if (authCode == null) return BadRequest("code expaired");
                var customer = await _customerDbContext.Customers.FindAsync(authCode.Customer_ID);
                if (customer == null) return BadRequest("ni expaired");
                customer.Password = customerparam.Password;
                Encryption.EncreptpassMD5(customer);
                 _customerDbContext.Customers.Update(customer);
                await _customerDbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(400);
            }
        }




    }
}