using FireApp.Domain;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace FireApp.Service.Email
{
    public static class Email
    {
        private static string templatePath = "..\\Email\\EmailTemplate.html".ToFullPath();
        private static string passwordResetPath = ConfigurationManager.AppSettings["passwordResetPath"];


        /// <summary>
        /// Sends an email from a service email account to the reciepients.
        /// </summary>
        /// <param name="recipients">The recipients of this email.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="message">The message you want to send.</param>
        public static void Send(string recipients, string subject, string message) {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("fro.diplomarbeit@gmail.com", "Codebusters!"),
                EnableSsl = true
            };

            var msg = new MailMessage();
            msg.IsBodyHtml = true;
            msg.From = new MailAddress("fro.diplomarbeit@gmail.com");
            msg.Subject = subject;
            msg.To.Add("fro.diplomarbeit@gmail.com");
            //msg.To.Add(recipients);
            msg.Body = message;

            client.Send(msg);
            // todo: use real email
            // client.Send("fro.diplomarbeit@gmail.com", recipients, subject, message);
            //client.Send("fro.diplomarbeit@gmail.com", "fro.diplomarbeit@gmail.com", subject, message);
        }

        /// <summary>
        /// Sends a welcome email to the user.
        /// </summary>
        /// <param name="u">The User you want to send the email to.</param>
        public static void WelcomeEmail(User u) // todo: improve
        {
            string title = "Welcome " + u.FirstName + " " + u.LastName + "!";
            string message = "A new FireApp-account has been created for you.<br />";
            message += "username: " + u.Id;
            message += "<br />password: " + u.Password;
            message += "<br /><br />Please follow this link to change your password: ";
            message += passwordResetPath;
            message += "<br /><br />With best regards, <br />your FireApp service team";

            string body = string.Empty;
            using (StreamReader reader = new StreamReader(templatePath))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{title}", title);
            body = body.Replace("{message}", message);

            Send(u.Email, "welcome", body);
        }

        /// <summary>
        /// Sends a email to the admin with a question of the user.
        /// </summary>
        /// <param name="user">The user that needs help.</param>
        /// <param name="adminEmail">The email address of the admin that receives the email.</param>
        /// <param name="text">The text the user entered.</param>
        public static void HelpEmail(User user, string adminEmail, string text)
        {
            string title = "Help!";
            string message = "The User \"" + user.FirstName + " " + user.LastName + "\" has a question for you:<br /><br />";
            message += text;
            message += "<br /><br />username: " + user.Id;
            message += "<br /><br />With best regards,\nyour FireApp server";

            string body = string.Empty;   
            using (StreamReader reader = new StreamReader(templatePath))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{title}", title);
            body = body.Replace("{message}", message);

            Send(adminEmail, "User question", body);
        }

        /// <summary>
        /// Sends an email to the User with the User's username and new password.
        /// </summary>
        /// <param name="u">The User you want to send the email to.</param>
        public static void ResetEmail(User u)// todo: improve
        {
            string title = "Welcome " + u.FirstName + " " + u.LastName + "!";
            string message = "You forgot your password so we generated you a new one.<br />";
            message += "username: " + u.Id;
            message += "<br />password: " + u.Password;
            message += "<br /><br />Please follow this link to change your password: ";
            message += passwordResetPath;
            message += "<br /><br />With best regards,\nyour FireApp service team";

            string body = string.Empty;
            using (StreamReader reader = new StreamReader(templatePath))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{title}", title);
            body = body.Replace("{message}", message);

            Send(u.Email, "password reset", body);
        }
    }
}