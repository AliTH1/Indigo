namespace Indigo.Areas.Admin.ViewModels
{
    public class PaginationVM<T> where T : class, new()
    {
        public ICollection<T> Data { get; set; }
        public int CurrentPage { get; set; }
        public bool HasPrevious { get; set; }
        public bool HasNext { get; set; }
        public int PageCount { get; set; }
        public int Take { get; set; }
    }
}
