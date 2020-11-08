﻿namespace Core.Models
{
    public class IndexModel
    {
        public string DatabaseName { get; }
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