using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service
{
    public static class DatabaseOperations
    {
        public static IEnumerable<FireEvent> GetAllFireEvents()
        {
            using (var db = AppData.FireEventDB())
            {
                var table = db.FireEventTable();
                return table.FindAll();
            }
        }

        public static bool UploadFireEvent(FireEvent fe)
        {
            using (var db = AppData.FireEventDB())
            {
                var table = db.FireEventTable();
                return table.Upsert(fe);
            }
        }
    }
}