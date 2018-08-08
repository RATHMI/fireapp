using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FireApp.Domain;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using MlkPwgen;

namespace FireApp.Service.Controllers
{
    [RoutePrefix("user")]
    public class UserController : ApiController
    {
        /// <summary>
        /// Inserts a User into the database or updates it if it already exists.
        /// Allows unregistered Users to create a new User.
        /// Allows registered Users to share an authorized object with another User.
        /// </summary>
        /// <param name="u">The User you want to upsert.</param>
        /// <returns>Returns true if the User was inserted.</returns>
        [HttpPost, Route("upload")]
        public bool UpsertUser([FromBody] User u)
        {
            try {
                if(u == null)
                {
                    throw new ArgumentNullException();
                }

                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    if (user.UserType == UserTypes.admin)
                    {
                        // Check if the User already exists.
                        User old = DatabaseOperations.Users.GetById(u.Id);
                        if (old != null)
                        {
                            // If the UserType changed
                            if (old.UserType != u.UserType)
                            {
                                // Allow to change the UserType to change to admin and unauthorized.
                                if (u.UserType != UserTypes.admin && u.UserType != UserTypes.unauthorized)
                                {
                                    if(u.AuthorizedObjectIds.Count != 0)
                                    {
                                        // Only change the UserType when there are no authorized obejects.
                                        throw new InvalidOperationException();
                                    }
                                }
                            }

                            return DatabaseOperations.Users.Upsert(u, user);
                        }
                        else
                        {
                            return DatabaseOperations.Users.Upsert(u, user);
                        }
                    }
                    else
                    {
                        // Allow the User to edit its own User.
                        if(user.Id == u.Id)
                        {
                            // Do not allow the User to change its UserType.
                            if(user.UserType != u.UserType)
                            {
                                throw new InvalidOperationException();
                            }

                            // Allow the User to delete but not to add AuthorizedObjectIds.
                            foreach (int authobject in u.AuthorizedObjectIds)
                            {
                                if (user.AuthorizedObjectIds.Contains(authobject) == false)
                                {
                                    throw new InvalidOperationException();
                                }
                            }

                            return DatabaseOperations.Users.Upsert(u, user);
                        }
                        else
                        {
                            // Do not allow the User to create a user with another UserType.
                            if (user.UserType != u.UserType)
                            {
                                throw new InvalidOperationException();
                            }

                            User old = DatabaseOperations.Users.GetById(u.Id);

                            // Allow Users to add other another User to their group.
                            if (old == null)
                            {
                                // The User does not already exist.
                                // Allow to create a User of the same UserType with the same or less AuthorizedObejctIds.                             

                                // Allow the User to delete but not to add AuthorizedObjectIds.
                                foreach (int authobject in u.AuthorizedObjectIds)
                                {
                                    if (user.AuthorizedObjectIds.Contains(authobject) == false)
                                    {
                                        throw new InvalidOperationException();
                                    }
                                }

                                return DatabaseOperations.Users.Upsert(u, user);
                            }
                            else
                            {                                                       
                                List<int> changedIds = new List<int>();

                                // Get all AuthorizedObejectIds that were added.
                                foreach (int authobject in u.AuthorizedObjectIds)
                                {
                                    if(old.AuthorizedObjectIds.Contains(authobject) == false)
                                    {
                                        changedIds.Add(authobject);
                                    }
                                }

                                // Get all AuthorizedObejectIds that were deleted.
                                foreach (int authobject in old.AuthorizedObjectIds)
                                {
                                    if (u.AuthorizedObjectIds.Contains(authobject) == false)
                                    {
                                        changedIds.Add(authobject);
                                    }
                                }

                                // Only allow the User to add or delete AuthorizedObejectIds that are linked to its own User.
                                foreach (int authobject in changedIds)
                                {
                                    if (user.AuthorizedObjectIds.Contains(authobject) == false)
                                    {
                                        throw new InvalidOperationException();
                                    }
                                }

                                old.AuthorizedObjectIds = u.AuthorizedObjectIds;

                                return DatabaseOperations.Users.Upsert(old, user);
                            }
                        }
                    }
                }
                else
                {
                    if (DatabaseOperations.Users.GetById(u.Id) == null)
                    {
                        // Allow everybody to create a new User but not with the UserType admin
                        // and with no AuthorizedObjectIds. 

                        if(u.UserType == UserTypes.admin)
                        {
                            throw new InvalidOperationException();
                        }

                        if(u.AuthorizedObjectIds.Count != 0)
                        {
                            throw new InvalidOperationException();
                        }

                        return DatabaseOperations.Users.Upsert(u, u);
                    }
                    else
                    {
                        // User must be logged in to update an existing User.
                        throw new InvalidOperationException();
                    }
                } 
            }
            catch(Exception ex)
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
        /// -1 : invalid or no token.
        /// -2 : user is not an admin.
        /// -3 : an error occured.</returns>
        [HttpPost, Route("uploadbulk")]
        public int UpsertBulk([FromBody] User[] users)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    if (user.UserType == UserTypes.admin)
                    {
                        return DatabaseOperations.Users.BulkUpsert(users, user);
                    }
                    else
                    {
                        // User is not an admin.
                        return -2;
                    }
                }
                else
                {
                    // Notify user that the login was not successful.
                    return -1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -3;
            }
        }

        /// <summary>
        /// Returns all Users as CSV file.
        /// </summary>
        /// <returns>Returns a CSV file with all Users.</returns>
        [HttpGet, Route("getcsv")]
        public HttpResponseMessage GetCsv()
        {
            HttpResponseMessage result;
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    if (user.UserType == UserTypes.admin)
                    {
                        var stream = new MemoryStream();

                        // Get all Users.
                        IEnumerable<User> users = DatabaseOperations.Users.GetAll();

                        // Convert FireBrigades into a CSV file.
                        byte[] file = FileOperations.UserFiles.ExportToCSV(users);

                        // Write CSV file into the stream.
                        stream.Write(file, 0, file.Length);

                        // Set position of stream to 0 to avoid problems with the index.
                        stream.Position = 0;
                        result = new HttpResponseMessage(HttpStatusCode.OK);

                        // Add the CSV file to the content of the response.
                        result.Content = new ByteArrayContent(stream.ToArray());                       
                        result.Content.Headers.ContentDisposition =
                            new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                            {
                                FileName = "Users.csv"
                            };
                        result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
                    }
                    else
                    {
                        // User is not an admin.
                        result = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                        result.Content = null;
                    }
                }
                else
                {
                    // Notify user that the login was not successful.
                    result = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    result.Content = null;
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                result = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                return result;
            }
        }

        /// <summary>
        /// Retrieves Users from a CSV and upserts them.
        /// </summary>
        /// <param name="byteArrayString">An array of bytes as a string that represents a CSV file.</param>
        /// <returns>The number of successfully upserted Users.</returns>
        [HttpPost, Route("uploadcsv")]
        public HttpResponseMessage UpsertCsv([FromBody] string byteArrayString)
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    if (user.UserType == UserTypes.admin)
                    {
                        IEnumerable<User> users;
                        List<byte> bytes = new List<byte>();

                        byteArrayString = byteArrayString.Trim('"'); 
                        // Convert the string into an array of bytes.                  
                        foreach (string s in byteArrayString.Split(' '))
                        {
                            bytes.Add(Convert.ToByte(s));
                        }

                        // Get the Users from the array of bytes.
                        users = FileOperations.UserFiles.GetUsersFromCSV(bytes.ToArray());

                        // Upsert the Users into the database.
                        int upserted = DatabaseOperations.Users.BulkUpsert(users, user);

                        // Sets the content of the response to the number of upserted Users.
                        result.Content = new ByteArrayContent(Encoding.ASCII.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(upserted)));
                    }
                    else
                    {
                        // The User is not an admin.
                        result = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                        result.Content = null;
                    }
                }
                else
                {
                    // Notify the User that the login was not successful.
                    result = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    result.Content = null;
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                result = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                return result;
            }
        }

        /// <summary>
        /// Checks if an id is already used by another User
        /// </summary>
        /// <param name="id">the id you want to check</param>
        /// <returns>returns true if id is not used by other User</returns>
        [HttpGet, Route("checkid/{id}")]
        public bool CheckId(string id)
        {
            return DatabaseOperations.Users.CheckId(id);
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
        [HttpPost, Route("checkpassword")]
        public Int32 CheckId([FromBody] User user)
        {
            return DatabaseOperations.Users.CheckPassword(user);
        }

        /// <summary>
        /// This method generates a new token and 
        /// saves it in the User object with the matching UserLogin. 
        /// </summary>
        /// <param name="login">The login data of a User.</param>
        /// <returns>Returns the token if the login worked or null if not.</returns>
        [HttpPost, Route("authenticate")]
        public string Authenticate([FromBody]UserLogin login)
        {
            return Authentication.Token.RefreshToken(login);
        }

        /// <summary>
        /// Checks if the token of the request is valid.
        /// </summary>
        /// <returns>Returns the user if the token is valid.</returns>
        [HttpGet, Route("getuser")]
        public User[] GetUser()
        {
            User user;
            Authentication.Token.CheckAccess(Request.Headers, out user);
            return new User[] { user };
        }

        /// <summary>
        /// Deletes the User from the databases.
        /// </summary>
        /// <param name="userName">Id of the User you want to delete.</param>
        /// <returns>Returns true if User was deleted.</returns>
        [HttpGet, Route("delete/{username}")]
        public bool DeleteUser(string userName)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    if (user.UserType == UserTypes.admin)
                    {
                        return DatabaseOperations.Users.Delete(userName, user);
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }
                else
                {
                    throw new ArgumentNullException();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }                        
        }

        /// <summary>
        /// Returns all Users.
        /// </summary>
        /// <returns>Returns a list of all Users.</returns>
        [HttpGet, Route("all")]
        public User[] GetAllUsers()
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    IEnumerable<User> users;
                    users = DatabaseOperations.Users.GetAll();
                    users = Filter.UsersFilter.UserFilter(users, user);
                    return users.ToArray();
                }
                else
                {
                    // Notify user that the login was not successful.
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new User[0];
            }           
        }

        /// <summary>
        /// Returns all Users that have a UserType that is matching with a UserType from usertypes.
        /// </summary>
        /// <param name="usertypes">An array of usertypes.</param>
        /// <returns>Returns a list of all Users with matching usertypes.</returns>
        [HttpPost, Route("usertype")]
        public User[] GetUserByUserTypes([FromBody] UserTypes[] usertypes)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    IEnumerable<User> users;
                    users = DatabaseOperations.Users.GetByUserType(usertypes);
                    users = Filter.UsersFilter.UserFilter(users, user);
                    return users.ToArray();
                }
                else
                {
                    // Notify user that the login was not successful.
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new User[0];
            }                      
        }

        /// <summary>
        /// Returns a User with a matching username.
        /// </summary>
        /// <param name="username">The username of the User you are looking for.</param>
        /// <returns>Returns a User with a matching username.</returns>
        [HttpGet, Route("id/{username}")]
        public User[] GetUserById(string username)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    // Get the User by the username.
                    user = DatabaseOperations.Users.GetById(username);
                    if (user != null)
                    {
                        return Filter.UsersFilter.UserFilter(new User[] { user }, user).ToArray();
                    }
                    else
                    {
                        // The User does not exist.
                        throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    // Notify user that the login was not successful.
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new User[0];
            }           
        }

        /// <summary>
        /// Returns all active Users.
        /// </summary>
        /// <returns>Returns a List of Users with a valid token.</returns>
        [HttpGet, Route("active")]
        public User[] GetActiveUsers()
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    if (user.UserType == UserTypes.admin)
                    {
                        IEnumerable<User> users;
                        users = DatabaseOperations.Users.GetActiveUsers();
                        users = Filter.UsersFilter.UserFilter(users, user);
                        return users.ToArray();
                    }
                    else
                    {
                        // User is not an admin.
                        throw new UnauthorizedAccessException();
                    }         
                }
                else
                {
                    // Notify user that the login was not successful.
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new User[0];
            }       
        }

        /// <summary>
        /// Returns all inactive Users.
        /// </summary>
        /// <returns>Returns a List of Users with an invalid token.</returns>
        [HttpGet, Route("inactive")]
        public User[] GetInactiveUsers()
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    if (user.UserType == UserTypes.admin)
                    {
                        IEnumerable<User> users;
                        users = DatabaseOperations.Users.GetInactiveUsers();
                        users = Filter.UsersFilter.UserFilter(users, user);
                        return users.ToArray();
                    }
                    else
                    {
                        // User is not an admin.
                        throw new UnauthorizedAccessException();
                    }
                }
                else
                {
                    // Notify user that the login was not successful.
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new User[0];
            }
        }

        /// <summary>
        /// If the type is "groups" it returns the authorized objects of the User.
        /// If the type is "fas" it returns the FireAlarmSystems that are directly 
        /// or indirectly associated with the User.
        /// </summary>
        /// <param name="username">The username of the User you want to get the objects from.</param>
        /// <param name="type">The type of objects you want.</param>
        /// <returns>Returns FireBrigades or ServiceGroups or FireAlarmSystems.</returns>
        [HttpGet, Route("authobjects/{username}/{type}")]
        public object[] GetAuthorizedObjects(string username, string type) 
        {
            try
            {
                List<object> results = new List<object>();
                User user;

                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    // Only return Users if User is an admin.
                    if (user.UserType == UserTypes.admin)
                    {
                        // Find the user with a matching username.
                        user = DatabaseOperations.Users.GetById(username);
                        if(user != null)
                        {
                            if (type == "groups")
                            {
                                results.AddRange(DatabaseOperations.Users.GetAuthorizedObjects(user));
                            }
                            else
                            {
                                if(type == "fas")
                                {
                                    results.AddRange(DatabaseOperations.FireAlarmSystems.GetByUser(user));
                                }
                            }

                            return results.ToArray();
                        }
                        else
                        {
                            // The User was not found.
                            throw new KeyNotFoundException();
                        }
                    }
                    else
                    {
                        // User is not an admin.
                        throw new UnauthorizedAccessException();
                    }
                }
                else
                {
                    // Notify user that the login was not successful.
                    return null;
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new object[0];
            }
        }

        /// <summary>
        /// Generates a new password for the User with a matching username and sends the new
        /// login credentials via email to the User.
        /// </summary>
        /// <param name="username">The username of the User who wants to reset the password.</param>
        /// <returns>Returns true if the email was sent.</returns>
        [HttpGet, Route("resetpassword/{username}")]
        public bool ResetPassword(string username)
        {
            User user = DatabaseOperations.Users.GetById(username);
            if(user != null)
            {
                // Generate a new password.
                user.Password = PasswordGenerator.Generate(10, Sets.Alphanumerics);

                // Upsert the User with the new password.
                DatabaseOperations.Users.Upsert(user, user);

                
                // Get a clone of the User so the password is not changed when decypting it.
                user = (User)user.Clone();
                user.Password = Encryption.Encrypt.DecryptString(user.Password);

                // Send the User an email with the new password.
                Email.Email.ResetPasswordEmail(user);

                return true;
            }
            else
            {
                // Notify user that the login was not successful.
                return false;
            }
        }

        /// <summary>
        /// Sets the password of the logged in User to the given password.
        /// </summary>
        /// <param name="password">The new password of the User.</param>
        /// <returns>Returns true if the password was set.</returns>
        [HttpPost, Route("changepassword")]
        public bool ChangePassword([FromBody] string password)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    // If the User is logged in, change the password.
                    user.Password = password;
                    DatabaseOperations.Users.Upsert(user, user);
                    return true;
                }
                else
                {
                    // Notify user that the login was not successful.
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Adds or Removes an authorizedObjectId to or from the User's authorizedObjectIds.
        /// </summary>
        /// <param name="username">The username of the User you want to perform the operation on.</param>
        /// <param name="authobj">The id of the authorized object.</param>
        /// <param name="operation">Describes which operation you want to perform ("add" or "delete").</param>
        /// <returns>
        /// Returns an error code.
        /// 1 : operation was successful.
        /// 0 : User is not logged in.
        /// -1 : User is not an admin.
        /// -2 : the User can not have authorized objects.
        /// -3 : one of the parameters was null.
        /// -4 : the operation does not exist.
        /// -5 : an error occured.
        /// </returns>
        [HttpGet, Route("changeauthobject/{username}/{authobj}/{operation}")]
        public Int32 ChangeAuthorizedObject(string username, int authobj, string operation)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    if (user.UserType == UserTypes.admin)
                    {
                        if (username == null || operation == null)
                        {
                            throw new ArgumentNullException();
                        }
                        else
                        {
                            // Get the User by the username.
                            User changedUser = DatabaseOperations.Users.GetById(username);
                            if (changedUser == null)
                            {
                                // The User was not found.
                                throw new ArgumentOutOfRangeException();
                            }

                            // Get the authorized object by the id.
                            // The method throws an Exception if the object does not exist.
                            if(changedUser.UserType == UserTypes.firebrigade)
                            {
                                DatabaseOperations.FireBrigades.GetById(authobj);
                            }
                            else
                            {
                                if(changedUser.UserType == UserTypes.servicemember)
                                {
                                    DatabaseOperations.ServiceGroups.GetById(authobj);
                                }
                                else
                                {
                                    if (changedUser.UserType == UserTypes.firealarmsystem)
                                    {
                                        DatabaseOperations.FireAlarmSystems.GetById(authobj);
                                    }
                                    else
                                    {
                                        // User has the wrong UserType.
                                        throw new InvalidOperationException();
                                    }
                                }
                            }
                            

                            if (operation == "add")
                            {
                                // Add authorizedObjectId to the User's authorizedObjectIds.
                                changedUser.AuthorizedObjectIds.Add(authobj);
                                DatabaseOperations.Users.Upsert(changedUser, user);
                                return 1;
                            }
                            else
                            {
                                if (operation == "delete")
                                {
                                    // Remove authorizedObjectId to the User's authorizedObjectIds.
                                    changedUser.AuthorizedObjectIds.Remove(authobj);
                                    DatabaseOperations.Users.Upsert(changedUser, user);
                                    return 1;
                                }
                                else
                                {
                                    // The operation does not exist.
                                    throw new ArgumentOutOfRangeException();
                                }
                            }
                        }
                    }
                    else
                    {
                        // User is not an admin.
                        return -1;
                    }
                }
                else
                {
                    // Notify user that the login was not successful.
                    return 0;
                }
            }
            catch (InvalidOperationException)
            {
                // User has the wrong UserType.
                return -2;
            }
            catch (ArgumentNullException)
            {
                return -3;
            }
            catch(ArgumentOutOfRangeException)
            {
                // The operation does not exist or the User was not found.
                return -4;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -5;
            }
        }
    }
}
