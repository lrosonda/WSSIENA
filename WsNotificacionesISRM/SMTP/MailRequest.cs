using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WsNotificacionesISRM.SMTP
{
    public class MailRequest
    {
        private List<string> toEmails;
        private List<string> ccEmails;
        private List<string> bccEmails;
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<IFormFile> Attachments { get; set; }
        public List<string> ToEmails { get => toEmails; set => toEmails = value; }
        public List<string> CcEmails { get => ccEmails; set => ccEmails = value; }
        public List<string> BccEmails { get => bccEmails; set => bccEmails = value; }
    }
}