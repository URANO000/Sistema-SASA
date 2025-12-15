namespace SASA.Services.Auth
{
    public enum AuthErrorCode
    {
        None,
        InvalidCredentials,
        AccountLocked,
        InvalidToken,
        ExpiredToken,
        WeakPassword,
        PasswordMismatch
    }

    public class AuthResult
    {
        public bool Ok { get; protected set; }
        public AuthErrorCode Code { get; protected set; }
        public string Message { get; protected set; } = "";

        public static AuthResult Success(string message = "") =>
            new AuthResult { Ok = true, Code = AuthErrorCode.None, Message = message };

        public static AuthResult Fail(AuthErrorCode code, string message) =>
            new AuthResult { Ok = false, Code = code, Message = message };
    }

    public class AuthResult<T> : AuthResult
    {
        public T? Data { get; protected set; }

        public static AuthResult<T> Success(T data, string message = "") =>
            new AuthResult<T> { Ok = true, Code = AuthErrorCode.None, Message = message, Data = data };

        public new static AuthResult<T> Fail(AuthErrorCode code, string message) =>
            new AuthResult<T> { Ok = false, Code = code, Message = message };
    }

    public record AuthSession(string Email, string DisplayName);
}
