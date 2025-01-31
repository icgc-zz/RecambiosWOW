using System.Linq.Expressions;

namespace RecambiosWOW.Core.Interfaces.Repositories;

/// <summary>
/// Interface for specification pattern
/// </summary>
public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
    List<Expression<Func<T, object>>> Includes { get; }
    List<string> IncludeStrings { get; }
    Expression<Func<T, object>>? OrderBy { get; }
    Expression<Func<T, object>>? OrderByDescending { get; }
    
    int Take { get; }
    int Skip { get; }
    bool IsPagingEnabled { get; }
}
