using Microsoft.VisualStudio.TestTools.UnitTesting;
using FireApp.Service.DatabaseOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FireApp.Domain;
using FireApp.Service;
using System.Collections;

namespace DatabaseOperationsTests
{
    [TestClass()]
    public class UsersTests
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
        public void UpsertTest()
        {
            // Test insert. 
            User user = generateUser("admin", UserTypes.unauthorized);                                        
            Assert.IsTrue(FireApp.Service.DatabaseOperations.Users.Upsert(user, user));

            // Test upsert.
            User copy = (User)user.Clone();
            copy.FirstName = "copy";        
            Assert.IsTrue(FireApp.Service.DatabaseOperations.Users.Upsert(copy, copy));

            // Test duplicate email.
            User duplicateEmail = generateUser("duplicateEmail", UserTypes.unauthorized);
            duplicateEmail.Email = user.Email;
            Assert.IsFalse(FireApp.Service.DatabaseOperations.Users.Upsert(duplicateEmail, duplicateEmail));
            
        }

        [TestMethod()]
        public void BulkUpsertTest()
        {
            User admin = generateUser("admin", UserTypes.unauthorized);
            List<User> users = new List<User>()
            {
                generateUser("test1", UserTypes.unauthorized),
                generateUser("test2", UserTypes.unauthorized),
                generateUser("test3", UserTypes.unauthorized),
                generateUser("test4", UserTypes.unauthorized)
            };

            // Test insert.
            Assert.Equals(4, FireApp.Service.DatabaseOperations.Users.BulkUpsert(users, admin));

            // Test duplicates.
            users.Add(generateUser("test1", UserTypes.unauthorized));
            Assert.Equals(4, FireApp.Service.DatabaseOperations.Users.BulkUpsert(users, admin));

            // Test empty list.
            users.Clear();
            Assert.Equals(0, FireApp.Service.DatabaseOperations.Users.BulkUpsert(users, admin));

            // Test null.
            Assert.Equals(0, FireApp.Service.DatabaseOperations.Users.BulkUpsert(null, admin));
        }

        [TestMethod()]
        public void DeleteTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CheckIdTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetAllTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetByUserTypeTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetByUserTypeTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetFirstAdminTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetByIdTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetByEmailTest()
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
        public void GetByAuthorizedObjectTest()
        {
            Assert.Fail();
        }
    }
}