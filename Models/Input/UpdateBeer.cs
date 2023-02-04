using System.ComponentModel.DataAnnotations;

namespace Models.Input
{
    public class UpdateBeer
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
