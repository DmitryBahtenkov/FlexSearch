namespace Core.Models.Search
{
    public class MultiSearchModel 
    {
        public QueryType QueryType { get; set; }
    
        public SearchModel[] Searches { get; set; }
    }
}