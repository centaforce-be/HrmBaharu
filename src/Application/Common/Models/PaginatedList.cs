namespace HrmBaharu.Application.Common.Models;

public class PaginatedList<T>
{
    public IReadOnlyCollection<T> Items { get; }
    public int PageNumber { get; }
    public int TotalPages { get; }
    public int TotalCount { get; }

    public PaginatedList(IReadOnlyCollection<T> items, int count, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
        Items = items;
    }

    public bool HasPreviousPage => PageNumber > 1;

    public bool HasNextPage => PageNumber < TotalPages;

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize, string? sortColumn = null, string? sortDirection = null)
    {
        //ASC & DESC
        if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortDirection))
        {
            if (string.Equals(sortDirection, "ASC", StringComparison.OrdinalIgnoreCase))
            {
                source = source.OrderBy(x => EF.Property<object>(x!, sortColumn));
            }
            else if (string.Equals(sortDirection, "DESC", StringComparison.OrdinalIgnoreCase))
            {
                source = source.OrderByDescending(x => EF.Property<object>(x!, sortColumn));
            }
        }
        var count = await source.CountAsync();
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }
}
