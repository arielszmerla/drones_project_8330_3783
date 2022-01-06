using System;
using System.Runtime.Serialization;

namespace BO
{
    [Serializable]
    public class BatteryException : Exception
    {
        public BatteryException()
        {
        }

        public BatteryException(string message) : base(message)
        {
        }

        public BatteryException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BatteryException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}