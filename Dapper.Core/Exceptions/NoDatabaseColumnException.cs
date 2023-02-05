using System.Runtime.Serialization;

namespace Dapper.Core.Exceptions;

[Serializable]
public class NoDatabaseColumnException : Exception
{
    public NoDatabaseColumnException(string message):base(message)
    {
    }

    protected NoDatabaseColumnException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(
        serializationInfo, streamingContext)
    {
    }
}