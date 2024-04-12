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

        [Test]
        public void TestUpdate()
        {
            _dict["a"] = 3;
            var newCounter = new Counter<string>
            {
                ["b"] = 4
            };

            _dict.Update(newCounter);
            
            Assert.AreEqual(4, _dict["b"]);
            Assert.AreEqual(2, _dict.Count());
        }

        [Test]
        public void TestSub()
        {
            _dict["a"] = 3;
            var newCounter = new Counter<string>
            {
                ["b"] = 4,
                ["a"] = 1
            };

            _dict.Subtract(newCounter);
            
            Assert.AreEqual(2, _dict["a"]);
            Assert.AreEqual(2, _dict.Count());
        }
        

        [Test]
        public void TestSetOp()
        {
            _dict["a"] = 3;
            var newCounter = new Counter<string>
            {
                ["b"] = 4,
                ["a"] = 1
            };

            var c1 = _dict + newCounter;
            var c2 = _dict - newCounter;
            
            Assert.AreEqual(2, c1.Count);
            Assert.AreEqual(c1["a"], c1["b"]);
            Assert.AreEqual(1, c2.Count);
            Assert.AreEqual(2, c2["a"]);
        }
        
        [Test]
        public void TestCompare()
        {
            _dict = new Counter<string>
            {
                ["b"] = 6
            };
            var newCounter = new Counter<string>
            {
                ["b"] = 4,
                ["a"] = -1
            };

            Assert.IsTrue(_dict > newCounter);
            Assert.IsTrue(newCounter < _dict);
            Assert.AreEqual(1, _dict.Count);
            Assert.AreEqual(2, newCounter.Count);
        }
    }
}