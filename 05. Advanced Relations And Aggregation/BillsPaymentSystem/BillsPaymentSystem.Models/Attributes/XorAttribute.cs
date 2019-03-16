using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BillsPaymentSystem.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class XorAttribute : ValidationAttribute
    {
        private string targetProperty;

        public XorAttribute(string targetProperty)
        {
            this.targetProperty = targetProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var targetPropertyValue = validationContext.ObjectType
                .GetProperty(targetProperty)
                .GetValue(validationContext.ObjectInstance);

            if ((value == null) ^ (targetProperty == null))
            {
                return new ValidationResult("The two properties must have opposite values!");
            }

            return ValidationResult.Success;
        }
    }
}
