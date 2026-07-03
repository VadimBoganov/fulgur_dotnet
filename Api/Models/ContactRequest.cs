using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models
{
    public enum ContactRequestStatus
    {
        New,
        InProgress,
        Closed
    }

    public class ContactRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [SwaggerSchema(ReadOnly = true)]
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public required string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(200, MinimumLength = 3)]
        public required string Email { get; set; }

        [StringLength(30)]
        public string? Phone { get; set; }

        [Required]
        [StringLength(2000, MinimumLength = 1)]
        public required string Message { get; set; }

        [SwaggerSchema(ReadOnly = true)]
        public ContactRequestStatus Status { get; set; }

        [SwaggerSchema(ReadOnly = true)]
        public DateTime CreatedAt { get; set; }
    }
}