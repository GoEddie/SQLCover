using System;
using System.Runtime.Serialization;

namespace SQLCover
{
    [Serializable]
    public class SqlCoverException : Exception
    {
        public SqlCoverException()
        {
        }

        public SqlCoverException(string message) : base(message)
        {
        }

        public SqlCoverException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SqlCoverException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}