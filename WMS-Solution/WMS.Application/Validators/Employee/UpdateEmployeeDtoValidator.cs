using FluentValidation;
using WMS.Application.DTOs.Employee;

namespace WMS.Application.Validators.Employee;

public class UpdateEmployeeDtoValidator
    : AbstractValidator<UpdateEmployeeDto>
{
    public UpdateEmployeeDtoValidator()
    {
        RuleFor(x => x.EmployeeId)
            .GreaterThan(0);

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^[0-9]{10}$")
            .WithMessage("Phone number must be 10 digits.");

        RuleFor(x => x.DepartmentId)
            .GreaterThan(0);

        RuleFor(x => x.RoleId)
            .GreaterThan(0);
    }
}
