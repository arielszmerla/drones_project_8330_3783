using System;
using System.Runtime.Serialization;

namespace DLAPI

{
    [Serializable]
    public class BaseExeption : Exception
    {
        public BaseExeption()
        {
        }

        public BaseExeption(string message) : base("Base station exeption: "+message)
        {
        }

        public BaseExeption(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BaseExeption(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}