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
            BsonMapper.Global.Entity<ServiceMember>()
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

        #region ServiceMemberDB
        /// <summary>
        /// Stores all ServiceMembers
        /// </summary>
        /// <returns>returns the database</returns>
        public static LiteDatabase ServiceMemberDB()
        {
            return new LiteDatabase(AppSettings.FireEventDBPath);
        }
        public static LiteCollection<ServiceMember> ServiceMemberTable(this LiteDatabase db)
        {
            return db.GetCollection<ServiceMember>("service");
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
    }
}