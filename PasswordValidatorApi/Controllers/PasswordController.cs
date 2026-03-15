using Microsoft.AspNetCore.Mvc;
using PasswordValidatorApi.Models;
using PasswordValidatorApi.Validators;

namespace PasswordValidatorApi.Controllers
{
    [ApiController]
    [Route("validate-password")]
    public class PasswordController : ControllerBase
    {
        private readonly IPasswordValidator _passwordValidator;

        public PasswordController(IPasswordValidator passwordValidator)
        {
            _passwordValidator = passwordValidator;
        }

        [HttpPost]
        public ActionResult<bool> Validate([FromBody] PasswordRequest request)
        {
            var passwordRequest = request?.Password ?? string.Empty;
            var result = _passwordValidator.IsValid(passwordRequest);

            if (result)
            {
                return Ok(result); 
            }

            return UnprocessableEntity(result);
        }
    }
}
