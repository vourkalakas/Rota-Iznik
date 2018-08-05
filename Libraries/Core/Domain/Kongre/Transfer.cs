namespace Core.Domain.Kongre
{
    public partial class Transfer : TemelVarlık
    {
        public int KongreId { get; set; }
        public string TransferAdı { get; set; }
        public string TransferUcreti { get; set; }
        public int TransferUcretiDoviz { get; set; }
        public string TransferAracı { get; set; }
        public string TransferNotu { get; set; }
    }

}
