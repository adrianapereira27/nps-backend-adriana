namespace nps_backend_adriana.Models.Entities
{
    public class NpsLog
    {
        public int Id { get; set; }
        public DateTime DateScore { get; set; }
        public Guid SystemId { get; set; }
        public int Score { get; set; }
        public string Description { get; set; }
        public Guid CategoryId { get; set; }
        public string UserId { get; set; }
    }
}
