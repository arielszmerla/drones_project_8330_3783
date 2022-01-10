using System;
using System.Runtime.Serialization;

namespace BL
{
    [Serializable]
    internal class BadStatusException : Exception
    {
        public BadStatusException()
        {
        }

        public BadStatusException(string message) : base(message)
        {
        }

        public BadStatusException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BadStatusException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}