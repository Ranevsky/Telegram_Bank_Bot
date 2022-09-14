namespace BankAPI.Exceptions;

public class DepartmentNotLocatedInRadiusException : Exception
{
    public DepartmentNotLocatedInRadiusException(int departmentId, double radiusOfOccurrence, double distance)
        : base($"Department with id = '{departmentId}', not located in radius of occurrence = '{radiusOfOccurrence}', distance = '{distance}'")
    {

    }
}
