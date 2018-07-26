using FireApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FireApp.Service.DatabaseOperations.BasicOperations
{
    public class Users
    {
        /// <summary>
        /// Inserts a User into the database or updates it if it already exists.
        /// </summary>
        /// <param name="user">The User you want to upsert.</param>
        /// <returns>Returns true if the User was inserted.</returns>
        public static bool Upsert(User user)
        {
            try
            {
                if (user != null)
                {
                    // Try to find an existing User.
                    User old = GetById(user.Id);
                    if (user.Email == null)
                    {
                        return false;
                    }

                    if (old == null)
                    {
                        Email.Email.WelcomeEmail(user);
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

                    if (user.Password != null)
                    {
                        // Encrypt password.
                        user.Password = Encryption.Encrypt.EncryptString(user.Password);
                    }


                    if (user.Password == null || user.Email == null)
                    {
                        if (old == null)
                        {
                            // The User should not be upserted if there is no password.
                            return false;
                        }
                        user.Password = old.Password;
                        user.Email = old.Email;
                    }



                    // Save the User in the database.
                    LocalDatabase.UpsertUser(user);
                    return DatabaseOperations.DbUpserts.UpsertUser(user);
                }
                return false;
            }
            catch (Exception ex)
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
        public static int BulkUpsert(IEnumerable<User> users)
        {
            int upserted = 0;
            if (users != null)
            {
                foreach (User user in users)
                {
                    Upsert(user);
                    upserted++;
                }
            }
            return upserted;
        }

        /// <summary>
        /// Deletes a User from all databases.
        /// </summary>
        /// <param name="userName">The Id of the User you want to delete.</param>
        /// <returns>Returns true if the User was deleted.</returns>
        public static bool Delete(string userName)
        {
            if (userName != null)
            {
                LocalDatabase.DeleteUser(userName);
                return DatabaseOperations.DbDeletes.DeleteUser(userName);
            }
            return false;
        }

        /// <summary>
        /// Checks if an id is already used by another User.
        /// </summary>
        /// <param name="id">The id you want to check.</param>
        /// <returns>Returns true if the id is not used by another User.</returns>
        public static bool CheckId(string id)
        {
            IEnumerable<User> all = LocalDatabase.GetAllUsers();
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
        /// Returns all Users.
        /// </summary>
        /// <returns>Returns a list of all Users.</returns>
        public static IEnumerable<User> GetAll()
        {
            return LocalDatabase.GetAllUsers();
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
    }
}