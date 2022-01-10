using System;
using System.Runtime.Serialization;

namespace BLAPI
{
    [Serializable]
    public class BLConfigException : Exception
    {
        public BLConfigException()
        {
        }

        public BLConfigException(string message) : base(message)
        {
        }

        public BLConfigException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BLConfigException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}