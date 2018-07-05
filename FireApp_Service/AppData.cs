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
        //public static LocalData Data;

        static AppData() {
            BsonMapper.Global.Entity<FireEvent>()
                .Id(x => x.Id, true);
            BsonMapper.Global.Entity<FireAlarmSystem>()
                .Id(x => x.Id, true);
            BsonMapper.Global.Entity<FireBrigade>()
                .Id(x => x.Id, true);
            BsonMapper.Global.Entity<Target>()
                .Id(x => x.Id, true);
        }
        
        #region FireEventDB
        public static LiteDatabase FireEventDB() {
            return new LiteDatabase(AppSettings.FireEventDBPath);
        }
        public static LiteCollection<FireEvent> FireEventTable(this LiteDatabase db) {
            return db.GetCollection<FireEvent>("events");
        }
        #endregion

        #region FireAlarmSystemDB
        public static LiteDatabase FireAlarmSystemDB()
        {
            return new LiteDatabase(AppSettings.FireEventDBPath);
        }
        public static LiteCollection<FireAlarmSystem> FireAlarmSystemTable(this LiteDatabase db)
        {
            return db.GetCollection<FireAlarmSystem>("fas");
        }
        #endregion

        #region FireBrigadeDB
        public static LiteDatabase FireBrigadeDB()
        {
            return new LiteDatabase(AppSettings.FireEventDBPath);
        }
        public static LiteCollection<FireBrigade> FireBrigadeTable(this LiteDatabase db)
        {
            return db.GetCollection<FireBrigade>("fb");
        }
        #endregion

        #region TargetDB
        public static LiteDatabase TargetDB()
        {
            return new LiteDatabase(AppSettings.FireEventDBPath);
        }
        public static LiteCollection<Target> TargetTable(this LiteDatabase db)
        {
            return db.GetCollection<Target>("target");
        }
        #endregion
    }
}