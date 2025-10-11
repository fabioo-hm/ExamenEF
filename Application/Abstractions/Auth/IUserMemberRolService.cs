using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Domain.Entities.Auth;

namespace Application.Abstractions.Auth;

public interface IUserMemberRolService
{
        Task<IEnumerable<UserMemberRol>> GetAllAsync();
        Task<UserMemberRol?> GetByIdsAsync(int userMemberId, int roleId);
        Task AddAsync(UserMemberRol entity);
        Task AddRangeAsync(IEnumerable<UserMemberRol> entities);
        void Update(UserMemberRol entity);
        void Remove(UserMemberRol entity);
        void RemoveRange(IEnumerable<UserMemberRol> entities);
        Task<List<UserMemberRol>> FindAsync(Expression<Func<UserMemberRol, bool>> predicate);
        Task<List<UserMemberRol>> GetByUserIdAsync(int userMemberId);
}