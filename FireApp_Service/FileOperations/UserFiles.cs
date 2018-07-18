using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;
using System.IO;

namespace FireApp.Service.FileOperations
{
    public static class UserFiles
    {
        public static IEnumerable<User> GetUsersFromCSV(object file)
        {
            //todo: implement method
            return null;
        }

        public static MemoryStream ConvertUsersIntoCSV(IEnumerable<User> user)
        {
            //todo: implement method

            //converting CSV file into bytes array  
            //var dataBytes = File.ReadAllBytes(reqBook);
            //adding bytes to memory stream   
            //var dataStream = new MemoryStream(dataBytes);
            return null;
        }
    }
}