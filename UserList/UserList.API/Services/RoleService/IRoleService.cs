namespace UserList.API.Services.RoleService
{
    public interface IRoleService
    {
        /// <summary>
        /// Проверка, существует ли объект с заданным id
        /// </summary>
        /// <param name="roleId">Id проверяемого объекта</param>
        /// <returns>True если объект с заданным id существует, false если не существует</returns>
        public Task<bool> DoesRoleExistAsync(int roleId);
    }
}
