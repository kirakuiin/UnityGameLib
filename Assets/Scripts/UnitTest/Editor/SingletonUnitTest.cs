using NUnit.Framework;
using GameLib.Common;

namespace UnitTest.Editor
{
    
    [TestFixture]
    public class SingletonUnitTest
    {
        class TestSingleton : Singleton<TestSingleton>
        {
            public static readonly string DefaultName = nameof(TestSingleton);
            public string Name = DefaultName;
        }

        public void Set()
        {
            
        }

        [SetUp]
        public void Setup()
        {
            TestSingleton.Instance.Name = "hello";
        }

        [Test]
        public void TestCreate()
        {
            TestSingleton.Create();
            
            Assert.AreEqual(TestSingleton.DefaultName, TestSingleton.Instance.Name);
        }

        [Test]
        public void TestDestroy()
        {
            TestSingleton.Destroy();
        }

        [Test]
        public void TestIsInitialized()
        {
            Assert.IsTrue(TestSingleton.Instance.IsInitialized());
        }
    }
}