namespace Validot
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;

    using Validot.Specification;
    using Validot.Translations;

    public static class EmailRules
    {
        private static readonly Regex EmailDomainRegex = new Regex(@"(@)(.+)$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(200));

        private static readonly Regex EmailRegex = new Regex(@"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

        public static IRuleOut<string> Email(this IRuleIn<string> @this)
        {
            return @this.RuleTemplate(IsValidEmail, MessageKey.Texts.Email);
        }

        private static bool IsValidEmail(string email)
        {
            // Entirely copy-pasted from https://docs.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            try
            {
                email = EmailDomainRegex.Replace(email, DomainMapper);

                string DomainMapper(Match match)
                {
                    var idn = new IdnMapping();

                    var domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }

            try
            {
                return EmailRegex.IsMatch(email);
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    }
}
