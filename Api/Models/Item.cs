using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class Item
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [SwaggerSchema(ReadOnly = true)]
        public int Id { get; set; }

        public int ProductItemId { get; set; }

        public required string Name { get; set; }

        public float Price { get; set; }

        [NotMapped]
        public IFormFile? File { get; set; }

        [SwaggerSchema(ReadOnly = true)]
        public string? ImageUrl { get; set; }

        public bool IsFullPrice { get; set; }
    }
}
