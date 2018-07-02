using FireApp.Domain;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace FireApp.Service {
    public static class AppData {
        static AppData() {
            BsonMapper.Global.Entity<FireEvent>()
                .Id(x => x.Id, true);

        }

        #region FireEventDB
        public static LiteDatabase FireEventDB() {
            return new LiteDatabase(AppSettings.FireEventDBPath);
        }
        public static LiteCollection<FireEvent> FrieEventTable(this LiteDatabase db) {
            return db.GetCollection<FireEvent>("events");
        }
        #endregion
    }
}