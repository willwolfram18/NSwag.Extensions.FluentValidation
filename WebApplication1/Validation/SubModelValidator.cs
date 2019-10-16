using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using WebApplication1.Models;

namespace WebApplication1.Validation
{
    public class SubModelValidator : AbstractValidator<SubModel>
    {
        public SubModelValidator()
        {
            RuleFor(x => x.Foobar).NotNull().NotEmpty();
        }
    }
}
