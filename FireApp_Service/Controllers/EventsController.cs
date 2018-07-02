﻿using FireApp.Domain;
using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FireApp.Service.Controllers {
    [RoutePrefix("events")]
    public class EventsController : ApiController {

        [HttpPost, Route("upload")]
        public bool DetailsByCompanyId([FromBody] FireEvent fe) {
            using (var db = AppData.FireEventDB()) {
                var table = db.FrieEventTable();
                return table.Upsert(fe);
            }
        }

        [HttpGet, Route("id/{sourceId}/{eventId}")]
        public FireEvent GetFireEventById(int sourceId, int eventId)
        {
            using (var db = AppData.FireEventDB())
            {
                var table = db.FrieEventTable();

                return table.FindOne(x => x.Id.SourceId == sourceId && x.Id.EventId == eventId);
            }
        }

        [HttpGet, Route("inserttest/{name}")]
        public bool DetailsByCompanyId(string name) {
            using (var db = AppData.FireEventDB()) {
                var table = db.FrieEventTable();
                return table.Upsert(new FireEvent {
                    TimeStamp = DateTime.Now,
                    TargetId = "asdasddas",
                });
            }
        }

        [HttpGet, Route("all")]
        public IEnumerable<FireEvent> All() {
            using (var db = AppData.FireEventDB()) {
                var table = db.FrieEventTable();
                return table.FindAll();
            }
        }

        /*[HttpGet, Route("getNameTest/{name}")]
        public IEnumerable<FireEvent> GetName(string name)
        {
            using (var db = AppData.FireEventDB())
            {
                var table = db.FrieEventTable();
                return table.Find(x => x.Name == name);             
            }
        }
        */
    }
}
