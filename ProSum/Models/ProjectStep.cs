namespace ProSum.Models
{
    public class ProjectStep
    {
       // public Guid ProjectId { get; set; }
        public Step Step { get; set; }
        public Step.Status Status { get; set; }
    }
}
