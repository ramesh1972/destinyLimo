using DestinyLimoServer.Models;

namespace DestinyLimoServer.DTOs.ResponseDTOs
{
    public class UserProfileDTO
    {
        public int ProfileId { get; set; }           // Maps to `profile_id`
        public int UserId { get; set; }              // Maps to `user_id`
        public string? Username { get; set; }        // Maps to `username`
        
        public required string FirstName { get; set; }        // Maps to `first_name`
        public string? LastName { get; set; }         // Maps to `last_name`
        public string? Avatar { get; set; }           // Maps to `avatar` (BLOB data type)
        public string? Address { get; set; }          // Maps to `address`
        public string? Email { get; set; }            // Maps to `email`
        public string? PhoneNumber { get; set; }      // Maps to `phone_number`
        public DateTime? Dob { get; set; }           // Maps to `dob` (nullable DateTime)
        public string? LicenseNumber { get; set; }    // Maps to `license_number`
        public DateTime? LicenseIssueDate { get; set; } // Maps to `license_issue_date` (nullable DateTime)
        public DateTime? LicenseExpiryDate { get; set; } // Maps to `license_expiry_date` (nullable DateTime)
        public DateTime CreatedAt { get; set; }      // Maps to `created_at`
        public DateTime? UpdatedAt { get; set; }     // Maps to `updated_at` (nullable DateTime)

        public IEnumerable<RoleDTO>? Roles { get; set; }           // Maps to `role`
    }
}