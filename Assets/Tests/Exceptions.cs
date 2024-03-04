using System;

namespace Tests
{
    public class UnitTestException : Exception
    {
        public UnitTestException(string msg) : base(msg) {}
    }
}