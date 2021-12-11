using System;
using System.Runtime.Serialization;

namespace DLAPI
{
    [Serializable]
    public class ParcelExeption : Exception
    {
        public ParcelExeption()
        {
        }

        public ParcelExeption(string message) : base("Parcel exeption: " +message)
        {
        }

        public ParcelExeption(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ParcelExeption(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}