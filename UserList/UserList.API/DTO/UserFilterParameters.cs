using Swashbuckle.AspNetCore.Annotations;

namespace UserList.API.DTO
{
    public class UserFilterParameters
    {
        [SwaggerParameter("The field by which the list is sorted.")]
        public string? SortBy { get; set; }

        [SwaggerParameter("Sorting order.")]
        public bool? Ascending { get; set; }

        [SwaggerParameter("The field by which filtering will be carried out.")]
        public string? FilterBy { get; set; }

        [SwaggerParameter("The value of the field by which filtering will be carried out.")]
        public string? FilterValue { get; set; }

        [SwaggerParameter("Id of the role by which filtering will be carried out.")]
        public int? RoleId { get; set; }

        [SwaggerParameter("Name of the role by which filtering will be carried out.")]
        public string? RoleName { get; set;}
    }
}
