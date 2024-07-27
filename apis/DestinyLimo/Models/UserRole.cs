namespace DestinyLimo.Models
{
    public class UserRole
    {
        public int UserRoleId { get; set; }
        public required User UserObj { get; set; }
        public required Role RoleObj { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}