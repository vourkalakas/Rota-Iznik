namespace Web.Framework.Kendoui
{
    public class DataSourceİsteği
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public DataSourceİsteği()
        {
            this.Page = 1;
            this.PageSize = 10;
        }
    }
}
