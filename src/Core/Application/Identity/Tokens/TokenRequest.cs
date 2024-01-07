namespace ICISAdminPortal.Application.Identity.Tokens;

public record TokenRequest(string Email, string Password, string Role);

public class TokenRequestValidator : CustomValidator<TokenRequest>
{
    public TokenRequestValidator()
    {
        RuleFor(p => p.Email).Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress()
                .WithMessage("Invalid Email");

        RuleFor(p => p.Password).Cascade(CascadeMode.Stop)
            .NotEmpty();

        RuleFor(p => p.Role).Cascade(CascadeMode.Stop)
            .NotEmpty();
    }
}