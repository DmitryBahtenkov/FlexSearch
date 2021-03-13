namespace Core.Models.Search
{
    public class OrSearchModels : ISearchModel
    {
        public QueryType QueryType => QueryType.Or;
        public SearchModel[] Searches { get; set; }
    }
}