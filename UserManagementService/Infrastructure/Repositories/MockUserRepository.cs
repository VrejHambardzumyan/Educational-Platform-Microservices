using System.Collections.Concurrent;
using UserManagementService.Infrastructure.Entities;
using UserManagementService.Infrastructure.Interfaces;
namespace UserManagementService.Infrastructure.Repositories
{
    public class MockUserRepository : IUserRepository
    {
        private readonly ConcurrentDictionary<string,User> _user =new();

        public Task<User?> GetByUserNameAsync (string userName)
        {
            _user.TryGetValue(userName, out var user);
            return Task.FromResult(user);
        }

        public Task AddEntityAsync(User entity)
        {
            _user[entity.UserName] = entity;
            return Task.CompletedTask;
        }
    }
}
