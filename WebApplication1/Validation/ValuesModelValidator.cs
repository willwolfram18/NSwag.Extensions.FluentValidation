﻿using System;
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
        public ValuesModelValidator(IValidator<SubModel> subModelValidator)
        {
            RuleFor(x => x.Values).NotNull();
            RuleFor(x => x.Values).NotEmpty();

            When(x => x.IsBilling, () => { RuleFor(x => x.Address).NotNull().NotEmpty(); });

            RuleSet("Example", () => { RuleFor(x => x.Address).NotNull().NotEmpty(); });

            RuleFor(x => x.Foo).SetValidator(subModelValidator);
        }
    }
}
