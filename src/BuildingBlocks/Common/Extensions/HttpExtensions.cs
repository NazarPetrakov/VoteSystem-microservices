using System.Text.Json;
using Common.Pagination;
using Microsoft.AspNetCore.Http;

namespace Common.Extensions;

public static class HttpExtensions
{
    public static void AddPaginationHeader<T>(this HttpResponse response, PagedList<T> data)
    {
        var metaData = new PaginationMetadata(data.PageNumber,
            data.PageSize, data.TotalCount, data.TotalPages, data.HasPreviousPage, data.HasNextPage);

        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        var jsonData = JsonSerializer.Serialize(metaData, jsonOptions);

        response.Headers.Append("Pagination", jsonData);
        response.Headers.Append("Access-Control-Expose-Headers", "Pagination");
    }
}
