using GameLib.Common.DataStructure;
using NUnit.Framework;

namespace Tests.Editor
{
    [TestFixture]
    public class DefaultDictUnitTest
    {
        private DefaultDict<string, int> _dict;

        private const int Default = 10;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _dict = new DefaultDict<string, int>(() => Default);
        }
        
        [TearDown]
        public void TearDown()
        {
            _dict.Clear();
        }

        [Test]
        public void TestGet()
        {
            Assert.AreEqual(_dict["a"], Default);
        }

        [Test]
        public void TestAdd()
        {
            _dict.Add("a", 3);
            
            Assert.AreEqual(_dict["a"], 3);
        }

        [Test]
        public void TestExists()
        {
            _dict["a"] = 1;
            
            Assert.IsFalse(_dict.ContainsKey("b"));
            Assert.IsTrue(_dict.ContainsKey("a"));
        }

        [Test]
        public void TestClear()
        {
            _dict["a"] = 1;
            
            _dict.Clear();
            
            Assert.IsFalse(_dict.ContainsKey("a"));
        }

        [Test]
        public void TestEnumerator()
        {
            _dict["a"] = 0;
            _dict["b"] = 0;

            foreach (var pair in _dict)
            {
                Assert.AreEqual(pair.Value, _dict[pair.Key]);
            }
        }
    }
}