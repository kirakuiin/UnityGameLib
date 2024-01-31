using System;
using NUnit.Framework;
using GameLib.Common;
using NUnit.Framework.Internal;

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
                throw new UnitTextException("单例初始化错误");
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
    }
}