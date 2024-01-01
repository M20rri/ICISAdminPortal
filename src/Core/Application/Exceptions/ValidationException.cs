namespace ICISAdminPortal.Application.Exceptions;
public class ValidationException : Exception
{
    public int ErrorCode { get; init; }
    public object Data { get; init; }

    public ValidationException(object data, int errorCode)
    {
        Data = data;
        ErrorCode = errorCode;
    }
}
