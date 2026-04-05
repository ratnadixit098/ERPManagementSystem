using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPManagementSystem.Models
{
    using System.Net;
    using System.Net.Mail;

    public class EmailHelper
    {
        public static void SendEmail(string to, string subject, string body)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(to);
            mail.From = new MailAddress("ratnadixit098@gmail.com");
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential(
                "ratnadixit098@gmail.com",
                "ubvo hygh vojr wqhl\r\n");

            smtp.Send(mail);
        }
    }
}