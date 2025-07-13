namespace PerRead_Server.DTOs
{
    public class PublisherCreateDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Website { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }
}
