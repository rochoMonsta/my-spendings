namespace MySpendings.Models
{
    public class UserOutlay
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int OutlayId { get; set; }
        public virtual Outlay Outlay { get; set; }
    }
}
