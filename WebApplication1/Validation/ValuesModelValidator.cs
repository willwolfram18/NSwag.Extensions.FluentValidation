using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using WebApplication1.Models;

namespace WebApplication1.Validation
{
    public class ValuesModelValidator : AbstractValidator<ValuesModel>
    {
        public ValuesModelValidator()
        {
            RuleFor(x => x.Values).NotNull();
            RuleFor(x => x.Values).NotEmpty();
        }
    }
}
