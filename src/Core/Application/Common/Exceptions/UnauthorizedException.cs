using System.Net;

namespace ICISAdminPortal.Application.Common.Exceptions;
public class UnauthorizedException : Exception
{
    public int ErrorCode { get; init; }
    public object Data { get; init; }

    public UnauthorizedException(object data, int errorCode)
    {
        Data = data;
        ErrorCode = errorCode;
    }
}