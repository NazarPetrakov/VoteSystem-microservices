namespace Common.Pagination;

public class PaginationMetadata(int currentPage, int itemsPerPage, int totalItems,
    int totalPages, bool hasPreviousPage, bool hasNextPage)
{
    public int CurrentPage { get; set; } = currentPage;
    public int ItemsPerPage { get; set; } = itemsPerPage;
    public int TotalItems { get; set; } = totalItems;
    public int TotalPages { get; set; } = totalPages;
    public bool HasNextPage { get; set; } = hasNextPage;
    public bool HasPreviousPage { get; set; } = hasPreviousPage;
}
