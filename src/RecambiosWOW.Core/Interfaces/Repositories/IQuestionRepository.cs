using RecambiosWOW.Core.Domain.Entities;

namespace RecambiosWOW.Core.Interfaces.Repositories;

public interface IQuestionRepository
{
    Task<Question> GetByIdAsync(int id);
    Task<IEnumerable<Question>> GetByListingIdAsync(int listingId);
    Task<IEnumerable<Question>> GetByAskerIdAsync(int askerId);
    Task<Question> AddAsync(Question question);
    Task<Question> UpdateAsync(Question question);
    Task<bool> DeleteAsync(int id);
}