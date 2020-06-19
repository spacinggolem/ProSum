namespace ProSum.Models
{
    public class StepPermission
    {
        public Step Step { get; set; }
        public enum PermissionEnum
        {
            None,
            Read,
            Write
        }
        public PermissionEnum Permission { get; set; }
    }
}
