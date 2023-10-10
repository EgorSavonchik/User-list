using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UserList.Domain.Entities
{
    public class Role
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set;}

        [JsonIgnore]
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
