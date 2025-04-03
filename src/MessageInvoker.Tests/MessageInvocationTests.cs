using System;
using System.Linq;
using System.Linq.Expressions;
using MessageInvoker.Shared.Helpers;
using MessageInvoker.Shared.Messages;
using MessageInvoker.Shared.Services;
using MessageInvoker.Tests.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Assert = MessageInvoker.Tests.Services.Assert;

namespace MessageInvoker.Tests
{
    [TestFixture]
    public class MessageInvocationTests
    {
        private IFakeService _fakeService;
        private IServiceProvider _serviceProvider;
        private IMessageInvoker _messageInvoker;

        Expression<Action<IFakeService>> _expression;

        [SetUp]
        public void Setup()
        {
            _fakeService = new FakeService();

            var mockServiceContainer = new Mock<IServiceProvider>();
            var mockScope = new Mock<IServiceScope>();
            var mockScopeFactory = new Mock<IServiceScopeFactory>();

            mockServiceContainer.Setup(x => x.GetService(typeof(IFakeService))).Returns(_fakeService);
            mockScopeFactory.Setup(x => x.CreateScope()).Returns(mockScope.Object);
            mockScope.Setup(x => x.ServiceProvider).Returns(mockServiceContainer.Object);
            mockServiceContainer.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(mockScopeFactory.Object);

            _serviceProvider = mockServiceContainer.Object;
            _expression = service => service.StringParameterMethod("Success");
        }

        [Test]
        public void ShouldGetMethodName()
        {
            var methodName = ExpressionHelpers.GetMethodName(_expression);
            
            Assert.AreEqual(nameof(_fakeService.StringParameterMethod), methodName);
        }

        [Test]
        public void ShouldGetParameter()
        {
            var parameters = ExpressionHelpers.GetParameters(_expression);
            Assert.AreEqual("Success", parameters.First().Value);
        }

        [Test]
        public void ShouldGetCallers()
        {
            Expression<Action<IFakeService>> nestedExpression = x => x.Nested.StringParameterMethod("Success");

            var callers = ExpressionHelpers.GetCallers(nestedExpression);

            Assert.AreEqual("Nested", callers);
        }

        [Test]
        public void ShouldGetTargetType()
        {
            var methodName = ExpressionHelpers.GetMethodName(_expression);
            var callers = ExpressionHelpers.GetCallers(_expression);
            var parameters = ExpressionHelpers.GetParameters(_expression);

            var message = new InvocationMessage<IFakeService>(parameters, methodName, callers, "tag");

            Assert.AreEqual(typeof(IFakeService).ToString(), message.TargetType);
        }

        [Test]
        public void ShouldInvokeExpression()
        {
            var methodName = ExpressionHelpers.GetMethodName(_expression);
            var callers = ExpressionHelpers.GetCallers(_expression);
            var parameters = ExpressionHelpers.GetParameters(_expression);

            var message = new InvocationMessage<IFakeService>(parameters, methodName, callers, "tag");

            var messageInvoker = new MessageInvokerService(_serviceProvider);

            NUnit.Framework.Assert.DoesNotThrow(() => messageInvoker.Invoke(message));
        }

        [Test]
        public void StringExecutionForStringParameterTest()
        {
            var message = new InvocationMessage("MessageInvoker.Tests.Services.IFakeService", "StringParameterMethod", new object[] { "Success" });

            Assert.AreEqual(nameof(_fakeService.StringParameterMethod), message.MethodName);
            Assert.AreEqual("Success", message.Parameters.First().Value);
            Assert.AreEqual(typeof(IFakeService).ToString(), message.TargetType);

            var messageInvoker = new MessageInvokerService(_serviceProvider);

            NUnit.Framework.Assert.DoesNotThrow(() => messageInvoker.Invoke(message));
        }

