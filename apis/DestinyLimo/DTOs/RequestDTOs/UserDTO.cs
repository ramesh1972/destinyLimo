namespace DestinyLimo.DTOs.RequestDTOs
{
    public class UserDTO
    {
        public int UserId { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public bool IsActive { get; set; }
    }
}
