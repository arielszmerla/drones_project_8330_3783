using System;
using System.Runtime.Serialization;

namespace DO
{
    [Serializable]
    public class XMLFileLoadCreateException : Exception
    {
        private string v;
        private Exception ex;

        public XMLFileLoadCreateException()
        {
        }

        public XMLFileLoadCreateException(string message) : base(message)
        {
        }

        public XMLFileLoadCreateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public XMLFileLoadCreateException(string message, string v, Exception ex) : this(message)
        {
            this.v = v;
            this.ex = ex;
        }

        protected XMLFileLoadCreateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}