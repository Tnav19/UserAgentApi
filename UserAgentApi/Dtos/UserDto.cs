using System.ComponentModel.DataAnnotations;

namespace UserAgentApi.Dtos
{
    public class UserDto
    {
        // Optional field for creation (in some cases, Id is not included)
        public int? Id { get; set; }

        // Required fields with validation
        [Required]
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Last name cannot be longer than 50 characters.")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string Phone { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Address cannot be longer than 100 characters.")]
        public string Address { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "City cannot be longer than 50 characters.")]
        public string City { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "State cannot be longer than 50 characters.")]
        public string State { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "Zip code cannot be longer than 10 characters.")]
        public string ZipCode { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public DateTime UpdatedDate { get; set; }

        // Optional field for related entities
        public int? AgentId { get; set; }

    }
}
