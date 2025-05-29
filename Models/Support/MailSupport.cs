using System.Net.Mail;
using System.Net;

namespace DiplomAPI.Models.Support
{
    public class MailSupport
    {
        private readonly MailAddress _address;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _appKey;

        private IConfiguration _configuration;

        public MailSupport(IConfiguration configuration)
        {
            _configuration = configuration;

            _address = new MailAddress(_configuration["Mail:sendersEmail"], _configuration["Mail:sendersName"]);
            _smtpHost = _configuration["Mail:smtpHost"];
            _smtpPort = int.Parse(_configuration["Mail:smtpPort"]);

            _appKey = _configuration["Mail:appKey"];
        }

        public string StartPoint(string toEmail)
        {
            Random random = new Random();
            string code = random.Next(100, 1000).ToString();

            SendEmail(toEmail, "Подтверждение операции", $"Код подтверждения: {code}.\r\n\r\n" +
                                $"Никому не говорите этот код! Его спрашивают ТОЛЬКО мошенники!");

            return code;
        }

        public void SendConfirmation(string toEmail, string password)
        {
            string subject = "Данные для входа в «Finance manager»";
            string body = $"Добрый день!\r\n\r\n" +
                          "Ваша заявка на регистрацию в «Finance manager» одобрена.\r\n\r\n" +
                          $"Логин: {toEmail}\r\n\r\n" +
                          $"Пароль: {password}";

            SendEmail(toEmail, subject, body);
        }

        public void BrokerSelfRegistration(string toEmail)
        {
            string subject = "Регистрация в «Finance manager»";
            string body = $"Добрый день!\r\n\r\n" +
                          "Ваша заявка на регистрацию в «Finance manager» принята на рассмотрение";

            SendEmail(toEmail, subject, body);
        }

        private void SendEmail(string toEmail, string subject, string body)
        {
            var toAddress = new MailAddress(toEmail);

            using (var smtp = new SmtpClient
            {
                Host = _smtpHost,
                Port = _smtpPort,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_address.Address, _appKey)
            })
            {
                using (var message = new MailMessage(_address, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
            }
        }
    }
}
