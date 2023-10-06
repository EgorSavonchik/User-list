using UserList.Domain.Entities;
using UserList.Domain.Models;

namespace UserList.API.Services.UserService
{
    public interface IUserService
    {
        /// <summary>
        /// Получение списка всех объектов
        /// </summary>
        /// <param name="pageNo">номер страницы списка</param>
        /// <param name="pageSize">количество объектов на странице</param>
        /// <returns></returns>
        public Task<ResponseData<ListModel<User>>> GetUserListAsync(int pageNo = 1, int pageSize = 10);

        /// <summary>
        /// Поиск объекта по Id
        /// </summary>
        /// <param name="id">Идентификатор объекта</param>
        /// <returns></returns>
        public Task<ResponseData<User>> GetUserByIdAsync(int id);

        /// <summary>
        /// Обновление объекта
        /// </summary>
        /// <param name="id">Id изменяемомго объекта</param>
        /// <param name="user">объект с новыми параметрами</param>
        /// <returns></returns>
        public Task UpdateUserAsync(int id, User user);

        /// <summary>
        /// Удаление объекта
        /// </summary>
        /// <param name="id">Id удаляемомго объекта</param>
        /// <returns></returns>
        public Task DeleteUserAsync(int id);

        /// <summary>
        /// Создание объекта
        /// </summary>
        /// <param name="user">Новый объект</param>
        /// <returns>Созданный объект</returns>
        public Task<ResponseData<User>> CreateUserAsync(User user);

        /// <summary>
        /// Проверка, существует ли объект с заданным id
        /// </summary>
        /// <param name="userId">Id проверяемого объекта</param>
        /// <returns>True если объект с заданным id существует, false если не существует</returns>
        public Task<bool> DoesUserExistAsync(int userId);

        /// <summary>
        /// Добавление роли пользователю с заданным id
        /// </summary>
        /// <param name="id">Id пользователя</param>
        /// <param name="role">Добавляемая роль</param>
        /// <returns></returns>
        public Task AddRoleAsync(int id, Role role);

        /// <summary>
        /// Проверка уникальности поля email
        /// </summary>
        /// <param name="email">Проверяемый email</param>
        /// <returns>True если в базе данных нету пользователей с таким же значением поля email, false если есть</returns>
        public Task<bool> IsEmailUnique(string email);
    }
}
