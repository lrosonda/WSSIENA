using System.Threading.Tasks;

namespace WsNotificacionesISRM.SMTP
{
    interface IMailService
    {
        Task<int> SendEmailAsync(MailRequest mailRequest, string cod);
    }


}
