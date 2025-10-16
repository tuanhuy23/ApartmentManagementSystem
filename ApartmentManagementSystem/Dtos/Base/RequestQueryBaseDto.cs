namespace ApartmentManagementSystem.Dtos.Base
{
    public class RequestQueryBaseDto
    {
        public string FilterQueries { get; set; }
        public string SortQueries { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
    public class SortQuery
    {
        public string Code { get; set; }
        public SortDirection Direction { get; set; }
    }
    public enum SortDirection
    {
        Ascending,
        Descending
    }
    public class FilterQuery
    {
        public string Code { get; set; }
        public FilterOperator Operator { get; set; }
        public dynamic Value { get; set; }
    }
    public enum FilterOperator
    {
        Equals,
        In,
        NotEquals,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        Contains,
        StartsWith,
        EndsWith
    }
}
