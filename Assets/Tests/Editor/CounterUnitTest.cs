using System.Collections.Generic;
using System.Linq;
using GameLib.Common.DataStructure;
using NUnit.Framework;

namespace Tests.Editor
{
    [TestFixture]
    public class CounterUnitTest 
    {
        private Counter<string> _dict;


        [SetUp]
        public void Setup()
        {
            _dict = new Counter<string>();
        }

        [Test]
        public void TestGet()
        {
            Assert.AreEqual(_dict["a"], 0);
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

        [Test]
        public void TestElements()
        {
            var wordList = new List<string>()
            {
                "z", "z", "z", "hello", "hello", "world"
            };
            _dict = new Counter<string>(wordList);
            
            Assert.AreEqual(wordList, _dict.Elements().ToList());
        }
        
        [Test]
        public void TestTotal()
        {
            var wordList = new List<string>()
            {
                "z", "z", "z", "hello", "hello", "world"
            };
            _dict = new Counter<string>(wordList);

            _dict["z"] += 10;
            
            Assert.AreEqual(16, _dict.Total());
        }
        
        [Test]
        public void TestMostCommon()
        {
            var wordList = new List<string>()
            {
                "z", "z", "z", "hello", "hello", "world", "world", "x"
            };
            _dict = new Counter<string>(wordList);

            Assert.AreEqual("z", _dict.MostCommon(1).First().Key);
            Assert.AreEqual("world", _dict.MostCommon(3).Last().Key);
            Assert.AreEqual("x", _dict.MostCommon().Last().Key);
        }
    }
}