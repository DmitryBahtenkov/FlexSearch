namespace Core.Storage.Serializers
{
    public interface ISerializer<T>
    {
        byte[] Serialize(T value);

        T Deserialize(byte[] buffer, int offset, int length);

        bool IsFixedSize { get; }

        int Length { get; }
    }
}