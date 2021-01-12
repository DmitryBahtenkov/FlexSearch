using System;

namespace Core.Storage.BinaryStorage
{
    public interface IRecordStorage
    {
        /// <summary>
        /// Effectively update an record
        /// </summary>
        public void Update(uint recordId, byte[] data);

        /// <summary>
        /// Grab a record's data
        /// </summary>
        public byte[] Find(uint recordId);

        /// <summary>
        /// This creates new empty record
        /// </summary>
        public uint Create();

        /// <summary>
        /// This creates new record with given data and returns its ID
        /// </summary>
        public uint Create(byte[] data);

        /// <summary>
        /// Similar to Create(byte[] data), but with dataGenerator which generates
        /// data after a record is allocated
        /// </summary>
        public uint Create(Func<uint, byte[]> dataGenerator);

        /// <summary>
        /// This deletes a record by its id
        /// </summary>
        public void Delete(uint recordId);
    }
}