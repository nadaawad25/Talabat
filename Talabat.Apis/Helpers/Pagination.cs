namespace Talabat.Apis.Helpers
{
    public class Pagination<T>
    {
        public Pagination(int pageSize, int pageIndex, IReadOnlyList<T> data ,int count)
        {

            Data = data;
            Count = count;
            PageSize = pageSize;
            PageIndex = pageIndex;

        }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public int Count { get; set; }
        public IReadOnlyList<T>  Data { get; set; }

    }
}
