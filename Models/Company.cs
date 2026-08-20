namespace JobTracker.Models
{
  public class Company
  {
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Website { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
  }
}