using FireApp.Domain;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using FireApp.Domain.Extensionmethods;

namespace FireApp.Service.Email
{
    public static class Email
    {
        private static string serviceEmail = ConfigurationManager.AppSettings["serviceEmail"];
        private static string serviceEmailPassword = ConfigurationManager.AppSettings["serviceEmailPassword"];

        private static string emailFolder = ConfigurationManager.AppSettings["serviceBaseAddress"] + "Email/";
        private static string welcomeEmailTemplatePath = emailFolder + "EmailTemplate.html";
        private static string resetEmailTemplatePath = emailFolder + "EmailTemplate.html";
        private static string helpEmailTemplatePath = emailFolder + "EmailTemplate_Help.html";

        private static string passwordResetPath = ConfigurationManager.AppSettings["passwordResetPath"];
        private static string logo = emailFolder + "images/logo_siemens.png";
        private static string icon = emailFolder + "images/Icon3.0.png";
        private static string helpFile = emailFolder + "helpFile.pdf";

        static Email()
        {
#if DEBUG
            emailFolder = "..\\Email".ToFullPath();
            welcomeEmailTemplatePath = "..\\Email\\EmailTemplate.html".ToFullPath();
            resetEmailTemplatePath = "..\\Email\\EmailTemplate.html".ToFullPath();
            helpEmailTemplatePath = "..\\Email\\EmailTemplate_Help.html".ToFullPath();
#endif
        }

        /// <summary>
        /// Sends an email from a service email account to the reciepients.
        /// </summary>
        /// <param name="recipients">The recipients of this email.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="message">The message you want to send.</param>
        public static void Send(string to, string subject, string message) {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(serviceEmail, Encryption.Encrypt.DecryptString(serviceEmailPassword)),
                EnableSsl = true
            };

            var msg = new MailMessage();
            msg.IsBodyHtml = true;
            msg.From = new MailAddress(serviceEmail);
            msg.Subject = subject;
            // todo: use right email address
            msg.To.Add(serviceEmail);
            //msg.To.Add(new MailAddress(to));
            msg.Body = message;

            try
            {
                client.Send(msg);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Sends a welcome email to the user.
        /// </summary>
        /// <param name="u">The User you want to send the email to.</param>
        public static void WelcomeEmail(User u)
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
            try
            {
                using (StreamReader reader = new StreamReader(welcomeEmailTemplatePath))
                {
                    body = reader.ReadToEnd();
                }

                body = body.Replace("{logo}", logo);
                body = body.Replace("{icon}", icon);
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
                body = body.Replace("{help_url2}", helpUrl2);

                Send(u.Email, "FireApp Accout-Erstellung", body);
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// Sends a email to the admin with a question of the user.
        /// </summary>
        /// <param name="user">The user that needs help.</param>
        /// <param name="adminEmail">The email address of the admin that receives the email.</param>
        /// <param name="text">The text the user entered.</param>
        public static void HelpEmail(User user, string adminEmail, string text)
        {
            string header = "Hilfe!";
            string title = text;
            string info = "Gesendet von:";
            info += "<br>Benutzername: " + user.Id;
            info += "<br>Vorname: " + user.FirstName;
            info += "<br>Nachname: " + user.LastName;
            string body = string.Empty;
            try
            {
                using (StreamReader reader = new StreamReader(helpEmailTemplatePath))
                {
                    body = reader.ReadToEnd();
                }

                body = body.Replace("{logo}", logo);
                body = body.Replace("{icon}", icon);
                body = body.Replace("{header}", header);
                body = body.Replace("{title}", title);
                body = body.Replace("{info}", info);

                Send(adminEmail, "User question", body);
            }
            catch (Exception)
            {
                return;
            }
        }
          
        /// <summary>
        /// Sends an email to the User with the User's username and new password.
        /// </summary>
        /// <param name="u">The User you want to send the email to.</param>
        public static void ResetPasswordEmail(User u)
        {
            string header = "Passwort-Zurücksetzung";
            string title = "Guten Tag, " + u.FirstName + " " + u.LastName + "!";
            string info1 = "Das Passwort für Ihren FireApp-Account wurde zurückgesetzt";
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
            try
            {
                using (StreamReader reader = new StreamReader(welcomeEmailTemplatePath))
                {
                    body = reader.ReadToEnd();
                }

                body = body.Replace("{logo}", logo);
                body = body.Replace("{icon}", icon);
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
                body = body.Replace("{help_url2}", helpUrl2);

                Send(u.Email, "FireApp Passwort-Zurücksetzung", body);
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}