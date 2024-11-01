namespace UserProfileService.Models
{
    public class UserProfile
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // Auto-generate GUID
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
