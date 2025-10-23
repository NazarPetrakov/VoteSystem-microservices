using Common.Pagination;

namespace Common.Contracts.Params;

public class QueryParams : PaginationParams
{
    public string? OrderBy { get; set; }
    public string? SearchTerm { get; set; }
}
