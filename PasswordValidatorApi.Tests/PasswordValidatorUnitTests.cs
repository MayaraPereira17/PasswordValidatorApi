using PasswordValidatorApi.Validators;

namespace PasswordValidatorApi.Tests
{
    public class PasswordValidatorUnitTests
    {
        private readonly PasswordValidator _validator = new();

        [Theory]
        [InlineData("")]
        [InlineData("aa")]
        [InlineData("ab")]
        [InlineData("AAAbbbCc")]
        [InlineData("AbTp9!foo")]
        [InlineData("AbTp9!foA")]
        [InlineData("AbTp9 fok")]
        public void ChallengeExamples_Invalid_ReturnsFalse(string password)
        {
            Assert.False(_validator.IsValid(password));
        }

        [Fact]
        public void ChallengeExample_Valid_ReturnsTrue()
        {
            Assert.True(_validator.IsValid("AbTp9!fok"));
        }

        [Fact]
        public void NullPassword_ReturnsFalse()
        {
            Assert.False(_validator.IsValid(null!));
        }

        [Fact]
        public void MinLength_LessThan9_ReturnsFalse()
        {
            Assert.False(_validator.IsValid("Ab1!Cd2@"));
        }

        [Fact]
        public void NoDigit_ReturnsFalse()
        {
            Assert.False(_validator.IsValid("AbTp!fokX"));
        }

        [Fact]
        public void NoLowercase_ReturnsFalse()
        {
            Assert.False(_validator.IsValid("ABTP9!FOK"));
        }

        [Fact]
        public void NoUppercase_ReturnsFalse()
        {
            Assert.False(_validator.IsValid("abtp9!fok"));
        }

        [Fact]
        public void NoSpecialCharacter_ReturnsFalse()
        {
            Assert.False(_validator.IsValid("AbT9cdEfg"));
        }

        [Fact]
        public void ContainsWhitespace_ReturnsFalse()
        {
            Assert.False(_validator.IsValid("AbTp9!fo k"));
        }

        [Fact]
        public void RepeatedCharacters_ReturnsFalse()
        {
            Assert.False(_validator.IsValid("AbTp9!foA"));
        }

        [Theory]
        [InlineData("AbTp9!fok")]
        [InlineData("Hj7@kLmNp")]
        [InlineData("xY3$zWvQr")]
        public void ValidPasswords_ReturnsTrue(string password)
        {
            Assert.True(_validator.IsValid(password));
        }

        [Theory]
        [InlineData("!")]
        [InlineData("@")]
        [InlineData("#")]
        [InlineData("$")]
        [InlineData("%")]
        [InlineData("^")]
        [InlineData("&")]
        [InlineData("*")]
        [InlineData("(")]
        [InlineData(")")]
        [InlineData("-")]
        [InlineData("+")]
        public void AllSpecialCharactersAreAccepted(string specialChar)
        {
            var password = "AbTp9" + specialChar + "fok";
            Assert.True(_validator.IsValid(password));
        }
    }
}
