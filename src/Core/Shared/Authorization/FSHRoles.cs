using System.Collections.ObjectModel;

namespace ICISAdminPortal.Shared.Authorization;
public static class FSHRoles
{
    public const string Basic = nameof(Basic);
    public const string Admin = nameof(Admin);
    public const string Applicant = nameof(Applicant);
    public const string TrafficManagementOfficer = nameof(TrafficManagementOfficer);
    public const string DirectorOfDepartmentApplicant = nameof(DirectorOfDepartmentApplicant);
    public const string DirectorOfDepartmentMovement = nameof(DirectorOfDepartmentMovement);
    public const string AdministrationOfficerMovement = nameof(AdministrationOfficerMovement);

    public static IReadOnlyList<string> DefaultRoles { get; } = new ReadOnlyCollection<string>(new[]
    {
        Basic,
        Admin,
        Applicant,
        TrafficManagementOfficer,
        DirectorOfDepartmentApplicant,
        DirectorOfDepartmentMovement,
        AdministrationOfficerMovement
    });

    public static bool IsDefault(string roleName) => DefaultRoles.Any(r => r == roleName);
}