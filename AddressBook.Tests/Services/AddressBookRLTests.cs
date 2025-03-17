using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Helper;
using RepositoryLayer.Service;

namespace AddressBook.Tests.Services
{
    [TestFixture]
    public class AddressBookRLTests
    {
        private AddressBookContext _context;
        private Mock<IRedisCacheHelper> _cacheHelperMock;
        private AddressBookRL _addressBookRL;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AddressBookContext>()
                .UseInMemoryDatabase(databaseName: "AddressBookDB")
                .Options;

            _context = new AddressBookContext(options);
            _cacheHelperMock = new Mock<IRedisCacheHelper>();

            _cacheHelperMock.Setup(c => c.GetCacheAsync<List<AddressBookEntry>>(It.IsAny<string>()))
                            .ReturnsAsync((List<AddressBookEntry>)null);
            _cacheHelperMock.Setup(c => c.SetCacheAsync(It.IsAny<string>(), It.IsAny<List<AddressBookEntry>>()))
                            .Returns(Task.CompletedTask);
            _cacheHelperMock.Setup(c => c.RemoveCacheAsync(It.IsAny<string>()))
                            .Returns(Task.CompletedTask);

            _addressBookRL = new AddressBookRL(_context, _cacheHelperMock.Object);

            _context.AddressBooks.AddRange(new List<AddressBookEntry>
            {
                new AddressBookEntry { Id = 1, UserId = 1, Name = "John Doe", Email = "john@example.com", Phone = "1234567890", Address = "123 Street" },
                new AddressBookEntry { Id = 2, UserId = 1, Name = "Jane Doe", Email = "jane@example.com", Phone = "0987654321", Address = "456 Avenue" }
            });
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public void test_GetAllContacts()
        {
            int userId = 1;

            _cacheHelperMock.Setup(c => c.GetCacheAsync<List<AddressBookEntry>>($"addressbook_{userId}"))
                .ReturnsAsync((List<AddressBookEntry>)null);

            var result = _addressBookRL.GetAllContacts(userId);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public void test_GetById()
        {
            int userId = 1, contactId = 1;

            _cacheHelperMock.Setup(c => c.GetCacheAsync<AddressBookEntry>($"addressbook_{userId}_{contactId}"))
                .ReturnsAsync((AddressBookEntry)null);

            var result = _addressBookRL.GetById(contactId, userId);

            Assert.IsNotNull(result);
            Assert.AreEqual("John Doe", result.Name);
        }

        [Test]
        public void test_AddContact()
        {
            var newContact = new AddressBookEntry { Id = 3, Name = "Alice", Email = "alice@example.com", Phone = "5554443322", Address = "789 Road" };
            int userId = 1;

            _cacheHelperMock.Setup(c => c.RemoveCacheAsync($"addressbook_user_{userId}")).Returns(Task.CompletedTask);
            _cacheHelperMock.Setup(c => c.SetCacheAsync($"addressbook_contact_{newContact.Id}", newContact)).Returns(Task.CompletedTask);

            var result = _addressBookRL.AddContact(newContact, userId);

            Assert.IsNotNull(result);
            Assert.AreEqual("Alice", result.Name);
            Assert.AreEqual(3, _context.AddressBooks.Count());
        }

        [Test]
        public void test_UpdateContact()
        {
            int userId = 1, contactId = 1;
            var updatedContact = new AddressBookEntry { Name = "John Updated", Email = "john_updated@example.com", Phone = "1231231234", Address = "New Address" };

            _cacheHelperMock.Setup(c => c.RemoveCacheAsync($"addressbook_user_{userId}")).Returns(Task.CompletedTask);
            _cacheHelperMock.Setup(c => c.SetCacheAsync($"addressbook_user{userId}_contact{contactId}", updatedContact)).Returns(Task.CompletedTask);

            var result = _addressBookRL.UpdateContact(contactId, updatedContact, userId);

            Assert.IsNotNull(result);
            Assert.AreEqual("John Updated", result.Name);
            Assert.AreEqual("john_updated@example.com", result.Email);
        }

        [Test]
        public void test_DeleteContact()
        {
            int userId = 1, contactId = 1;

            _cacheHelperMock.Setup(c => c.RemoveCacheAsync($"addressbook_user{userId}_contact{contactId}")).Returns(Task.CompletedTask);
            _cacheHelperMock.Setup(c => c.RemoveCacheAsync($"addressbook_user_{userId}")).Returns(Task.CompletedTask);

            var result = _addressBookRL.DeleteContact(contactId, userId);

            Assert.IsTrue(result);
            Assert.AreEqual(1, _context.AddressBooks.Count());
        }

        [Test]
        public void test_DeleteContactAsAdmin()
        {
            int contactId = 1;

            var result = _addressBookRL.DeleteContactAsAdmin(contactId);

            Assert.IsTrue(result);
            Assert.AreEqual(1, _context.AddressBooks.Count());
        }
    }
}
