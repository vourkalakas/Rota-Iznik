using System;
using System.Runtime.Serialization;

namespace Core
{
    public class Hata : Exception
    {
        public Hata()
        {
        }
        public Hata(string message)
            : base(message)
        {
        }
        public Hata(string messageFormat, params object[] args)
            : base(string.Format(messageFormat, args))
        {
        }
        protected Hata(SerializationInfo
            info, StreamingContext context)
            : base(info, context)
        {
        }
        public Hata(string message, Hata innerException)
            : base(message, innerException)
        {
        }
    }
}
