namespace aspnet_todolist.Models
{
    /// <summary>
    /// Represents a paginated result with metadata.
    /// </summary>
    /// <typeparam name="T">The type of items in the result.</typeparam>
    public record PagedResult<T>
    {
        /// <summary>
        /// The items for the current page.
        /// </summary>
        public required IEnumerable<T> Items { get; init; }

        /// <summary>
        /// The total number of items across all pages.
        /// </summary>
        public required int TotalCount { get; init; }

        /// <summary>
        /// The current page number (1-based).
        /// </summary>
        public required int CurrentPage { get; init; }

        /// <summary>
        /// The number of items per page.
        /// </summary>
        public required int PageSize { get; init; }

        /// <summary>
        /// The total number of pages.
        /// </summary>
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        /// <summary>
        /// Indicates whether there is a previous page.
        /// </summary>
        public bool HasPreviousPage => CurrentPage > 1;

        /// <summary>
        /// Indicates whether there is a next page.
        /// </summary>
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
