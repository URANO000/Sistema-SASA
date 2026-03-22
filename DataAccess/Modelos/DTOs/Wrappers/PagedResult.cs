namespace DataAccess.Modelos.DTOs.Wrappers
{
    public class PagedResult<T>
    {
        public IReadOnlyList<T>? Items { get; set; }

        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }

        // Nuevos: para que Web no “adivine” valores
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}