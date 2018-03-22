using Mediporta.KsKoSample.Tests.Factories;
using NUnit.Framework;
using System;
using System.Net;
using FluentAssertions;

namespace Mediporta.KsKoSample.Tests
{
    public class PermissionTests
    {
        private TestClientFactory testClientFactory;

        [SetUp]
        public void Setup()
        {
            testClientFactory = new TestClientFactory();
        }

        [Test]
        public void It_cannot_create_user()
        {
            Action action = () => testClientFactory.GetTestClient().CreateUser("testuser", "dGVzdHBhc3N3b3Jk");

            action
                .Should()
                .Throw<AggregateException>()
                .WithInnerException<WebException>();
        }

        [Test]
        public void It_cannot_create_user_group()
        {
            Action action = () => testClientFactory.GetTestClient().CreateGroup("testgroup");

            action
                .Should()
                .Throw<AggregateException>()
                .WithInnerException<WebException>();
        }

        [Test]
        public void It_cannot_search_users()
        {
            Action action = () => testClientFactory.GetTestClient().SearchUsers("test");

            action
                .Should()
                .Throw<AggregateException>()
                .WithInnerException<WebException>();
        }
    }
}
