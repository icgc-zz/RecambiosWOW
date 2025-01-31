using RecambiosWOW.Core.Domain.Criteria;
using RecambiosWOW.Core.Domain.Entities;

namespace RecambiosWOW.Core.Interfaces.Repositories;

public interface IMemberRepository
{
    Task<Member> GetByIdAsync(int id);
    Task<Member> GetByEmailAsync(string email);
    Task<Member> GetByUsernameAsync(string username);
    Task<IEnumerable<Member>> SearchAsync(MemberSearchCriteria criteria);
    Task<Member> AddAsync(Member member);
    Task<Member> UpdateAsync(Member member);
    Task<bool> DeleteAsync(int id);
}