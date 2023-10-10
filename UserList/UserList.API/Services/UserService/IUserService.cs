using System.Collections.Generic;
using UserList.API.DTO;
using UserList.Domain.Entities;
using UserList.Domain.Models;

namespace UserList.API.Services.UserService
{
    public interface IUserService
    {
        /// <summary>
        /// Getting a list of all objects
        /// </summary>
        /// <param name="pageNo">List page number</param>
        /// <param name="pageSize">Number of objects on page</param>
        /// <param name="parameters">Sampling parameters, contains
        /// sortBy - field by which sorting will take place
        /// Ascending - sort order
        /// filterBy - the field by which the filtering will take place
        /// folterValue - the value by which the filtering will take place
        /// roleId - role id for which filtering will be performed
        /// roleName - role name for which filtering will be performed</param>
        /// <returns>Page of users with specified sampling parameters</returns>
        public Task<ResponseData<ListModel<User>>> GetUserListAsync(int pageNo = 1, int pageSize = 10, UserFilterParameters? parameters = null);

        /// <summary>
        /// Search for an object by Id
        /// </summary>
        /// <param name="id">Object Identifier</param>
        /// <returns>User with a specified id</returns>
        public Task<ResponseData<User>> GetUserByIdAsync(int id);

        /// <summary>
        /// Updating an object
        /// </summary>
        /// <param name="id">Id of the object to be changed</param>
        /// <param name="user">Object with new parameters</param>
        /// <returns>Updated user</returns>
        public Task<ResponseData<User>> UpdateUserAsync(int id, User user);

        /// <summary>
        /// Deleting an object
        /// </summary>
        /// <param name="id">Id of the object to be deleted</param>
        /// <returns></returns>
        public Task DeleteUserAsync(int id);

        /// <summary>
        /// Object creation
        /// </summary>
        /// <param name="user">New object</param>
        /// <returns>Created object</returns>
        public Task<ResponseData<User>> CreateUserAsync(User user);

        /// <summary>
        /// Check if an object with the given id exists
        /// </summary>
        /// <param name="userId">Id of the inspected object</param>
        /// <returns>True if the object with the given id exists, false if it does not exist</returns>
        public Task<bool> DoesUserExistAsync(int userId);

        /// <summary>
        /// Adding a role to a user with a specified id
        /// </summary>
        /// <param name="id">User Id</param>
        /// <param name="role">Added role</param>
        /// <returns></returns>
        public Task AddRoleAsync(int id, Role role);

        /// <summary>
        /// Checking the uniqueness of the email field
        /// </summary>
        /// <param name="email">Verified email</param>
        /// <returns>True if there are no users with the same email field value in the database, false if there are</returns>
        public Task<bool> IsEmailUnique(string email);

        /// <summary>
        /// Retrieving a user by email field
        /// </summary>
        /// <param name="email">User email</param>
        /// <returns>Returns the user with the given email, or null if there is no such user</returns>
        public Task<User?> GetUserByEmail(string email);
    }
}
