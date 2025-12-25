using System.Linq.Expressions;

namespace Agentic_Rentify.Application.Specifications;

public class GenericSpecification<T>(
    Expression<Func<T, bool>>? criteria = null,
    Expression<Func<T, object>>? orderBy = null,
    Expression<Func<T, object>>? orderByDescending = null,
    int? skip = null,
    int? take = null,
    bool isPagingEnabled = false) : BaseSpecification<T>(criteria, orderBy, orderByDescending, skip, take, isPagingEnabled);
