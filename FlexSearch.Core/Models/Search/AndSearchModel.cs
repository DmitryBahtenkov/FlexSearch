namespace Core.Models.Search
{
    public class AndSearchModel : ISearchModel
    {
        public QueryType QueryType => QueryType.And;
        public SearchModel[] Searches { get; set; }
    }
}