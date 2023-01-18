using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using model_api;
using model_api.Models;
using model_api.Services;

namespace model_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminsController : ControllerBase
    {
        private readonly CustomerDbContext _context;
        private readonly AppSettings _appSettings;
   

    
        public AdminsController(CustomerDbContext context, IOptions<AppSettings> appSettings)
        {
            _context = context; 
            _appSettings = appSettings.Value;
        }

        // GET: api/Admins
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Admin>>> GetAdmins()
        {
            var tokeserver = new Token(_appSettings);
            var id = tokeserver.ValidatedToken(HttpContext.Request.Headers["Authorization"]);
            var admin_from_token = await _context.Admins.FindAsync(id);
            if ((admin_from_token == null)) return StatusCode(400);
            if ((admin_from_token.state != "superadmin")) return StatusCode(400);
          
            return await _context.Admins.ToListAsync();
        }

        // GET: api/Admins/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Admin>> GetAdmin(int id)
        {
            try
            {
                var tokeserver = new Token(_appSettings);
                var id1 = tokeserver.ValidatedToken(HttpContext.Request.Headers["Authorization"]);
                var admin_from_token = await _context.Admins.FindAsync(id1);
                if ((admin_from_token == null)) return StatusCode(400);
                if ((admin_from_token.state != "superadmin")) return StatusCode(400);

                var admin = await _context.Admins.FindAsync(id);


                return admin;
            }
            catch (Exception e)
            {

                return BadRequest();
            }

        }

        // PUT: api/Admins/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAdmin(int id, Admin adminparam)
        {
            try
            {
            var tokeserver = new Token(_appSettings);
            var id1 = tokeserver.ValidatedToken(HttpContext.Request.Headers["Authorization"]);
            var admin_from_token = await _context.Admins.FindAsync(id1);
            if ((admin_from_token == null)) return StatusCode(400);
            if ((admin_from_token.state != "superadmin")) return StatusCode(400);
            var admin = await _context.Admins.FindAsync(id);
                if (admin == null) return BadRequest();
                if (admin.Password != adminparam.Password)
                {
                    Encryption.EncreptpassMD5(admin);
                }
                admin.Name = adminparam.Name;
                admin.Gender = adminparam.Gender;
                admin.BirthDate = adminparam.BirthDate;
                admin.Email = adminparam.Email;

                _context.Admins.Update(admin);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest();
            }

        }

        [HttpPost]
        public async Task<ActionResult<bool>> Creat(Admin admin)
        {
            try
            {
                var tokeserver = new Token(_appSettings);
                var id = tokeserver.ValidatedToken(HttpContext.Request.Headers["Authorization"]);
                var superadmin = await _context.Admins.FindAsync(id);
                if ((superadmin.state != "superadmin")) return StatusCode(400);

                if (_context.Admins.Any(x => x.Email == admin.Email)) return BadRequest();

                Encryption.EncreptpassMD5(admin);
               // admin.state = "admin";


                // save admin
                await _context.Admins.AddAsync(admin);
                await _context.SaveChangesAsync();


                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

   

        // DELETE: api/Admins/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Admin>> DeleteAdmin(int id)
        {
            var tokeserver = new Token(_appSettings);
            var id1 = tokeserver.ValidatedToken(HttpContext.Request.Headers["Authorization"]);
            var admin_from_token = await _context.Admins.FindAsync(id1);
            if ((admin_from_token == null)) return StatusCode(400);
         //   if ((admin_from_token.state != "superadmin")) return StatusCode(400);

            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }

            _context.Admins.Remove(admin);
            await _context.SaveChangesAsync();

            return admin;
        }



        [HttpPost("login")]
        public async Task<IActionResult> AuthenticateAsync([FromBody]Customer adminparam)
        {
          
            try
            {
                var admin1 = _context.Admins
                           .Where(b => b.Email == adminparam.Email)
                           .FirstOrDefault();
                if (admin1 == null) return BadRequest();
             
                Encryption.EncreptpassMD5(adminparam);
                // && customer1.state== "verified"
                if (!(adminparam.Password == admin1.Password)) throw new Exception("ddw");

                // authentication successful so generate jwt token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.NameIdentifier, admin1.Id.ToString()),

                    }),
                    Expires = DateTime.UtcNow.AddDays(3),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var Token = tokenHandler.WriteToken(token);

                // remove password before returning
                // customer1.Password = null;
                admin1.Password = Token;
                return Ok(admin1);
            }
            catch (Exception e)
            {

                return BadRequest(e);
            }
        }
        [HttpPost("vladateToken")]
        public async Task<IActionResult> LoginAsync()
        {

            try
            {
                var tokeserver = new Token(_appSettings);
                var id = tokeserver.ValidatedToken(HttpContext.Request.Headers["Authorization"]);
                var admin = await _context.Admins.FindAsync(id);
                if ((admin == null)) return StatusCode(400);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(400);
            }
        }




        private bool AdminExists(int id)
        {
            return _context.Admins.Any(e => e.Id == id);
        }
    }
}
