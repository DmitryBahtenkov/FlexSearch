namespace Core.Models.Search
{
    public interface ISearchModel
    {
        public QueryType QueryType { get; }
        public SearchModel[] Searches { get; set; }
    }
}