using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;

using System.IO;
using System.Threading.Tasks;
using MailKit.Security;
using WsNotificacionesISRM.Business;

namespace WsNotificacionesISRM.SMTP
{
    public class MailService : BusinessParent, IMailService
    {
        private readonly MailSettings _mailSettings;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(MailRequest mailRequest, Dictionary<String, Object> dcod)
        {
            var email = new MimeMessage();
            try
            {
                email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
                foreach (var onEmail in mailRequest.ToEmails)
                {
                    email.To.Add(MailboxAddress.Parse(onEmail));
                }
                foreach (var onEmail in mailRequest.CcEmails)
                {
                    email.Cc.Add(MailboxAddress.Parse(onEmail));
                }
                foreach (var onEmail in mailRequest.BccEmails)
                {
                    email.Bcc.Add(MailboxAddress.Parse(onEmail));
                }
                email.Subject = mailRequest.Subject;
                var builder = new BodyBuilder();
                if (mailRequest.Attachments != null)
                {
                    byte[] fileBytes;
                    foreach (var file in mailRequest.Attachments)
                    {
                        if (file.Length > 0)
                        {
                            using (var ms = new MemoryStream())
                            {
                                file.CopyTo(ms);
                                fileBytes = ms.ToArray();
                            }
                            builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                        }
                    }
                }
                builder.HtmlBody = mailRequest.Body;
                email.Body = builder.ToMessageBody();

                using (var smtp = new SmtpClient())
                {
                    try
                    {
                        smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                        if (_mailSettings.Password != null && _mailSettings.Password.Length > 0)
                        {
                            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
                        }
                        await smtp.SendAsync(email);
                        await smtp.DisconnectAsync(true);
                        SaveCorrectMailDelivery(mailRequest, dcod);
                    }
                    catch (AuthenticationException ex)
                    {
                        log.Error("Message:" + ex.Message);
                        SMTPErrorHandling(mailRequest, ex, ex.GetType(), dcod);
                    }
                    catch (SmtpCommandException ex)
                    {
                        log.Error("Message:[" + ex.Message + "], StatusCode:" + ex.StatusCode);
                        SMTPErrorHandling(mailRequest, ex, ex.GetType(), dcod);
                    }
                    catch (SmtpProtocolException ex)
                    {
                        log.Error("Message:" + ex.Message);
                        SMTPErrorHandling(mailRequest, ex, ex.GetType(), dcod);
                    }
                }
            }
            catch (ParseException ex)
            {
                log.Error("Message:" + ex.Message);
                SMTPErrorHandling(mailRequest, ex, ex.GetType(), dcod);
            }


        } 

    }
}