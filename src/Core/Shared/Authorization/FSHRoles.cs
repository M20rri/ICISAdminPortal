using System.Collections.ObjectModel;

namespace ICISAdminPortal.Shared.Authorization;
public static class FSHRoles
{
    public static IReadOnlyList<string> DefaultRoles { get; } = new ReadOnlyCollection<string>(new[]
    {
        "Basic",
        "Admin",
        "Applicant",
        "TrafficManagementOfficer",
        "DirectorOfDepartmentApplicant",
        "DirectorOfDepartmentMovement",
        "AdministrationOfficerMovement"
    });

    public static bool IsDefault(string roleName) => DefaultRoles.Any(r => r == roleName);
}