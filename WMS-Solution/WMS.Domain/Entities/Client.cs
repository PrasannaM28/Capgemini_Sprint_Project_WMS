using System.ComponentModel.DataAnnotations;
using WMS.Domain.Common;

namespace WMS.Domain.Entities;

public class Client : BaseEntity
{
    [Key]
    public int ClientId { get; set; }

    [Required]
    [StringLength(100)]
    public string ClientName { get; set; } = string.Empty;

    public string? ClientAddress { get; set; }

    [Phone]
    [StringLength(10)]
    public string? ClientPhoneNumber { get; set; }

    [StringLength(20)]
    public string? ClientLocation { get; set; }

    public bool Status { get; set; } = true;

    // Navigation Property
    public ICollection<Project> Projects { get; set; }
        = new List<Project>();
}
