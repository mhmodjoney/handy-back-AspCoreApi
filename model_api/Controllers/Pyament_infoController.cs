using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using model_api;
using model_api.Models;
using model_api.Services;

namespace model_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Pyament_infoController : ControllerBase
    {
        private readonly CustomerDbContext _context;
        private readonly AppSettings _appSettings;


 
        public Pyament_infoController(CustomerDbContext context, IOptions<AppSettings> appSettings, IEmailService emailService)
        {
            _context = context;
            _appSettings = appSettings.Value;
        }

        // GET: api/Pyament_info
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Pyament_info>>> GetPyament_infos()
        //{
        //    return await _context.Pyament_infos.ToListAsync();
        //}

        // GET: api/Pyament_info/5
        [HttpGet]
        public async Task<ActionResult<Payment_info>> GetPyament_info()
        {
            try
            {
                var tokeserver = new Token(_appSettings);
                var id = tokeserver.ValidatedToken(HttpContext.Request.Headers["Authorization"]);

                var pyament_Info = _context.Pyament_infos.Where(x => x.Customer_ID.Equals(id)).ToList();

                if (pyament_Info == null) return StatusCode(400);
                return Ok(pyament_Info);
            }
            catch(Exception e)
            {
                return BadRequest(e);
            }
        }

      

        //// DELETE: api/Pyament_info/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<Payment_info>> DeletePyament_info(int id)
        //{
        //    var pyament_info = await _context.Pyament_infos.FindAsync(id);
        //    if (pyament_info == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Pyament_infos.Remove(pyament_info);
        //    await _context.SaveChangesAsync();

        //    return pyament_info;
        //}

    
    }
}
