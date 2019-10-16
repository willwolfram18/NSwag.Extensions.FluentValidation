using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Validators;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NJsonSchema;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace NSwag.Extensions.FluentValidation
{
    public class FluentValidationProcessor : IOperationProcessor
    {
        private readonly IValidatorFactory _validatorFactory;
        private readonly ILogger _log;

        public FluentValidationProcessor(IValidatorFactory validatorFactory,
            ILoggerFactory loggerFactory = null)
        {
            _validatorFactory = validatorFactory ?? throw new ArgumentNullException(nameof(validatorFactory));
            _log = loggerFactory?.CreateLogger(typeof(FluentValidationProcessor)) ??
                   NullLogger.Instance;
        }

        public bool Process(OperationProcessorContext context)
        {
            if (context is AspNetCoreOperationProcessorContext aspNetProcessorContext)
            {
                return Process(aspNetProcessorContext);
            }

            return true;
        }

        private bool Process(AspNetCoreOperationProcessorContext context)
        {
            foreach (var operationParameter in context.OperationDescription.Operation.Parameters)
            {
                var methodParameter = context.MethodInfo.GetParameters().Single(x => x.Name == operationParameter.Name);
                var validator = _validatorFactory.GetValidator(methodParameter.ParameterType);

                if (validator == null)
                {
                    _log.LogWarning($"Unable to create validator for parameter '{operationParameter.Name}' " +
                                    $"(type '{methodParameter.ParameterType}') for operation '{context.OperationDescription.Path}'");
                    continue;
                }

                foreach (var parameterProperty in operationParameter.ActualSchema.Properties)
                {
                    var validationRules = GetValidatorsForProperty(validator, parameterProperty.Key);

                    foreach (var validationRule in validationRules)
                    {
                        if (validationRule is INotNullValidator || validationRule is INotEmptyValidator)
                        {
                            parameterProperty.Value.IsNullableRaw = false;
                            parameterProperty.Value.IsRequired = true;
                        }

                        if (validationRule is INotEmptyValidator)
                        {
                            parameterProperty.Value.MinLength = 1;
                        }
                    }
                }
            }

            return true;
        }

        private IEnumerable<IPropertyValidator> GetValidatorsForProperty(IValidator validator, string propertyName)
        {
            var validationRules = validator as IEnumerable<IValidationRule>;

            if (validationRules == null)
            {
                return Enumerable.Empty<IPropertyValidator>();
            }

            return validationRules.OfType<PropertyRule>()
                .Where(propertyRule =>
                    propertyRule.Condition == null &&
                    propertyRule.AsyncCondition == null &&
                    !string.IsNullOrWhiteSpace(propertyRule.PropertyName) &&
                    propertyRule.PropertyName.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase)
                ).SelectMany(propertyRule => propertyRule.Validators);
        }
    }
}
