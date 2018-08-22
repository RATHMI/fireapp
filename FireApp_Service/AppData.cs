using FireApp.Domain;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml.Serialization;
using static FireApp.Domain.User;

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

        public static string ServerAddress()
        {
            return ConfigurationManager.AppSettings["serviceBaseAddress"];
        }

        public static Int64 TokenValidHours(UserTypes userType)
        {
            Int64 rv = 0;

            switch (userType)
            {
                case UserTypes.admin: rv = Convert.ToInt64(ConfigurationManager.AppSettings["tokenValidHours_Admin"]); break;
                case UserTypes.fireSafetyEngineer: rv = Convert.ToInt64(ConfigurationManager.AppSettings["tokenValidHours_FireSafetyEngineer"]); break;
                case UserTypes.servicemember: rv = Convert.ToInt64(ConfigurationManager.AppSettings["tokenValidHours_Servicemember"]); break;
                case UserTypes.fireFighter: rv = Convert.ToInt64(ConfigurationManager.AppSettings["tokenValidHours_Firefighter"]); break;
                case UserTypes.unauthorized: rv = Convert.ToInt64(ConfigurationManager.AppSettings["tokenValidHours_Unauthorized"]); break;
            }

            return rv;
        }

        #region FireEventDB
        /// <summary>
        /// Stores all FireEvents.
        /// </summary>
        /// <returns>Returns the database.</returns>
        public static LiteDatabase FireEventDB() {
            return new LiteDatabase(AppSettings.DBPath);
        }
        public static LiteCollection<FireEvent> FireEventTable(this LiteDatabase db) {
            return db.GetCollection<FireEvent>("events");
        }
        #endregion

        #region FireAlarmSystemDB
        /// <summary>
        /// Stores FireAlarmSystems.
        /// </summary>
        /// <returns>Returns the database.</returns>
        public static LiteDatabase FireAlarmSystemDB()
        {
            return new LiteDatabase(AppSettings.DBPath);
        }
        public static LiteCollection<FireAlarmSystem> FireAlarmSystemTable(this LiteDatabase db)
        {
            return db.GetCollection<FireAlarmSystem>("fas");
        }
        #endregion      

        #region FireBrigadeDB
        /// <summary>
        /// Stores all FireBrigades.
        /// </summary>
        /// <returns>Returns the database.</returns>
        public static LiteDatabase FireBrigadeDB()
        {
            return new LiteDatabase(AppSettings.DBPath);
        }
        public static LiteCollection<FireBrigade> FireBrigadeTable(this LiteDatabase db)
        {
            return db.GetCollection<FireBrigade>("fb");
        }
        #endregion

        #region ServiceGroupDB
        /// <summary>
        /// Stores all ServiceGroups.
        /// </summary>
        /// <returns>Returns the database.</returns>
        public static LiteDatabase ServiceGroupDB()
        {
            return new LiteDatabase(AppSettings.DBPath);
        }
        public static LiteCollection<ServiceGroup> ServiceGroupTable(this LiteDatabase db)
        {
            return db.GetCollection<ServiceGroup>("service");
        }
        #endregion

        #region ActiveFireEventDB
        /// <summary>
        /// Stores all active FireEvents.
        /// </summary>
        /// <returns>Returns the database.</returns>
        public static LiteDatabase ActiveFireEventDB()
        {
            return new LiteDatabase(AppSettings.DBPath);
        }
        public static LiteCollection<FireEvent> ActiveFireEventTable(this LiteDatabase db)
        {
            return db.GetCollection<FireEvent>("active");
        }
        #endregion

        #region UserDB
        /// <summary>
        /// Stores all Users.
        /// </summary>
        /// <returns>Returns the database.</returns>
        public static LiteDatabase UserDB()
        {
            return new LiteDatabase(AppSettings.DBPath);
        }
        public static LiteCollection<User> UserTable(this LiteDatabase db)
        {
            return db.GetCollection<User>("user");
        }
        #endregion
    }
}