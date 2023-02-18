using System.Runtime.Serialization;

namespace Dapper.Core.Exceptions;

[Serializable]
public class NoPrimaryKeyValueException : Exception
{
    public NoPrimaryKeyValueException(string message) : base(message)
    {
    }
    
    protected NoPrimaryKeyValueException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(
        serializationInfo, streamingContext)
    {
    }
}