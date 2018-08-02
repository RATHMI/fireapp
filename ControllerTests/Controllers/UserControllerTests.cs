using Microsoft.VisualStudio.TestTools.UnitTesting;
using FireApp.Service.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FireApp.Domain;
using FireApp.Service;

namespace UserController
{
    [TestClass()]
    public class UserControllerTests
    {
        private static User generateUser(string username, UserTypes usertype)
        {
            string password = "test";
            string firstname = "firstname";
            string lastname = "lastname";
            string email = username + "@siemens.com";

            return new User(username, password, firstname, lastname, email, usertype);
        }

        [TestMethod()]
        public void UpsertUserTest()
        {

        }

        [TestMethod()]
        public void UpsertBulkTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetCsvTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void UpsertCsvTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CheckIdTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AuthenticateTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetUserTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DeleteUserTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetAllUsersTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetUserByUserTypesTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetUserByIdTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetActiveUsersTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetInactiveUsersTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetAuthorizedObjectsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ResetPasswordTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ChangePasswordTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ChangeAuthorizedObjectTest()
        {
            Assert.Fail();
        }
    }
}