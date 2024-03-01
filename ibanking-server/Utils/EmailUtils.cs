using System.Net.Mail;
using System.Net;

namespace ibanking_server.Utils
{
    public class EmailUtils
    {
        public bool SendMail(string to, string body, string subject)
        {
            string emailFrom = "cskhshop5@gmail.com";
            string password = "mlwzoxdlvhjqjmmq";

            try
            {
                // SMTP của Gmail
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(emailFrom, password);

                MailMessage mailMessage = new MailMessage(emailFrom, to, subject, body);
                client.Send(mailMessage);

                return true;
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error sending email: " + ex.Message);
                return false;
            }
        }
    }
}
