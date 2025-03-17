using NUnit.Framework;
using Moq;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ModelLayer.DTO;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Helper;
using RepositoryLayer.Service;
using RepositoryLayer.Hashing;
using RepositoryLayer.Interface;

namespace AddressBook.Tests.Services
{
    [TestFixture]
    public class UserRLTests
    {
        private AddressBookContext _context;
        private UserRL _userRL;
        private Mock<IJwtHelper> _jwtHelperMock;
        private Mock<IResetTokenHelper> _resetTokenHelperMock;
        private Mock<IConfiguration> _configurationMock;
        private Mock<IEmailService> _emailServiceMock;
        private Mock<IRedisCacheHelper> _cacheHelperMock;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AddressBookContext>()
                .UseInMemoryDatabase(databaseName: "UserDB")
                .Options;

            _context = new AddressBookContext(options);

            _jwtHelperMock = new Mock<IJwtHelper>();
            _resetTokenHelperMock = new Mock<IResetTokenHelper>();
            _configurationMock = new Mock<IConfiguration>();
            _emailServiceMock = new Mock<IEmailService>();
            _cacheHelperMock = new Mock<IRedisCacheHelper>();

            _userRL = new UserRL(
                _context,
                _jwtHelperMock.Object,
                _resetTokenHelperMock.Object,
                _configurationMock.Object,
                _emailServiceMock.Object,
                _cacheHelperMock.Object
            );

            _context.Users.Add(new UserEntry
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                PasswordHash = new Password_Hash().HashPassword("password123"),
                Role = "User"
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
        public void test_Register_ShouldCreateNewUser()
        {
            var userDto = new UserDTO
            {
                FirstName = "Alice",
                LastName = "Smith",
                Email = "alice@example.com",
                Password = "securepass",
                Role = "User"
            };

            var result = _userRL.Register(userDto);

            Assert.IsNotNull(result);
            Assert.That(_context.Users.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task test_Login_ShouldReturnToken()
        {
            var loginDto = new LoginDTO
            {
                Email = "john@example.com",
                Password = "password123"
            };

            _jwtHelperMock.Setup(j => j.GenerateToken(It.IsAny<string>(), It.IsAny<string>()))
                .Returns("mocked_token");

            var result = await _userRL.Login(loginDto);

            Assert.IsNotNull(result);
            Assert.That(result, Is.EqualTo("mocked_token"));
        }

        [Test]
        public async Task test_Login_ShouldReturnNullForInvalidPassword()
        {
            var loginDto = new LoginDTO
            {
                Email = "john@example.com",
                Password = "wrongpassword"
            };

            var result = await _userRL.Login(loginDto);

            Assert.IsNull(result);
        }

        [Test]
        public void test_GetUserIdByEmail_ShouldReturnUserId()
        {
            var result = _userRL.GetUserIdByEmail("john@example.com");

            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void test_GetUserIdByEmail_ShouldReturnZeroForNonExistingUser()
        {
            var result = _userRL.GetUserIdByEmail("nonexistent@example.com");

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void test_GetUserRoleByEmail_ShouldReturnUserRole()
        {
            var result = _userRL.GetUserRoleByEmail("john@example.com");

            Assert.That(result, Is.EqualTo("User"));
        }

        [Test]
        public void test_GetUserRoleByEmail_ShouldReturnDefaultRoleForNonExistingUser()
        {
            var result = _userRL.GetUserRoleByEmail("unknown@example.com");

            Assert.That(result, Is.EqualTo("User"));
        }

        [Test]
        public void test_ForgotPassword_ShouldReturnTrueForExistingUser()
        {
            _resetTokenHelperMock.Setup(r => r.GeneratePasswordResetToken(It.IsAny<int>(), It.IsAny<string>()))
                .Returns("mocked_token");

            _configurationMock.Setup(c => c["Application:BaseUrl"])
                .Returns("http://localhost:5000");

            _emailServiceMock.Setup(e => e.SendPasswordResetEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            var result = _userRL.ForgotPassword("john@example.com");

            Assert.IsTrue(result);
        }

        [Test]
        public void test_ForgotPassword_ShouldReturnFalseForNonExistingUser()
        {
            var result = _userRL.ForgotPassword("unknown@example.com");

            Assert.That(result, Is.False);
        }

        [Test]
        public void test_ResetPassword_ShouldReturnTrueForValidToken()
        {
            _resetTokenHelperMock.Setup(r => r.ValidatePasswordResetToken(It.IsAny<string>(), out It.Ref<string>.IsAny))
                .Returns((string token, out string email) =>
                {
                    email = "john@example.com";
                    return true;
                });


            var result = _userRL.ResetPassword("valid_token", "newpassword123", "newpassword123");

            Assert.IsTrue(result);
        }

        [Test]
        public void test_ResetPassword_ShouldReturnFalseForInvalidToken()
        {
            _resetTokenHelperMock.Setup(r => r.ValidatePasswordResetToken(It.IsAny<string>(), out It.Ref<string>.IsAny))
                .Returns(false);

            var result = _userRL.ResetPassword("invalid_token", "newpassword123", "newpassword123");

            Assert.That(result, Is.False);
        }

        [Test]
        public void test_ResetPassword_ShouldReturnFalseForMismatchedPasswords()
        {
            var result = _userRL.ResetPassword("valid_token", "password1", "password2");

            Assert.That(result, Is.False);
        }

        [Test]
        public void test_GetUserProfile_ShouldReturnUserProfile()
        {
            var result = _userRL.GetUserProfile("john@example.com");

            Assert.IsNotNull(result);
            Assert.That(result.FirstName, Is.EqualTo("John"));
            Assert.That(result.Email, Is.EqualTo("john@example.com"));
        }

        [Test]
        public void test_GetUserProfile_ShouldReturnNullForNonExistingUser()
        {
            var result = _userRL.GetUserProfile("unknown@example.com");

            Assert.IsNull(result);
        }
    }
}
