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

        static AppData()
        {
            BsonMapper.Global.Entity<FireEvent>()
                .Id(x => x.Id, true);
            BsonMapper.Global.Entity<FireAlarmSystem>()
                .Id(x => x.Id, true);
            BsonMapper.Global.Entity<FireBrigade>()
                .Id(x => x.Id, true);
            BsonMapper.Global.Entity<ServiceGroup>()
                .Id(x => x.Id, true);
            BsonMapper.Global.Entity<User>()
                .Id(x => x.Id, true);
        }

        #region FireEventDB
        /// <summary>
        /// Stores all FireEvents
        /// </summary>
        /// <returns>returns the database</returns>
        public static LiteDatabase FireEventDB() {
            return new LiteDatabase(AppSettings.FireEventDBPath);
        }
        public static LiteCollection<FireEvent> FireEventTable(this LiteDatabase db) {
            return db.GetCollection<FireEvent>("events");
        }
        #endregion

        #region FireAlarmSystemDB
        /// <summary>
        /// Stores FireAlarmSystems
        /// </summary>
        /// <returns>returns the database</returns>
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
        /// <summary>
        /// Stores all FireBrigades
        /// </summary>
        /// <returns>returns the database</returns>
        public static LiteDatabase FireBrigadeDB()
        {
            return new LiteDatabase(AppSettings.FireEventDBPath);
        }
        public static LiteCollection<FireBrigade> FireBrigadeTable(this LiteDatabase db)
        {
            return db.GetCollection<FireBrigade>("fb");
        }
        #endregion

        #region ServiceGroupDB
        /// <summary>
        /// Stores all ServiceGroups
        /// </summary>
        /// <returns>returns the database</returns>
        public static LiteDatabase ServiceGroupDB()
        {
            return new LiteDatabase(AppSettings.FireEventDBPath);
        }
        public static LiteCollection<ServiceGroup> ServiceGroupTable(this LiteDatabase db)
        {
            return db.GetCollection<ServiceGroup>("service");
        }
        #endregion

        #region ActiveFireEventDB
        /// <summary>
        /// Stores all active FireEvents
        /// </summary>
        /// <returns>returns the database</returns>
        public static LiteDatabase ActiveFireEventDB()
        {
            return new LiteDatabase(AppSettings.FireEventDBPath);
        }
        public static LiteCollection<FireEvent> ActiveFireEventTable(this LiteDatabase db)
        {
            return db.GetCollection<FireEvent>("active");
        }
        #endregion

        #region UserDB
        /// <summary>
        /// Stores all Users
        /// </summary>
        /// <returns>returns the database</returns>
        public static LiteDatabase UserDB()
        {
            return new LiteDatabase(AppSettings.FireEventDBPath);
        }
        public static LiteCollection<User> UserTable(this LiteDatabase db)
        {
            return db.GetCollection<User>("user");
        }
        #endregion
    }
}