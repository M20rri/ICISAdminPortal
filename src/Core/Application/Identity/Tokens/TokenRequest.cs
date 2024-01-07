namespace ICISAdminPortal.Application.Identity.Tokens;

public record TokenRequest(string Email, string Password, string Role);

public class TokenRequestValidator : CustomValidator<TokenRequest>
{
    public TokenRequestValidator()
    {
        RuleFor(p => p.Email)
            .NotEmpty()
            .EmailAddress()
                .WithMessage("Invalid Email Address.");

        RuleFor(p => p.Password)
            .NotEmpty()
            .WithMessage("Password is required.");

        RuleFor(p => p.Role)
            .NotEmpty()
            .WithMessage("Role is required.");
    }
}