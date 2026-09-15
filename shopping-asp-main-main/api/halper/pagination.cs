namespace api.halper
{
    public class pagination<T> where T : class
    {
        public pagination(int pagenumber, int pagesize, int totalpage, IReadOnlyList<T> data)
        {
            this.pagenumber = pagenumber;
            this.pagesize = pagesize;
            this.totalpage = totalpage;
            Data = data;
        }

        public int pagenumber { get; set; }
        public int pagesize { get; set; }
        public int totalpage { get; set; }
        public IReadOnlyList<T> Data { get; set; }
    }
}
