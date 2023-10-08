using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
