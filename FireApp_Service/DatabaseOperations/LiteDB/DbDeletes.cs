using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service.DatabaseOperations
{
    /// <summary>
    /// This class is for deleting objects from the LiteDB
    /// </summary>
    public static class DbDeletes
    {
        public static bool DeleteActiveFireEvent(FireEvent fe)
        {
            if (fe != null)
            {
                using (var db = AppData.ActiveFireEventDB())
                {
                    var table = db.ActiveFireEventTable();
                    FireEvent target = table.FindOne(x => x.Id.SourceId == fe.Id.SourceId && x.TargetId == fe.TargetId);
                    if (target != null)
                    {
                        if (table.Delete(x => x.Id == target.Id) > 0)
                        {
                            return true;
                        }                     
                    }
                }
            }

            return false;
        }
    }
}