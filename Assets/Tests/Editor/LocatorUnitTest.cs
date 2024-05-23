using GameLib.Common;
using GameLib.Common.Pattern;
using NUnit.Framework;

namespace Tests.Editor
{
    [TestFixture]
    public class LocatorUnitTest
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            ServiceLocator.Instance.Register(new TestService1());
            ServiceLocator.Instance.Register(new TestService2());
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            ServiceLocator.Instance.UnRegister<TestService1>();
            ServiceLocator.Instance.UnRegister<TestService2>();
        }

        [Test]
        public void TestAddService()
        {
            ServiceLocator.Instance.Register(new TestService3());

            var service = ServiceLocator.Instance.Get<TestService3>();
            
            Assert.AreEqual(nameof(TestService3), service.TestStr);
            ServiceLocator.Instance.UnRegister<TestService3>();
        }

        [Test]
        public void TestGetService()
        {
            var service1 = ServiceLocator.Instance.Get<TestService1>();
            var service2 = ServiceLocator.Instance.Get<TestService2>();
            
            Assert.AreEqual(nameof(TestService1), service1.TestStr);
            Assert.AreEqual(nameof(TestService2), service2.TestStr);
        }

        [Test]
        public void TestGetNotExist()
        {
            Assert.Throws<InvalidOperationException>(
                () =>
                {
                    var service = ServiceLocator.Instance.Get<TestService4>();
                }
            );
        }
        
        [Test]
        public void TestUnRegister()
        {
            var service = ServiceLocator.Instance.Get<TestService1>();
            Assert.AreEqual(nameof(TestService1), service.TestStr);
            
            ServiceLocator.Instance.UnRegister<TestService1>();
            
            Assert.Throws<InvalidOperationException>(
                () =>
                {
                    ServiceLocator.Instance.Get<TestService1>();
                }
            );
            
            ServiceLocator.Instance.Register<TestService1>(new TestService1());
        }
    }

    public class TestService1 : IGameService
    {
        public string TestStr => nameof(TestService1);
    }
    
    public class TestService2 : IGameService
    {
        public string TestStr => nameof(TestService2);
    }
    
    public class TestService3 : IGameService
    {
        public string TestStr => nameof(TestService3);
    }
    
    public class TestService4 : IGameService
    {
        public string TestStr => nameof(TestService4);
    }
}