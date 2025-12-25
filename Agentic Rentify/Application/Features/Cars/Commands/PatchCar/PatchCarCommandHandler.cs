using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Application.Interfaces;
using MediatR;

namespace Agentic_Rentify.Application.Features.Cars.Commands.PatchCar;

public class PatchCarCommandHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<PatchCarCommand, int>
{
    public async Task<int> Handle(PatchCarCommand request, CancellationToken cancellationToken)
    {
        var car = await unitOfWork.Repository<Car>().GetByIdAsync(request.Id);
        if (car == null)
        {
            throw new Exception($"Car with ID {request.Id} not found.");
        }

        // Apply partial updates
        if (request.Status != null)
        {
            car.Status = request.Status;
        }

        if (request.Featured.HasValue)
        {
            car.Featured = request.Featured.Value;
            car.IsFeatured = request.Featured.Value;
        }

        if (request.IsFeatured.HasValue)
        {
            car.IsFeatured = request.IsFeatured.Value;
            car.Featured = request.IsFeatured.Value;
        }

        await unitOfWork.Repository<Car>().UpdateAsync(car);
        await unitOfWork.CompleteAsync();

        return car.Id;
    }
}
