using System.ComponentModel.DataAnnotations;

namespace football_prediction_MVC.Models;

public class UserFactHistory
{
    public long Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public int FactId { get; set; }

    public DateTime ShownAt { get; set; }
}
