using System;

namespace GameLib.Network.NGO
{
    public class NgoException : Exception
    {
        public NgoException(string msg) : base(msg) {}
    }
}