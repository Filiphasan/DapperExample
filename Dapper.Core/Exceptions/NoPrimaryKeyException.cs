using System.Runtime.Serialization;

namespace Dapper.Core.Exceptions;

[Serializable]
public class NoPrimaryKeyException : Exception
{
    public NoPrimaryKeyException(string message) : base(message)
    {
    }

    protected NoPrimaryKeyException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(
        serializationInfo, streamingContext)
    {
    }
}