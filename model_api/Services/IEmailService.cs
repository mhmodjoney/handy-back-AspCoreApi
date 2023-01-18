using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace model_api.Services
{
   public interface IEmailService
{
    bool SendEmail(EmailData emailData);
}
}
