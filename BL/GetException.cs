using System;
using System.Runtime.Serialization;

namespace BO
{
    [Serializable]
    public class GetException : Exception
    {
        public GetException()
        {
        }

        public GetException(string message) : base(message)
        {
        }

        public GetException(string message, Exception innerException) : base(message, innerException)
        {
        }

      

        protected GetException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}