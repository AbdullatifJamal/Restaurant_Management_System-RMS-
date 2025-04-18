namespace RMS_API.DTOs.AuthDTO.Request
{
    public class SignUpDTO
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int UserRoleId { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? Image { get; set; }
        public string CreatedBy { get; set; }
    }
}
