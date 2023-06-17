using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

public class ChatMessage
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string SesssionId { get; set; }
    [Required]
    [MaxLength(1000)]
    public string? Content { get; set; }
    [Required]
    public Color SenderColor { get; set; }
    [Required]
    public DateTime Timestamp { get; set; }
}
