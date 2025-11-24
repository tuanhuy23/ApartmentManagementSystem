namespace ApartmentManagementSystem.Dtos.Base
{
    public class RequestQueryBaseDto<T>
    {
        public List<FilterQuery> Filters { get; set; }
        public List<SortQuery> Sorts { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public T Request {get;set;}
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
    public class Pagination<T> : List<T>
    {
        public int Totals { get; set; }
        public List<T> Items { get; set; }
    }
}
