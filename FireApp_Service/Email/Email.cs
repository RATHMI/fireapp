using FireApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace FireApp.Service.Email
{
    public static class Email
    {
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

            // todo: use real email
            // client.Send("fro.diplomarbeit@gmail.com", recipients, subject, message);
            //client.Send("fro.diplomarbeit@gmail.com", "fro.diplomarbeit@gmail.com", "Schöne Ferien!", "Schöne Ferien Neini!\n\nWünschen dir deine Kameraden aus Gmunden ;)");
            client.Send("fro.diplomarbeit@gmail.com", "fro.diplomarbeit@gmail.com", "Schöne Ferien!", message);
        }

        /// <summary>
        /// Sends a welcome email to the user.
        /// </summary>
        /// <param name="u">The User you want to send the email to.</param>
        public static void WelcomeEmail(User u) // todo: comment, improve
        {
            string message = "Welcome " + u.FirstName + " " + u.LastName + "!\n\n";
            message += "A new FireApp-account has been created for you.\n";
            message += "username: " + u.Id;
            message += "\npassword: " + u.Password;
            message += "\n\nPlease follow this link to change your password: http://example.org/";
            message += "\nWith best regards,\nyour FireApp service team";

            Send(u.Email, "welcome", message);
        }

        /// <summary>
        /// Sends a email to the admin with a question of the user.
        /// </summary>
        /// <param name="user">The user that needs help.</param>
        /// <param name="adminEmail">The email address of the admin that receives the email.</param>
        /// <param name="text">The text the user entered.</param>
        public static void HelpEmail(User user, string adminEmail, string text)
        {
            string message = "The User \"" + user.FirstName + " " + user.LastName + "\" has a question for you:\n\n";
            message += text;
            message += "\n\nusername: " + user.Id;
            message += "\n\nWith best regards,\nyour FireApp server";

            Send(adminEmail, "User question", message);
        }

        public static void ResetEmail(User u)
        {
            string message = "Welcome " + u.FirstName + " " + u.LastName + "!\n\n";
            message += "You forgot your password so we generated you a new one.\n";
            message += "username: " + u.Id;
            message += "\npassword: " + Encryption.Encrypt.DecryptString(u.Password);
            message += "\n\nPlease follow this link to change your password: http://example.org/";
            message += "\nWith best regards,\nyour FireApp service team";

            Send(u.Email, "password reset", message);
        }
    }
}