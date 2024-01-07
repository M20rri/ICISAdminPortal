namespace ICISAdminPortal.Application.Identity.Tokens;

public record TokenRequest(string Email, string Password, string Role);

public class TokenRequestValidator : AbstractValidator<TokenRequest>
{
    public TokenRequestValidator()
    {
        RuleFor(p => p.Email)
            .NotEmpty()
            .EmailAddress()
                .WithMessage("Invalid Email");

        RuleFor(p => p.Password)
            .NotEmpty()
            .WithMessage("Password is required");

        RuleFor(p => p.Role)
            .NotEmpty()
            .WithMessage("Role is required");
    }
}