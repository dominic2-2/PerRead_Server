namespace PerRead_Server.DTOs
{
    public class AuthorUpdateDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? Email { get; set; }
        public int? BirthYear { get; set; }
        public int? DeathYear { get; set; }
    }
}
