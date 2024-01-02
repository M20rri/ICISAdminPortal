namespace ICISAdminPortal.Application.Common.Exceptions;
public class CustomException : Exception
{
    public int ErrorCode { get; init; }
    public object Data { get; init; }

    public CustomException(object data, int errorCode)
    {
        Data = data;
        ErrorCode = errorCode;
    }
}