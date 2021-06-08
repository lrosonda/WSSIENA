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
        public MailService(MailSettings mailSettings)
        {
            _mailSettings = mailSettings;
        }

        public async Task<int> SendEmailAsync(MailRequest mailRequest, string cod)
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
                    if (onEmail != null && onEmail.Length > 0 && !onEmail.Equals("")) {
                        email.Cc.Add(MailboxAddress.Parse(onEmail));
                    }
                    
                }
                foreach (var onEmail in mailRequest.BccEmails)
                {
                    if (onEmail != null && onEmail.Length > 0 && !onEmail.Equals("")) {
                        email.Bcc.Add(MailboxAddress.Parse(onEmail));
                    }
                        
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
                                ms.Close();
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
                    {//TLS
                        if (_mailSettings.SecureSmtp != null && "TLS".Equals(_mailSettings.SecureSmtp))
                        {
                          smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                        }
                        else {
                            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.None);
                        }
                      
                        smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
                      
                        await smtp.SendAsync(email);
                        await smtp.DisconnectAsync(true);
                    }
                    catch (AuthenticationException ex)
                    {
                        log.Error("Message:" + ex.Message);
                        return -1003;
                    }
                    catch (SmtpCommandException ex)
                    {
                        log.Error("Message:[" + ex.Message + "], StatusCode:" + ex.StatusCode); 
                        return processExceptionSMTP( ex);
                    }
                    catch (SmtpProtocolException ex)
                    {
                        log.Error("Message:" + ex.Message);
                        return -1007;
                    }
                }
            }
            catch (ParseException ex)
            {
                log.Error("Message:" + ex.Message);
                return -1010;
            }
            return 0;

        }
       private int processExceptionSMTP(SmtpCommandException ex) {
            switch (ex.ErrorCode)
            {
                case SmtpErrorCode.RecipientNotAccepted:
                  return -1004;
                case SmtpErrorCode.SenderNotAccepted:
                    return -1005;
                case SmtpErrorCode.MessageNotAccepted:
                    return -1006;
                default: return -9999;
            }
        }
    }
}