        [Test]
        public void ExpressionExecutionForBooleanParameterTest()
        {
            var methodName = ExpressionHelpers.GetMethodName<IFakeService>(x => x.BoolParameterMethod(true));
            var callers = ExpressionHelpers.GetCallers<IFakeService>(x => x.BoolParameterMethod(true));
            var parameters = ExpressionHelpers.GetParameters<IFakeService>(x => x.BoolParameterMethod(true));

            var message = new InvocationMessage<IFakeService>(parameters, methodName, callers);

            Assert.AreEqual(nameof(_fakeService.BoolParameterMethod), message.MethodName);
            Assert.AreEqual(true, message.Parameters.First().Value);
            Assert.AreEqual(typeof(IFakeService).ToString(), message.TargetType);

            var messageInvoker = new MessageInvokerService(_serviceProvider);

            NUnit.Framework.Assert.DoesNotThrow(() => messageInvoker.Invoke(message));
        }

        [Test]
        public void StringExecutionForBooleanParameterTest()
        {
            var message = new InvocationMessage("MessageInvoker.Tests.Services.IFakeService", "BoolParameterMethod", new object[] { true });
            Assert.AreEqual(nameof(_fakeService.BoolParameterMethod), message.MethodName);
            Assert.AreEqual(true, message.Parameters.First().Value);
            Assert.AreEqual(typeof(IFakeService).ToString(), message.TargetType);

            var messageInvoker = new MessageInvokerService(_serviceProvider);

            NUnit.Framework.Assert.DoesNotThrow(() => messageInvoker.Invoke(message));
        }

        [Test]
        public void ExpressionExecutionForBooleanReturnedTest()
        {
            var methodName = ExpressionHelpers.GetMethodName<IFakeService>(x => x.BooleanReturnedMethod());
            var callers = ExpressionHelpers.GetCallers<IFakeService>(x => x.BooleanReturnedMethod());
            var parameters = ExpressionHelpers.GetParameters<IFakeService>(x => x.BooleanReturnedMethod());

            var message = new InvocationMessage<IFakeService>(parameters, methodName, callers);

            Assert.AreEqual(nameof(_fakeService.BooleanReturnedMethod), message.MethodName);
            Assert.AreEqual(null, message.Parameters.FirstOrDefault().Value ?? null);
            Assert.AreEqual(typeof(IFakeService).ToString(), message.TargetType);

            var messageInvoker = new MessageInvokerService(_serviceProvider);

            NUnit.Framework.Assert.DoesNotThrow(() => messageInvoker.Invoke(message));
        }

        [Test]
        public void StringExecutionForBooleanReturnedTest()
        {
            var message = new InvocationMessage("MessageInvoker.Tests.Services.IFakeService", "BooleanReturnedMethod", new object[] { });
            Assert.AreEqual(nameof(_fakeService.BooleanReturnedMethod), message.MethodName);
            Assert.AreEqual(null, message.Parameters.FirstOrDefault().Value);
            Assert.AreEqual(typeof(IFakeService).ToString(), message.TargetType);

            var messageInvoker = new MessageInvokerService(_serviceProvider);

            NUnit.Framework.Assert.DoesNotThrow(() => messageInvoker.Invoke(message));
        }

        [Test]
        public void ExpressionExecutionForNestedStringParameterTest()
        {
            var methodName = ExpressionHelpers.GetMethodName<IFakeService>(x => x.Nested.StringParameterMethod("Success"));
            var callers = ExpressionHelpers.GetCallers<IFakeService>(x => x.Nested.StringParameterMethod("Success"));
            var parameters = ExpressionHelpers.GetParameters<IFakeService>(x => x.Nested.StringParameterMethod("Success"));
            var message = new InvocationMessage<IFakeService>(parameters, methodName, callers);

            Assert.AreEqual(nameof(_fakeService.Nested.StringParameterMethod), message.MethodName);
            Assert.AreEqual("Success", message.Parameters.First().Value);
            Assert.AreEqual(typeof(IFakeService).ToString(), message.TargetType);

            var messageInvoker = new MessageInvokerService(_serviceProvider);

            NUnit.Framework.Assert.DoesNotThrow(() => messageInvoker.Invoke(message));
        }
    }
}