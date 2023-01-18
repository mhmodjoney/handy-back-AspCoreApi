using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using model_api.Services;

namespace model_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        IEmailService _emailService = null;
        public ValuesController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost]
        public bool SendEmail(EmailData emailData)
        {
            EmailData emailData2 = new EmailData()
            { EmailBody="sbd",EmailSubject="dd",EmailToId= "amm337342@gmail.com", EmailToName= "mm ar" };
         
            return _emailService.SendEmail(emailData2);
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<HttpResponseMessage> Get(int id)
        {
            return Redirect("https://www.youtube.com/watch?v=98gCJostAcI&list=RDve7BwXacVr0&index=20");
        }

        // POST api/values
   

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
