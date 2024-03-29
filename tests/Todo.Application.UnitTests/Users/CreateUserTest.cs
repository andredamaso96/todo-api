﻿using FluentAssertions;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Application.Repositories;
using Todo.Application.UseCases.Users.CreateUser;
using Todo.Application.Exceptions;
using Todo.Domain.Enums;
using Todo.Infra.Repositories;

namespace Todo.Application.UnitTests.Users
{
    [TestFixture]
    public class CreateUserTest : TestBase
    {
        private UserRepository _userRepository;
        private PasswordHasher _passwordHasher;

        [SetUp]
        public void SetUp()
        {
            _userRepository = new UserRepository(Context);
            _passwordHasher = new PasswordHasher();

            Environment.SetEnvironmentVariable("JWT_SECRET", "this is my custom Secret key for authentication");
            Environment.SetEnvironmentVariable("JWT_ISSUER", "LINCS");

        }

        [Test]
        [TestCase("andre@gmail.com", Roles.Administrator)]
        [TestCase("pedro@gmail.com", Roles.Operator)]
        public async Task CreateUser1_UserNotExists_ReturnNewUserAsync(string email, Roles role)
        {
            // Arrange
            var name = "André"; 

            var command = new CreateUserCommand(){
                Name = name, 
                Email = email, 
                Password = "1234", 
                Roles = role
            };

            var handler = new CreateUserCommandHandler(_userRepository, _passwordHasher, Mapper);
            // Act
            var result = await handler.Handle(command, CancellationToken.None);
            var user = await _userRepository.GetUserByEmail(email);
            // Assert

            Assert.That(result.Name, Is.EqualTo(name));
            Assert.That(result.Name, Is.EqualTo(user.Name));
            Assert.That(result.Id, Is.EqualTo(user.Id));
            Assert.That(user.CreatedBy, Is.EqualTo("admin"));
            Assert.That(user.Role, Is.EqualTo(role));

        }

        [Test]
        public void CreateUser2_UserExists_ThrowApplicationDataExeption()
        {
            // Arrange
            var name = "André";
            var email = "andre@gmail.com";

            var command = new CreateUserCommand()
            {
                Name = name,
                Email = email,
                Password = "1234",
                Roles = Roles.Administrator
            };

            var handler = new CreateUserCommandHandler(_userRepository, _passwordHasher, Mapper);

            Assert.That(() => handler.Handle(command, CancellationToken.None), Throws.Exception.TypeOf<ApplicationException>());

        }

        [Test]
        public void CreateUser3_InvalidCommands_ReturnFalse()
        {
            var command = new CreateUserCommand();
           
            var handler = new CreateUserCommandHandler(_userRepository, _passwordHasher, Mapper);

            FluentActions.Invoking(() =>
                handler.Handle(command, CancellationToken.None)).Should().ThrowAsync<ValidationException>();

        }

    }
}
