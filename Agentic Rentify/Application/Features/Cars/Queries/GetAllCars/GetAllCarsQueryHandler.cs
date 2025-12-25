using System.Linq.Expressions;
using Agentic_Rentify.Application.Features.Cars.DTOs;
using Agentic_Rentify.Application.Wrappers;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Application.Interfaces;
using AutoMapper;
using MediatR;

namespace Agentic_Rentify.Application.Features.Cars.Queries.GetAllCars;

public class GetAllCarsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) 
    : IRequestHandler<GetAllCarsQuery, PagedResponse<CarResponseDTO>>
{
    public async Task<PagedResponse<CarResponseDTO>> Handle(GetAllCarsQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Car, bool>>? filter = null;
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim();
            filter = c =>
                (c.Name != null && c.Name.Contains(term)) ||
                (c.City != null && c.City.Contains(term)) ||
                (c.Brand != null && c.Brand.Contains(term)) ||
                (c.Model != null && c.Model.Contains(term));
        }

        var (items, totalCount) = await unitOfWork.Repository<Car>()
            .GetPagedAppAsync(request.PageNumber, request.PageSize, filter);

        var dtos = mapper.Map<IReadOnlyList<CarResponseDTO>>(items);

        return new PagedResponse<CarResponseDTO>(dtos, request.PageNumber, request.PageSize, totalCount);
    }
}
