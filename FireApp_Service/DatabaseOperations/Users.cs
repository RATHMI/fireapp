using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;
using MlkPwgen;
using System.Text.RegularExpressions;
using static FireApp.Domain.User;
using FireApp.Domain.Extensionmethods;

namespace FireApp.Service.DatabaseOperations
{
    public static class Users
    {
        /// <summary>
        /// Inserts a User into the database or updates it if it already exists.
        /// </summary>
        /// <param name="user">The User you want to upsert.</param>
        /// <returns>Returns true if the User was inserted.</returns>
        public static bool Upsert(User user, User admin)
        {
            try
            {
                if (user != null && user.Id != null)
                {
                    // Try to find an existing User.
                    User old = GetById(user.Id);

                    if(user.Email == null)
                    {
                        if(old != null && (old.Email != null && old.Email != ""))
                        {
                            user.Email = old.Email;
                        }
                        else
                        {
                            // User should not be upserted without an email address.
                            return false;
                        }
                    }

                    // Make sure the email address is unique.
                    if (GetByEmail(user.Email) != null && GetByEmail(user.Email).Id != old.Id)
                    {
                        return false;
                    }

                    if (user.Token == null)
                    {
                        if (old == null)
                        {
                            // If there is no existing User you have to generate a new token
                            // to guarantee a safe authentication.
                            user.Token = Authentication.Token.GenerateToken();
                        }
                        else
                        {
                            // If there is an existing user, copy the token so the User does not have to login again.
                            user.Token = old.Token;
                        }
                    }

                    if(user.Password == null || user.Password == "")
                    {
                        if (old == null)
                        {
                            // Generate a new password.
                            user.Password = PasswordGenerator.Generate(10, Sets.Alphanumerics);
                        }
                        else
                        {
                            // Decrypt the old password to avoid encrypting the password twice.
                            user.Password = Encryption.Encrypt.DecryptString(old.Password);
                        }                                               
                    }

                    if (old == null)
                    {
                        Email.Email.WelcomeEmail(user);
                    }
                    int passwordOk = DatabaseOperations.Users.CheckPassword(user);

                    #if DEBUG
                    passwordOk = 1;
                    #endif

                    if (passwordOk == 1)
                    {

                        // Encrypt password.
                        user.Password = Encryption.Encrypt.EncryptString(user.Password);

                        // Save the User in the database.
                        bool ok = DatabaseOperations.DbUpserts.UpsertUser(user);
                        if (ok)
                        {                           
                            Logging.Logger.Log("upsert", admin.GetUserDescription(), user);
                        }
                        return ok;
                    }
                }
                return false;
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Inserts a list of Users into the database or updates them if they already exist.
        /// </summary>
        /// <param name="users">A list of Users you want to upsert.</param>
        /// <returns>Returns the number of Users that were successfully upserted.</returns>
        public static int BulkUpsert(IEnumerable<User> users, User admin)
        {
            int upserted = 0;
            if (users != null)
            {
                foreach (User user in users)
                {
                    if (Upsert(user, admin) == true)
                    {
                        upserted++;
                    }
                }
            }
            return upserted;
        }

        /// <summary>
        /// Deletes a User from all databases.
        /// </summary>
        /// <param name="userName">The Id of the User you want to delete.</param>
        /// <returns>Returns true if the User was deleted.</returns>
        public static bool Delete(string userName, User user)
        {
            var ok = DatabaseOperations.DbDeletes.DeleteUser(userName);
            if (ok)
            {
                try
                {
                    User old = GetById(userName);
                    Logging.Logger.Log("delete", user.GetUserDescription(), old);                   
                }
                catch (Exception)
                {
                    // Could not be found in Cache.
                }
            }

            return ok;
        }

        /// <summary>
        /// Checks if an id is already used by another User.
        /// </summary>
        /// <param name="id">The id you want to check.</param>
        /// <returns>Returns true if the id is not used by another User.</returns>
        public static bool CheckId(string id)
        {
            IEnumerable<User> all = GetAll();
            foreach (User user in all)
            {
                if (user.Id == id)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Checks if a password is valid.
        /// </summary>
        /// <param name="user">The User whose password is to be checked.</param>
        /// <returns>
        /// Returns an error code.
        /// 1 : password is valid.
        /// 0 : password does not fit the criteria.
        /// -1 : password is too easy.
        /// -2 : an error occured.
        /// </returns>
        public static int CheckPassword(User user)
        {
            int rv = 1;
            try
            {
                // Check if the password is long enough.
                switch (user.UserType)
                {
                    case UserTypes.admin: if (user.Password.Length < 10) { rv = 0; } break;
                    default: if (user.Password.Length < 6) { rv = 0; } break;
                }

                // Check if the password contains upper and lower case letters and numbers.
                if (
                    !Regex.IsMatch(user.Password, "[A-Z]") ||
                    !Regex.IsMatch(user.Password, "[a-z]") ||
                    !Regex.IsMatch(user.Password, "[0-9]"))
                {
                    rv = 0;
                }

                // Check for the most common passwords in Austria.
                List<string> common = new List<string>();
                common.AddRange
                    (
                        new string[] {
                        "hallo",
                        "passwort",
                        "1234",
                        "5678",
                        "0000",
                        "qwertz",
                        "test",
                        "schatz",
                        "blink182",
                        user.FirstName,
                        user.LastName,
                        user.Id
                        }
                    );

                foreach (string s in common)
                {
                    if (user.Password.ToLower().Contains(s))
                    {
                        rv = -1;
                        break;
                    }
                }
            }
            catch (Exception)
            {
                rv = -2;
            }

            return rv;
        }

        /// <summary>
        /// Returns all Users.
        /// </summary>
        /// <returns>Returns a list of all Users.</returns>
        public static IEnumerable<User> GetAll()
        {
            return DatabaseOperations.DbQueries.QueryUsers();
        }

        /// <summary>
        /// Returns all Users that have a UserType that is matching with a UserType from "usertypes".
        /// </summary>
        /// <param name="usertypes">An array of UserTypes.</param>
        /// <returns>Returns a list of all users with matching UserTypes.</returns>
        public static IEnumerable<User> GetByUserType(UserTypes[] usertypes)
        {
            List<User> results = new List<User>();
            foreach(UserTypes type in usertypes)
            {
                results.AddRange(GetByUserType(type));
            }

            return results;
        }

        /// <summary>
        /// Returns all Users that have a UserType that is matching with "usertype".
        /// </summary>
        /// <param name="usertype">The UserType you want to filter.</param>
        /// <returns>Returns all Users with a UserType matching "usertype".</returns>
        public static IEnumerable<User> GetByUserType(UserTypes usertype)
        {
            List<User> results = new List<User>();
            foreach (User user in GetAll())
            {
                if (usertype == user.UserType)
                {
                    results.Add(user);
                }
            }

            return results;
        }

        /// <summary>
        /// Returns the first admin that is found in the database.
        /// </summary>
        /// <returns>Returns a User with Usertype admin.</returns>
        public static User GetFirstAdmin()
        {
            IEnumerable<User> admins = GetByUserType(UserTypes.admin);
            if(admins.Count() > 0)
            {
                return admins.First();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the User with a matching Username.
        /// </summary>
        /// <param name="username">The Username of the User you are looking for.</param>
        /// <returns>Returns a User with a matching Username.</returns>
        public static User GetById(string userName)
        {
            IEnumerable<User> users = GetAll();
            foreach (User u in users)
            {
                if (u.Id == userName)
                {
                    return u;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the User with a matching email address.
        /// </summary>
        /// <param name="email">The email address of the User.</param>
        /// <returns>Returns the User with a matching email address.</returns>
        public static User GetByEmail(string email)
        {
            IEnumerable<User> users = GetAll();
            foreach (User u in users)
            {
                if (u.Email == email)
                {
                    return u;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns all active Users.
        /// </summary>
        /// <returns>Returns a List of Users with a valid token.</returns>
        public static IEnumerable<User> GetActiveUsers()
        {
            List<User> results = new List<User>();
            foreach(User user in GetAll())
            {
                if (DateTime.Now < user.TokenCreationDate.AddHours(AppData.TokenValidHours(user.UserType)))
                {
                    results.Add(user);
                }
            }

            return results;
        }

        /// <summary>
        /// Returns all inactive Users.
        /// </summary>
        /// <returns>Returns a List of Users with an invalid token.</returns>
        public static IEnumerable<User> GetInactiveUsers()
        {
            List<User> results = new List<User>();
            foreach (User user in GetAll())
            {
                if (DateTime.Now >= user.TokenCreationDate.AddHours(AppData.TokenValidHours(user.UserType)))
                {
                    results.Add(user);
                }
            }

            return results;
        }

        /// <summary>
        /// Finds all authorized objects of the User depending on the UserType.
        /// </summary>
        /// <param name="user">The User you want the authorized objects from.</param>
        /// <returns>Returns a list of all authorized objects.</returns>
        public static IEnumerable<object> GetAuthorizedObjects(User user)
        {
            List<object> results = new List<object>();
            if (user != null)
            {
                // If the UserType is firealarmsystem.
                if (user.UserType == UserTypes.fireSafetyEngineer)
                {
                    // Get all FireAlarmSystems that are contained in AuthorizedObjectIds.
                    foreach (int id in user.AuthorizedObjectIds)
                    {
                        try
                        {
                            results.Add(DatabaseOperations.FireAlarmSystems.GetById(id));
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                }

                // If the UserType is firebrigade.
                if (user.UserType == UserTypes.fireFighter)
                {
                    // Get all FireBrigades that are contained in AuthorizedObjectIds.
                    foreach (int id in user.AuthorizedObjectIds)
                    {
                        try
                        {
                            results.Add(DatabaseOperations.FireBrigades.GetById(id));
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                }

                // If the UserType is servicemember
                if (user.UserType == UserTypes.servicemember)
                {
                    // Get all ServiceGroups that are contained in AuthorizedObjectIds.
                    foreach (int id in user.AuthorizedObjectIds)
                    {
                        try
                        {
                            results.Add(DatabaseOperations.ServiceGroups.GetById(id));
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                }          
            }

            return results;
        }

        /// <summary>
        /// Returns all Users that contain the "id" in their AuthorizedObjectIds.
        /// </summary>
        /// <param name="id">The id of the authorized object.</param>
        /// <param name="type">The UserType of the User.</param>
        /// <returns>Returns a list of all Users where their AuthorizedObjectIds contain "id"</returns>
        public static IEnumerable<User> GetByAuthorizedObject(int id, UserTypes type)
        {
            // Get all Users with a UserType matching "type".
            IEnumerable<User> users = GetByUserType(type);

            List<User> results = new List<User>();

            // Get all Users whose AuthorizedObjectIds contains "id".
            foreach (User user in users)
            {
                if (user.AuthorizedObjectIds.Contains(id))
                {
                    results.Add(user);
                }
            }

            return results;
        }

    }
}