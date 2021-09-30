namespace Core.Models
{
    public record IndexModel
    {
        /// <summary>
        /// Название базы данных
        /// </summary>
        public string DatabaseName { get; }
        /// <summary>
        /// Название коллекции
        /// </summary>
        public string IndexName { get; }

        public IndexModel(string databaseName, string indexName)
        {
            DatabaseName = databaseName;
            IndexName = indexName;
        }

        public override string ToString()
        {
            return $"{DatabaseName}/{IndexName}";
        }
    }
}