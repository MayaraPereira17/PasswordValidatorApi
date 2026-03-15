using System.Text.RegularExpressions;

using System.Text.RegularExpressions;

namespace PasswordValidatorApi.Validators
{
    public class PasswordValidator : IPasswordValidator
    {
        public bool IsValid(string password)
        {
            if (string.IsNullOrEmpty(password)) return false;

            bool hasMinimumLength = password.Length >= 9;
            bool hasNoWhiteSpaces = !password.Any(char.IsWhiteSpace);
            bool hasDigit = Regex.IsMatch(password, @"\d");
            bool hasUpperCase = Regex.IsMatch(password, @"[A-Z]");
            bool hasLowerCase = Regex.IsMatch(password, @"[a-z]");
            bool hasSpecialCharacter = Regex.IsMatch(password, @"[!@#$%^&*()\-\+]");
            bool hasNoRepeatedCharacters = !HasRepeatedCharacters(password);

            return hasMinimumLength &&
                   hasNoWhiteSpaces &&
                   hasDigit &&
                   hasUpperCase &&
                   hasLowerCase &&
                   hasSpecialCharacter &&
                   hasNoRepeatedCharacters;
        }

        private bool HasRepeatedCharacters(string password)
        {
            var characters = new HashSet<char>();
            foreach (var c in password)
            {
                if (!characters.Add(c))
                {
                    return true;
                }
            }
            return false;
        }
    }
}