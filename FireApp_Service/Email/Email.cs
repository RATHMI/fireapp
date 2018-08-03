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
        private static string serviceEmail = ConfigurationManager.AppSettings["serviceEmail"];
        private static string serviceEmailPassword = ConfigurationManager.AppSettings["serviceEmailPassword"];

        private static string welcomeEmailTemplatePath = "..\\Email\\EmailTemplate.html".ToFullPath();
        private static string resetEmailTemplatePath = "..\\Email\\EmailTemplate.html".ToFullPath();
        private static string helpEmailTemplatePath = "..\\Email\\EmailTemplate.html".ToFullPath();

        private static string passwordResetPath = ConfigurationManager.AppSettings["passwordResetPath"];
        private static string helpFile = HttpContext.Current.Request.Url.Host + ConfigurationManager.AppSettings["helpFile"];

        // Only for testing.
        static int count = 0;
        //******************

        /// <summary>
        /// Sends an email from a service email account to the reciepients.
        /// </summary>
        /// <param name="recipients">The recipients of this email.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="message">The message you want to send.</param>
        public static void Send(string to, string subject, string message) {

            // Only for testing.
            if(count >= 1)
            {
                return;
            }
            //******************

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(serviceEmail, serviceEmailPassword),
                EnableSsl = true
            };

            var msg = new MailMessage();
            msg.IsBodyHtml = true;
            msg.From = new MailAddress(serviceEmail);
            msg.Subject = subject;
            msg.To.Add(serviceEmail);
            //msg.To.Add(new MailAddress(to));
            msg.Body = message;

            client.Send(msg);
            count++;
        }

        /// <summary>
        /// Sends a welcome email to the user.
        /// </summary>
        /// <param name="u">The User you want to send the email to.</param>
        public static void WelcomeEmail(User u) // todo: improve
        {
            string header = "Schön, dass Sie da sind!";
            string title = "Willkommen " + u.FirstName + " " + u.LastName + "!";
            string info1 = "Ein neuer FireApp-Account wurde für Sie erstellt.";
            string usernameInfo = "Ihr Benutzername lautet: " + u.Id;
            string passwordInfo = "Ihr vorläufiges Passwort lautet: " + u.Password;
            string info2 = "Bitte klicken Sie auf den untenstehenden Button, um ihr vorläufiges Passwort zu ändern.";
            string buttonText = "PASSWORT ÄNDERN";
            string helpTitle = "Sie benötigen Hilfe?";
            string helpButtonText1 = "Email an Serviceteam";
            string helpButtonText2 = "Bedienungsanleitung";
            string helpUrl1 = "mailto:" + serviceEmail;
            string helpUrl2 = helpFile;

            string body = string.Empty;
            using (StreamReader reader = new StreamReader(welcomeEmailTemplatePath))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{header}", header);
            body = body.Replace("{title}", title);
            body = body.Replace("{info}", info1);
            body = body.Replace("{username_info}", usernameInfo);
            body = body.Replace("{password_info}", passwordInfo);
            body = body.Replace("{info2}", info2);
            body = body.Replace("{button}", buttonText);
            body = body.Replace("{url}", passwordResetPath);
            body = body.Replace("{help_title}", helpTitle);
            body = body.Replace("{help_button1}", helpButtonText1);
            body = body.Replace("{help_url1}", helpUrl1);
            body = body.Replace("{help_button2}", helpButtonText2);
            body = body.Replace("{help_url1}", helpUrl2);

            Send(u.Email, "FireApp Accout-Erstellung", body);
        }

        /// <summary>
        /// Sends a email to the admin with a question of the user.
        /// </summary>
        /// <param name="user">The user that needs help.</param>
        /// <param name="adminEmail">The email address of the admin that receives the email.</param>
        /// <param name="text">The text the user entered.</param>
        public static void HelpEmail(User user, string adminEmail, string text)
        {
            string title = "Hilfe!";
            string message = "Der Benutzer \"" + user.FirstName + " " + user.LastName + "\" hat eine Frage für Sie:<br /><br />";
            message += text;
            message += "<br />Benutzer: " + user.Id;
            message += "<br /><br />Mit besten Grüßen,\nIhr FireApp-Server";

            string body = string.Empty;   
            using (StreamReader reader = new StreamReader(helpEmailTemplatePath))
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
            using (StreamReader reader = new StreamReader(resetEmailTemplatePath))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{title}", title);
            body = body.Replace("{message}", message);

            Send(u.Email, "password reset", body);
        }
    }
}