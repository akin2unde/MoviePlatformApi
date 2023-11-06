
namespace MoviePlatformApi.Models
{
    public class PagedResult<T>
    {
        public int PageSize { get; set; }
        public int Skip { get; set; }
        public long Total { get; set; }
        public IEnumerable<T> Result { get; set; }
        public string LastItem { get; set; }
        public PagedResult()
        {

        }
    }
}