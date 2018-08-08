using Microsoft.VisualStudio.TestTools.UnitTesting;
using FireApp.Service.DatabaseOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FireApp.Domain;

namespace DatabaseOperationsTests
{
    [TestClass()]
    public class FireEventsTests
    {


        [TestMethod()]
        public void GetAllTest()
        {
            UsersTests.GenerateUser("test1", UserTypes.unauthorized);
        }

        [TestMethod()]
        public void UpsertTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CheckIdTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetBySourceIdTest()
        {
            User user;
            FireEvent fe;
            IEnumerable<FireEvent> retrieved;
            int sourceId1 = 1;
            int sourceId2 = 2;

            user = UsersTests.GenerateUser("test1", UserTypes.unauthorized);
            
            fe = new FireEvent(new FireEventId(sourceId1, 1), DateTime.Now, "target", "description", EventTypes.test);
            FireApp.Service.DatabaseOperations.FireEvents.Upsert(fe, user);
            retrieved = FireApp.Service.DatabaseOperations.FireEvents.GetBySourceId(sourceId1);
            Assert.IsTrue(retrieved.Count() == 1);

            fe = new FireEvent(new FireEventId(sourceId1, 2), DateTime.Now, "target", "description", EventTypes.test);
            FireApp.Service.DatabaseOperations.FireEvents.Upsert(fe, user);
            retrieved = FireApp.Service.DatabaseOperations.FireEvents.GetBySourceId(sourceId1);
            Assert.IsTrue(retrieved.Count() == 2);

            fe = new FireEvent(new FireEventId(sourceId2, 1), DateTime.Now, "target", "description", EventTypes.test);
            FireApp.Service.DatabaseOperations.FireEvents.Upsert(fe, user);
            retrieved = FireApp.Service.DatabaseOperations.FireEvents.GetBySourceId(sourceId1);
            Assert.IsTrue(retrieved.Count() == 2);
            retrieved = FireApp.Service.DatabaseOperations.FireEvents.GetBySourceId(sourceId2);
            Assert.IsTrue(retrieved.Count() == 1);
            
        }

        [TestMethod()]
        public void GetByIdTest()
        {
            User user = UsersTests.GenerateUser("test1", UserTypes.unauthorized);
            FireEvent fe = new FireEvent(new FireEventId(1, 1), DateTime.Now, "target", "description", EventTypes.test);
            FireApp.Service.DatabaseOperations.FireEvents.Upsert(fe, user);

            FireEvent retrieved = FireApp.Service.DatabaseOperations.FireEvents.GetById(fe.Id.SourceId, fe.Id.EventId);

            Assert.IsTrue(fe.Equals(retrieved));
        }

        [TestMethod()]
        public void GetByTargetTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetByEventTypeTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CountByEventTypePerYearTest()
        {
            Assert.Fail();
        }
    }
}