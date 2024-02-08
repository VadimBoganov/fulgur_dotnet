using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class ProductItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [SwaggerSchema(ReadOnly = true)]
        public int Id { get; set; }

        public int ProductSubTypeId { get; set; }

        public required string Name { get; set; }

        [NotMapped]
        public required IFormFile File { get; set; }

        [SwaggerSchema(ReadOnly = true)]
        public string? ImageUrl { get; set; }
    }
}
