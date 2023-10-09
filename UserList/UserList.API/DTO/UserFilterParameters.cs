namespace UserList.API.DTO
{
    public class UserFilterParameters
    {
        public string? SortBy { get; set; }

        // по умолчанию сортировка по возрастанию
        public bool? Ascending { get; set; } 

        public string? FilterBy { get; set; }

        public string? FilterValue { get; set; } 

        public int? RoleId { get; set; }

        public string? RoleName { get; set;}
    }
}
