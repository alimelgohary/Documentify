using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Documentify.ApplicationCore.Features.Categories.Add
{
    public record AddCategoryCommand(string categoryName) : IRequest<Result<AddCategoryResponse>>;
}
