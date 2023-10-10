using System.ComponentModel.DataAnnotations;


namespace UserList.Domain.Entities
{
    public class User
    {

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Значение должно быть больше 0")]
        public int Age { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }


        public ICollection<Role> Roles { get; set; } = new List<Role>();
    }
}
