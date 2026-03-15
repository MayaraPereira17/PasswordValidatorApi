
namespace PasswordValidatorApi.Validators
{
    public interface IPasswordValidator
    {
        bool IsValid(string password);
    }
}
