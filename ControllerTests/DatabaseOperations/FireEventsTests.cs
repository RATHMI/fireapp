﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            Assert.Fail();
        }

        [TestMethod()]
        public void GetByIdTest()
        {
            Assert.Fail();
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