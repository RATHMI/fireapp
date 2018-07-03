using FireApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FireApp.Test {
    class Program
    {
        private static HttpClient httpClient = new HttpClient();

        internal string ServiceUrl { get; private set; }
        static string addr = "http://localhost:50862/events/";

        const string userName = "someuser@someemail.com";
        const string password = "Password1!";
        const string apiBaseUri = "http://localhost:18342";
        const string apiGetPeoplePath = "/api/people";

        static void Main(string[] args)
        {
            httpClient = new HttpClient();
            string rv;

            #region testsWithoutAuthorization
            System.Console.WriteLine("\r\n\r\nTest UploadFireEvent");
            FireEvent fe = new FireEvent(new FireEventId(9, 9), new DateTime(2018,6,1,0,0,0), "test", "description", EventTypes.test);
            rv = Tests.UploadFireEvent(addr, fe);
            System.Console.WriteLine(rv);
            System.Console.WriteLine("\r\n------------------------------------------------------------------------------\r\n");
            
            var byId = ServiceGetCall<FireEvent>(addr + "id/9/9");
            System.Console.WriteLine("\r\n\r\nTest GetFireEventById");
            rv = Tests.GetFireEventById(addr, new FireEventId(9,9));
            System.Console.WriteLine(rv);
            System.Console.WriteLine("\r\n------------------------------------------------------------------------------\r\n");
            
            System.Console.WriteLine("\r\n\r\nTest GetFireEventsBySourceId");
            rv = Tests.GetFireEventsBySourceId(addr, 9);
            System.Console.WriteLine(rv);
            System.Console.WriteLine("\r\n------------------------------------------------------------------------------\r\n");

            System.Console.WriteLine("\r\n\r\nTest GetAllFireEvents");
            rv = Tests.GetAllFireEvents(addr);
            System.Console.WriteLine(rv);
            System.Console.WriteLine("\r\n------------------------------------------------------------------------------\r\n");

            System.Console.WriteLine("\r\n\r\nTest GetFireEventsBySourceIdTargetId");
            rv = Tests.GetFireEventsBySourceIdTargetId(addr, 9, "test");
            System.Console.WriteLine(rv);
            System.Console.WriteLine("\r\n------------------------------------------------------------------------------\r\n");

            System.Console.WriteLine("\r\n\r\nTest GetFireEventsBySourceIdEventType");
            rv = Tests.GetFireEventsBySourceIdEventType(addr, 9, EventTypes.test);
            System.Console.WriteLine(rv);
            System.Console.WriteLine("\r\n------------------------------------------------------------------------------\r\n");

            System.Console.WriteLine("\r\n\r\nTest GetFireEventsBySourceIdTimespan");
            DateTime startTime = new DateTime(100);
            DateTime endTime = DateTime.Now;
            rv = Tests.GetFireEventsBySourceIdTimespan(addr, 9, startTime, endTime);
            System.Console.WriteLine("\r\nstartTime: " + startTime.ToString());
            System.Console.WriteLine("\r\nendTime: " + endTime.ToString());
            System.Console.WriteLine(rv);
            System.Console.WriteLine("\r\n------------------------------------------------------------------------------\r\n");
            #endregion

            #region testsWithAuthorization
            //https://blogs.msdn.microsoft.com/martinkearn/2015/03/25/securing-and-securely-calling-web-api-and-authorize/
            //Get the token
            var token = GetAPIToken(userName, password, apiBaseUri).Result;
            Console.WriteLine("Token: {0}", token);

            //Make the call
            var response = GetRequest(token, apiBaseUri, apiGetPeoplePath).Result;
            Console.WriteLine("response: {0}", response);

            //wait for key press to exit
            Console.ReadKey();
        }

        private static async Task<string> GetAPIToken(string userName, string password, string apiBaseUri)
        {
            using (var client = new HttpClient())
            {
                //setup client
                client.BaseAddress = new Uri(apiBaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //setup login data
                var formContent = new FormUrlEncodedContent(new[]
                {
                     new KeyValuePair<string, string>("grant_type", "password"),
                     new KeyValuePair<string, string>("username", userName),
                     new KeyValuePair<string, string>("password", password),
                 });

                //send request
                HttpResponseMessage responseMessage = await client.PostAsync("/Token", formContent);

                //get access token from response body
                var responseJson = await responseMessage.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);
                return jObject.GetValue("access_token").ToString();
            }
        }

        static async Task<string> GetRequest(string token, string apiBaseUri, string requestPath)
        {
            using (var client = new HttpClient())
            {
                //setup client
                client.BaseAddress = new Uri(apiBaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                //make request
                HttpResponseMessage response = await client.GetAsync(requestPath);
                var responseString = await response.Content.ReadAsStringAsync();
                return responseString;
            }
        }
        #endregion

        System.Console.ReadKey();
        }

        #region Templates
        private static T ServiceGetCall<T>(string callAddress)
        {
            HttpResponseMessage resp = httpClient.GetAsync(callAddress).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<T>().Result;
        }

        //public static T ServiceDeleteCall<T>(string callAddress) {
        //    HttpResponseMessage resp = httpClient.DeleteAsync(ServiceUrl + callAddress).Result;
        //    resp.EnsureSuccessStatusCode();
        //    return resp.Content.ReadAsAsync<T>().Result;
        //}

        //public static R ServicePutCall<T, R>(string callAddress, T element) {
        //    HttpResponseMessage resp = httpClient.PutAsJsonAsync<T>(ServiceUrl + callAddress, element).Result;
        //    resp.EnsureSuccessStatusCode();
        //    return resp.Content.ReadAsAsync<R>().Result;
        //}

        private static R ServicePostCall<T, R>(string callAddress, T element)
        {
            HttpResponseMessage resp = httpClient.PostAsJsonAsync<T>(callAddress, element).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<R>().Result;
        }
        #endregion
    }
}
