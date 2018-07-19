﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FireApp.Domain;
using System.IO;
using System.Net.Http.Headers;
using System.Text;

namespace FireApp.Service.Controllers
{
    //todo: implement authentication
    [RoutePrefix("user")]
    public class UserController : ApiController
    {
        /// <summary>
        /// inserts a User into the database or updates it if it already exists
        /// </summary>
        /// <param name="user">The User you want to insert</param>
        /// <returns>returns true if User was inserted</returns>
        [HttpPost, Route("upload")]
        public bool UpsertUser([FromBody] User user)
        {
            return DatabaseOperations.Users.UpsertUser(user);
        }

        /// <summary>
        /// inserts a User into the database or updates it if it already exists
        /// </summary>
        /// <param name="user">The User you want to insert</param>
        /// <returns>returns the number of upserted Users.
        /// -1 : invalid or no token
        /// -2 : user is not an admin</returns>
        [HttpPost, Route("upload")]
        public int UpsertBulk([FromBody] User[] users)
        {
            List<User> list = new List<User>();
            list.AddRange(users);
            IEnumerable<User> user = Authentication.Token.VerifyToken(Authentication.Token.GetTokenFromHeader(Request.Headers));
            if (user != null)
            {
                if (user.First<User>().UserType == UserTypes.admin)
                {                                      
                    return DatabaseOperations.Users.UpsertUsers(list);
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a csv file with all ServiceGroups</returns>
        [HttpGet, Route("getcsv")]  //todo: comment
        public HttpResponseMessage GetCsv()
        {
            HttpResponseMessage result;
            try
            {
                IEnumerable<User> user = Authentication.Token.VerifyToken(Authentication.Token.GetTokenFromHeader(Request.Headers));
                if (user != null)
                {
                    if (user.First<User>().UserType == UserTypes.admin)
                    {
                        var stream = new MemoryStream();
                        byte[] file = FileOperations.UserFiles.ExportToCSV(DatabaseOperations.Users.GetAllUsers());
                        stream.Write(file, 0, file.Length);

                        stream.Position = 0;
                        result = new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new ByteArrayContent(stream.ToArray())
                        };
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
        /// Retrieves Users from CSV and upserts them
        /// </summary>
        /// <param name="bytes">an array of bytes that represents a csv file</param>
        /// <returns>the number of successfully upserted Users</returns>
        [HttpPost, Route("uploadcsv")]  //todo: comment
        public HttpResponseMessage UpsertCsv([FromBody] byte[] bytes)
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            try
            {
                IEnumerable<User> user = Authentication.Token.VerifyToken(Authentication.Token.GetTokenFromHeader(Request.Headers));
                if (user != null)
                {
                    if (user.First<User>().UserType == UserTypes.admin)
                    {
                        List<User> users = FileOperations.UserFiles.GetUsersFromCSV(bytes).ToList<User>();
                        result.Content = new ByteArrayContent(Encoding.ASCII.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(DatabaseOperations.Users.UpsertUsers(users))));
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
            string token = Authentication.Token.GetTokenFromHeader(Request.Headers);
            IEnumerable<User> users = Authentication.Token.VerifyToken(token);
            if(users != null)
            {
                return Filter.UsersFilter.UserFilter(users, users.First<User>()).ToArray<User>();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName">Id of the User you want to delete</param>
        /// <returns>returns true if User was deleted</returns>
        [HttpGet, Route("delete/{username}")]
        public bool DeleteUser(string userName)
        {
            return DatabaseOperations.Users.DeleteUser(userName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a list with all Users</returns>
        [HttpGet, Route("all")]
        public User[] GetAllUsers()
        {
            //todo: only in debugging
            IEnumerable<User> admin = DatabaseOperations.Users.GetUserById("admin");
            if (admin != null && admin.Count<User>() > 0)
            {
                return Filter.UsersFilter.UserFilter(DatabaseOperations.Users.GetAllUsers(), admin.First<User>()).ToArray<User>();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// returns all Users that have a UserType that is matching with a UserType from usertypes
        /// </summary>
        /// <param name="usertypes">an array of usertypes</param>
        /// <returns>returns a list of all users with matching usertypes</returns>
        public User[] GetUserByUserTypes([FromBody] UserTypes[] usertypes)
        {
            //todo: only in debugging
            IEnumerable<User> admin = DatabaseOperations.Users.GetUserById("admin");
            if (admin != null && admin.Count<User>() > 0)
            {
                return Filter.UsersFilter.UserFilter(DatabaseOperations.Users.GetUserByUserTypes(usertypes), admin.First<User>()).ToArray<User>();
            }
            else
            {
                return null;
            }           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username">The username of the User you are looking for</param>
        /// <returns>returns a User with a matching username</returns>
        [HttpGet, Route("id/{username}")]
        public User[] GetUserById(string userName)
        {
            return DatabaseOperations.Users.GetUserById(userName).ToArray<User>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a List of Users with a valid token</returns>
        [HttpGet, Route("active")]
        public User[] GetActiveUsers()
        {
            return DatabaseOperations.Users.GetActiveUsers().ToArray<User>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a List of Users with an invalid token</returns>
        [HttpGet, Route("inactive")]
        public User[] GetInactiveUsers()
        {
            return DatabaseOperations.Users.GetInactiveUsers().ToArray<User>();
        }


        [HttpGet, Route("filedownload")]
        public HttpResponseMessage GetUserCSV()
        {
            string fileName = "users.CSV";

            //converting CSV file into bytes array  
            var dataBytes = File.ReadAllBytes("");
            //adding bytes to memory stream   
            var dataStream = new MemoryStream(dataBytes);

            HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
            httpResponseMessage.Content = new StreamContent(dataStream);
            httpResponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            httpResponseMessage.Content.Headers.ContentDisposition.FileName = fileName;
            httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

            return httpResponseMessage;
        }

    }
}
