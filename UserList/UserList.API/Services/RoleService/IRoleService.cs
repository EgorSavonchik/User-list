namespace UserList.API.Services.RoleService
{
    public interface IRoleService
    {
        /// <summary>
        /// Check if a role with a given id exists
        /// </summary>
        /// <param name="roleId">Id of the role to be checked</param>
        /// <returns>True if the role with the given id exists, false if it does not exist</returns>
        public Task<bool> DoesRoleExistAsync(int roleId);
    }
}
