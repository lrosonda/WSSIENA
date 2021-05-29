using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WsNotificacionesISRM.SMTP
{
    interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest, Dictionary<String, Object> dcod);
    }

   
}
