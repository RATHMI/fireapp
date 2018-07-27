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
        /// inserts a User into the database or updates it if it already exists
        /// </summary>
        /// <param name="user">The User you want to insert</param>
        /// <returns>returns true if User was inserted</returns>
        [HttpPost, Route("upload")]//todo: comment
        public bool UpsertUser([FromBody] User u)
        {
            try {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    if (user.UserType == UserTypes.admin)
                    {
                        return DatabaseOperations.Users.Upsert(u, user);
                    }
                }
                return false;   
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);               
                return false;
            }
            
        }

        /// <summary>
        /// inserts an array of Users into the database or updates it if it already exists
        /// </summary>
        /// <param name="user">The User you want to insert</param>
        /// <returns>returns the number of upserted Users.
        /// -1 : invalid or no token
        /// -2 : user is not an admin
        /// -3 : an error occured</returns>
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
                        return -2;
                    }
                }
                else
                {
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
        /// 
        /// </summary>
        /// <returns>returns a csv file with all ServiceGroups</returns>
        [HttpGet, Route("getcsv")]//todo: comment
        public HttpResponseMessage GetCsv()
        {
            HttpResponseMessage result;
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    if (user.UserType == UserTypes.admin) // only allow admin to get csv file
                    {
                        var stream = new MemoryStream();

                        // convert all users to a byte array
                        IEnumerable<User> users;
                        users = DatabaseOperations.Users.GetAll();
                        byte[] file = FileOperations.UserFiles.ExportToCSV(users);
                        stream.Write(file, 0, file.Length);

                        stream.Position = 0;    // set position of stream to start
                        result = new HttpResponseMessage(HttpStatusCode.OK);
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
                        result = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                        result.Content = null;
                    }
                }
                else
                {
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
        /// Retrieves Users from CSV and upserts them.
        /// </summary>
        /// <param name="bytes">An array of bytes that represents a CSV file.</param>
        /// <returns>The number of successfully upserted Users.</returns>
        [HttpPost, Route("uploadcsv")]//todo: comment
        public HttpResponseMessage UpsertCsv([FromBody] byte[] bytes)
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
                        users = FileOperations.UserFiles.GetUsersFromCSV(bytes);
                        int upserted = DatabaseOperations.Users.BulkUpsert(users, user);

                        // sets the content of the response to the number of upserted users
                        result.Content = new ByteArrayContent(Encoding.ASCII.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(upserted)));
                    }
                    else
                    {
                        result = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                        result.Content = null;
                    }
                }
                else
                {
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
        [HttpPost, Route("checkid/{id}")]
        public bool CheckId(string id)
        {
            return DatabaseOperations.Users.CheckId(id);
        }

        /// <summary>
        /// This method generates a new token and 
        /// saves it in the User object with the matching UserLogin. 
        /// </summary>
        /// <param name="login">the login data of a user</param>
        /// <returns>returns the token if the login worked or null if not</returns>
        [HttpPost, Route("authenticate")]
        public string Authenticate([FromBody]UserLogin login)
        {
            return Authentication.Token.RefreshToken(login);
        }

        /// <summary>
        /// Checks if the token of the request is valid
        /// </summary>
        /// <returns>returns the user if the token is valid</returns>
        [HttpGet, Route("getuser")]
        public User[] GetUser()
        {
            User user;
            Authentication.Token.CheckAccess(Request.Headers, out user);
            return new User[] { user };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName">Id of the User you want to delete</param>
        /// <returns>returns true if User was deleted</returns>
        [HttpGet, Route("delete/{username}")]//todo: comment
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
        /// 
        /// </summary>
        /// <returns>returns a list with all Users</returns>
        [HttpGet, Route("all")]//todo: comment
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
        /// returns all Users that have a UserType that is matching with a UserType from usertypes
        /// </summary>
        /// <param name="usertypes">an array of usertypes</param>
        /// <returns>returns a list of all users with matching usertypes</returns>
        [HttpPost, Route("usertype")]//todo: comment
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
        /// 
        /// </summary>
        /// <param name="username">The username of the User you are looking for</param>
        /// <returns>returns a User with a matching username</returns>
        [HttpGet, Route("id/{username}")]//todo: comment
        public User[] GetUserById(string userName)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    return new User[] { DatabaseOperations.Users.GetById(userName) };
                }
                else
                {
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
        [HttpGet, Route("active")]//todo: comment
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
                        return DatabaseOperations.Users.GetActiveUsers().ToArray();
                    }
                    else
                    {
                        throw new UnauthorizedAccessException();
                    }         
                }
                else
                {
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
        [HttpGet, Route("inactive")]//todo: comment
        public User[] GetInactiveUsers()
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    // Only return Users if User is an admin.
                    if (user.UserType == UserTypes.admin)
                    {
                        return DatabaseOperations.Users.GetInactiveUsers().ToArray();
                    }
                    else
                    {
                        throw new UnauthorizedAccessException();
                    }
                }
                else
                {
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
                            throw new KeyNotFoundException();
                        }
                    }
                    else
                    {
                        throw new UnauthorizedAccessException();
                    }
                }
                else
                {
                    return null;
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new object[0];
            }
        }

        [HttpPost, Route("resetpassword")]
        public bool ResetPassword([FromBody] string email) // todo: comment
        {
            User user = DatabaseOperations.Users.GetByEmail(email);
            if(user != null)
            {
                // Generate a new password.
                user.Password = PasswordGenerator.Generate(10, Sets.Alphanumerics + Sets.Symbols);

                // Upsert the User with the new password.
                DatabaseOperations.Users.Upsert(user, user);

                // Send the User an email with the new password.
                user = (User)user.Clone();
                user.Email = Encryption.Encrypt.DecryptString(user.Email);
                Email.Email.ResetEmail(user);

                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpPost, Route("changepassword")]
        public bool ChangePassword([FromBody] string password) // todo: comment
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    user.Password = password;
                    DatabaseOperations.Users.Upsert(user, user);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
