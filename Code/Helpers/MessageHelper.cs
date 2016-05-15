using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public class MessageHelper
    {
        public static string ConfigureExceptionMessage(Exception ex)
        {
            var st = new StackTrace(ex, true);
            var frame = st.GetFrame(0);
            var line = frame.GetFileLineNumber();

            return $"{{\"errorMessage\":\"{ex.Message.Replace("\"", "")}\"}}";
        }

        public static void SendMailSmtp(string subject, string body, bool isBodyHtml, IEnumerable<string> mailTo, IEnumerable<string> hiddenMailTo = null, AttachmentFile file = null)
        {
            var recipients = new List<MailAddress>();
            if (mailTo != null)
            {
                foreach (var email in mailTo)
                {
                    if (string.IsNullOrEmpty(email)) continue;
                    recipients.Add(new MailAddress(email));
                }
            }

            var recHidden = new List<MailAddress>();
            if (hiddenMailTo != null)
            {
                foreach (var email in hiddenMailTo)
                {
                    if (string.IsNullOrEmpty(email)) continue;
                    recHidden.Add(new MailAddress(email));
                }
            }

            Task.Run(() => { SendMailSmtp(subject, body, isBodyHtml, recipients.ToArray(), recHidden.ToArray(), file); });

        }

        public static void SendMailSmtp(string subject, string body, bool isBodyHtml, string mailTo, string hiddenMailTo = null, AttachmentFile file = null)
        {
            SendMailSmtp(subject, body, isBodyHtml, new[] { mailTo }, new[] { hiddenMailTo }, file);
        }

        public static void SendMailSmtp(string subject, string body, bool isBodyHtml, IEnumerable<MailAddress> mailTo, IEnumerable<MailAddress> hiddenMailTo = null, AttachmentFile file = null)
        {
            Task.Run(() => { SendMailSmtp(subject, body, isBodyHtml, mailTo.ToArray(), hiddenMailTo.ToArray(), file, false); });
        }

        public static void SendMailSmtp(string subject, string body, bool isBodyHtml, MailAddress[] mailTo, MailAddress[] hiddenMailTo = null, AttachmentFile file = null)
        {
            Task.Run(() => { SendMailSmtp(subject, body, isBodyHtml, mailTo.ToArray(), hiddenMailTo?.ToArray(), file, false); });
        }

        public static void SendMailSmtp(string subject, string body, bool isBodyHtml, MailAddress[] mailTo, MailAddress[] hiddenMailTo, AttachmentFile file, bool isTest)
        {
            if (!mailTo.Any() && (hiddenMailTo == null || !hiddenMailTo.Any())) throw new Exception("Не указаны получатели письма!");

            var mail = new MailMessage();

            string host = ConfigurationManager.AppSettings["SmtpHost"];
            int port = String.IsNullOrEmpty(ConfigurationManager.AppSettings["Host"]) ? 587 :Convert.ToInt32(ConfigurationManager.AppSettings["Host"]);
            string login = ConfigurationManager.AppSettings["SmtpLogin"];
            string pass = ConfigurationManager.AppSettings["SmtpPass"];
            string mailFrom = ConfigurationManager.AppSettings["SmtpMailFrom"];

            var client = new SmtpClient(host, port);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(login, pass);

            mail.From = new MailAddress(mailFrom, string.Empty, System.Text.Encoding.UTF8);

            //client.EnableSsl = false;

            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["Environment"]) && !isTest)
            {
                if (mailTo != null)
                {
                    foreach (var mailAddress in mailTo)
                    {
                        if (string.IsNullOrEmpty(mailAddress.Address)) continue;
                        mail.To.Add(mailAddress);
                    }
                }
                if (hiddenMailTo != null)
                {
                    foreach (var mailAddress in hiddenMailTo)
                    {
                        if (string.IsNullOrEmpty(mailAddress.Address)) continue;
                        mail.CC.Add(mailAddress);
                    }
                }
            }
            else
            {
                var testMails = ConfigurationManager.AppSettings["Emails4Test"]?.Split('|');
                foreach (var email in testMails)
                {
                    if (string.IsNullOrEmpty(email)) continue;
                    mail.To.Add(email);
                }

                body += "\r\n";
                if (mailTo != null)
                {
                    foreach (var mailAddress in mailTo)
                    {
                        body += "\r\n" + mailAddress.Address;
                    }
                }
                //Hidden recipients
                if (hiddenMailTo != null)
                {
                    foreach (var mailAddress in hiddenMailTo)
                    {
                        body += "\r\n" + mailAddress.Address;
                    }
                }
            }

            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = isBodyHtml;
            //client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
            //client.SendCompleted += (s, e) => {
            //    SendCompletedCallback(s, e);
            //    client.Dispose();
            //    mail.Dispose();
            //};

            if (file != null && file.Data.Length > 0)
            {
                var stream = new MemoryStream(file.Data);
                var attachment = new Attachment(stream, file.FileName, file.DataMimeType);
                mail.Attachments.Add(attachment);
            }

            try
            {
                client.Send(mail);
            }
            catch (Exception ex)
            {
                throw new Exception($"Сообщение не было отправлено. Текст ошибки - {ex.Message}");
            }
        }

        private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the message we sent
            var msg = (MailMessage)e.UserState;

            if (e.Cancelled)
            {
                // prompt user with "send cancelled" message 
            }
            if (e.Error != null)
            {
                // prompt user with error message 
            }
            else
            {
                // prompt user with message sent!
                // as we have the message object we can also display who the message
                // was sent to etc 
            }

            // finally dispose of the message
            if (msg != null)
                msg.Dispose();
        }
    }

    
}
