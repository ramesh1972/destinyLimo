namespace DestinyLimoServer.Models
{
    public class User
    {
        public int UserId { get; set; }
        public required string Username { get; set; }
        public string PasswordHash { get; set; } = String.Empty;
        public required string Email { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsApproved { get; set; } = false;
        public string ApproveRejectReason { get; set; } = String.Empty;
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
    }
}