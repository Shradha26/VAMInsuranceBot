using System.Net.Mail;

namespace VAMInsuranceBot.Process
{
    public class Email
    {
        private static string sender = "coolguy.sourav.sharma@gmail.com";
        private static string password = "Qwerty!@34";
        private static string subject = "One Time Password for Authentication with Bot";

        public static string recipient;
        public static string message;

        public static void SendEmail()
        {
            SmtpClient client = new SmtpClient("smtp.gmail.com");

            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(sender, password);
            client.EnableSsl = true;
            client.Credentials = credentials;

            var mail = new MailMessage(sender.Trim(), recipient.Trim());
            mail.Subject = subject;
            mail.Body = message;
            client.Send(mail);
        }
    }
}