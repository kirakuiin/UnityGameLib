using System;

namespace UnitTest
{
    public class UnitTestException : Exception
    {
        public UnitTestException(string msg) : base(msg) {}
    }
}