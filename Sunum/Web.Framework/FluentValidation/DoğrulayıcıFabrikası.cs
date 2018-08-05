using System;
using FluentValidation;
using FluentValidation.Attributes;
using Core.Altyapı;

namespace Web.Framework.FluentValidation
{
    public class DoğrulayıcıFabrikası : AttributedValidatorFactory
    {
        public override IValidator GetValidator(Type tipi)
        {
            if (tipi == null)
                return null;

            var doğrulayıcıÖzniteliği = (ValidatorAttribute)Attribute.GetCustomAttribute(tipi, typeof(ValidatorAttribute));
            if (doğrulayıcıÖzniteliği == null || doğrulayıcıÖzniteliği.ValidatorType == null)
                return null;
            
            var instance = EngineContext.Current.ResolveUnregistered(doğrulayıcıÖzniteliği.ValidatorType);
            return instance as IValidator;
        }
    }
}
