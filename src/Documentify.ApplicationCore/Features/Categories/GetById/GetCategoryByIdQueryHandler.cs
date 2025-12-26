using Documentify.ApplicationCore.Common.Exceptions;
using Documentify.ApplicationCore.Repository;
using Documentify.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Documentify.ApplicationCore.Features.Categories.GetById
{
    public class GetCategoryByIdQueryHandler(IUnitOfWork _unitOfWork) : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDto>>
    {
        public async Task<Result<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken ct)
        {
            Category? category = await _unitOfWork.Repository<Category, Guid>().GetByIdAsync(request.id, ct);
            if (category is null)
                throw new NotFoundException($"Category with id {request.id} not found.");

            return Result<CategoryDto>.Success(new CategoryDto(category));
        }
    }
}
