using System.Net.Mail;
using System.Net;

namespace DiplomAPI.Models.Support
{
    public class MailSupport
    {
        public string StartPoint(string toEmail)
        {
            Random random = new Random();
            string code = random.Next(100, 1000).ToString();

            SendEmail(toEmail, code);

            return code;
        }

        private void SendEmail(string toEmail, string code)
        {
            // Укажите адрес вашей почты и пароль приложения
            var fromAddress = new MailAddress("paul27.utkin@gmail.com", "Павел Уткин");
            var toAddress = new MailAddress(toEmail);
            const string fromPassword = "lddm tren rljd wfum"; // Пароль приложения
            const string smtpHost = "smtp.gmail.com";
            const int smtpPort = 587;

            var smtp = new SmtpClient
            {
                Host = smtpHost,
                Port = smtpPort,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = "Подтверждение операции",
                Body = $"Код подтверждения: {code}. Никому не говорите этот код! Его спрашивают ТОЛЬКО мошенники!"
            })
            {
                smtp.Send(message);
            }
        }
    }
}
