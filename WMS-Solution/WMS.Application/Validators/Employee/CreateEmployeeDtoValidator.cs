using FluentValidation;
using WMS.Application.DTOs.Employee;

namespace WMS.Application.Validators.Employee;

public class CreateEmployeeDtoValidator
    : AbstractValidator<CreateEmployeeDto>
{
    public CreateEmployeeDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^[0-9]{10}$")
            .WithMessage("Phone number must be 10 digits.");

        RuleFor(x => x.DOB)
            .Must(BeAtLeast18YearsOld)
            .WithMessage("Employee must be at least 18 years old.");

        RuleFor(x => x.DOJ)
            .LessThanOrEqualTo(DateTime.Today);

        RuleFor(x => x.DepartmentId)
            .GreaterThan(0);

        RuleFor(x => x.RoleId)
            .GreaterThan(0);
    }

    private bool BeAtLeast18YearsOld(DateTime dob)
    {
        return dob <= DateTime.Today.AddYears(-18);
    }
}
