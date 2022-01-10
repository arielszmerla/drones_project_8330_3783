using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
namespace DO
{
    [Serializable]
    public class DroneChargeException : Exception
    {
        public DroneChargeException()
        {
        }

        public DroneChargeException(string message) : base(message)
        {
        }

        public DroneChargeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DroneChargeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
