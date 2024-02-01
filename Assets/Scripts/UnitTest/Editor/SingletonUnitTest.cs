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
        
        [Test]
        public void TestInstance()
        {
            CheckIsDefault();
        }

        void CheckIsDefault()
        {
            if (TestSingleton.Instance.Name != TestSingleton.DefaultName)
            {
                throw new UnitTestException("单例初始化错误");
            }
        }

        [Test]
        public void TestCreate()
        {
            TestSingleton.Instance.Name = "hello";
            TestSingleton.Create();
            CheckIsDefault();
        }

        [Test]
        public void TestDestroy()
        {
            TestSingleton.Instance.Name = "hello";
            TestSingleton.Destroy();
        }

        [Test]
        public void TestIsInitialized()
        {
            if (!TestSingleton.Instance.IsInitialized())
            {
                throw new UnitTestException("单例未初始化");
            }
        }
    }
}