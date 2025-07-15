using PerRead_Server.Models;

public class BookDetailDTO
{
    public string Id { get; set; }
    public string Title { get; set; }
    public List<Author> Authors { get; set; }
    public List<Category> Categories { get; set; }
    public Publisher Publisher { get; set; }
    public string Language { get; set; }
    public string Edition { get; set; }
    public string ISBN { get; set; }
    public int Pages { get; set; }
    public string Summary { get; set; }
    public string CoverUrl { get; set; }
    public string FileUrl { get; set; }
    public long FileSize { get; set; }
    public int Price { get; set; }
    public bool Availability { get; set; }
    public List<string> Tags { get; set; }
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}