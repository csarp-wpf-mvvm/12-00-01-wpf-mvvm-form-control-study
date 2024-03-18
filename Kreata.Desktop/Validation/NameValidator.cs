using Kreta.Desktop.Validation.ValidationRules;
using System.Globalization;
using System.Windows.Controls;

namespace Kreta.Desktop.Validation
{
    public class NameValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string)
            {
                string nameToValidate = (string)value;
                NameValidationRules nvr = new NameValidationRules(nameToValidate);
                if (nvr.IsNameShort)
                {
                    return new ValidationResult(false, "A név túl rövid!");
                }
                if (!nvr.IsFirstLetterUppercase)
                {
                    return new ValidationResult(false, "A név első betűje nagybetű kell legyen!");
                }
                if (!nvr.IsOtherLetterLowercase)
                {
                    return new ValidationResult(false, "Csak a név első betűje lehet nagybetű!");
                }
                if (!nvr.IsOnlyLetters)
                {
                    return new ValidationResult(false, "A név csak betűből állhat!");
                }
            }
            return new ValidationResult(true, "");
        }
    }
}
