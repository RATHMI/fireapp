using FireApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace FireApp.Service.Email
{
    public static class EmailSender
    {
        public static void Send(string recipients, string subject, string message) {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("fro.diplomarbeit@gmail.com", "Codebusters!"),
                EnableSsl = true
            };

            // todo: use real email
            // client.Send("fro.diplomarbeit@gmail.com", recipients, subject, message);
            client.Send("fro.diplomarbeit@gmail.com", "ollmann.ph@gmail.com", subject, message);
        }

        public static void WelcomeEmail(User u) // todo: comment, improve
        {
            string message = "Welcome " + u.FirstName + " " + u.LastName + "!\n\n";
            message += "A new FireApp-account has been created for you.\n";
            message += "username: " + u.Id;
            message += "\npassword: " + u.Password;
            message += "\n\nPlease follow this link to change your password: http://example.org/";
            message += "\nWith best regards,\n your FireApp service team";

            Send(u.Email, "welcome", message);
        }
    }
}