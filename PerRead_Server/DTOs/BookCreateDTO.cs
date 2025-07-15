namespace PerRead_Server.DTOs
{
    public class BookCreateDTO
    {
        public string Title { get; set; } = string.Empty;
        public List<string> AuthorIds { get; set; } = new();
        public string PublisherId { get; set; } = string.Empty;
        public List<string> CategoryIds { get; set; } = new();
        public string Language { get; set; } = string.Empty;
        public string Edition { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int Pages { get; set; }
        public string Summary { get; set; } = string.Empty;
        public string CoverUrl { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public int Price { get; set; }
        public bool Availability { get; set; }
        public List<string> Tags { get; set; } = new();
    }
}
