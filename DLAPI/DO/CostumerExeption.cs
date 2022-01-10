using System;
using System.Runtime.Serialization;

namespace DO

{
    [Serializable]
    public class CostumerExeption : Exception
    {
        public CostumerExeption()
        {
        }

        public CostumerExeption(string message) : base("Customer exeption: " + message)
        {
        }

        public CostumerExeption(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CostumerExeption(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}