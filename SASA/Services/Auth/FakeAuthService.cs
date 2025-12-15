using System.Text.RegularExpressions;

namespace SASA.Services.Auth
{
    public interface IAuthService
    {
        Task<AuthResult<AuthSession>> LoginAsync(string email, string password);
        Task<AuthResult> ForgotPasswordAsync(string email);
        Task<AuthResult> ResetPasswordAsync(string token, string newPassword);
        Task<AuthResult> ActivateAccountAsync(string token);
    }

    public class FakeAuthService : IAuthService
    {
        private const int MaxFailedAttempts = 5;

        // Simulación en memoria
        private readonly Dictionary<string, int> _failedAttemptsByEmail = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _lockedEmails = new(StringComparer.OrdinalIgnoreCase);

        // Tokens simulados (puedes ajustar valores para demo)
        private readonly HashSet<string> _expiredTokens = new(StringComparer.OrdinalIgnoreCase)
        {
            "expired-reset-token",
            "expired-activate-token"
        };

        private readonly HashSet<string> _invalidTokens = new(StringComparer.OrdinalIgnoreCase)
        {
            "invalid-reset-token",
            "invalid-activate-token"
        };

        // “Usuario” demo
        private readonly string _demoEmail = "admin@sasa.com";
        private readonly string _demoPassword = "Admin123!";

        public Task<AuthResult<AuthSession>> LoginAsync(string email, string password)
        {
            // Regla mock: email vacío o password vacío no debería llegar aquí si VM valida,
            // pero igual nos protegemos.
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return Task.FromResult(AuthResult<AuthSession>.Fail(AuthErrorCode.InvalidCredentials, "Credenciales inválidas."));

            if (_lockedEmails.Contains(email))
                return Task.FromResult(AuthResult<AuthSession>.Fail(AuthErrorCode.AccountLocked, "Cuenta bloqueada por múltiples intentos."));

            // Credenciales válidas solo para demo
            var isValid = email.Equals(_demoEmail, StringComparison.OrdinalIgnoreCase) && password == _demoPassword;

            if (!isValid)
            {
                var current = _failedAttemptsByEmail.TryGetValue(email, out var c) ? c : 0;
                current++;
                _failedAttemptsByEmail[email] = current;

                if (current >= MaxFailedAttempts)
                {
                    _lockedEmails.Add(email);
                    return Task.FromResult(AuthResult<AuthSession>.Fail(AuthErrorCode.AccountLocked, "Cuenta bloqueada por múltiples intentos."));
                }

                return Task.FromResult(AuthResult<AuthSession>.Fail(AuthErrorCode.InvalidCredentials, "Credenciales inválidas."));
            }

            // éxito → resetea contador
            _failedAttemptsByEmail.Remove(email);

            var session = new AuthSession(email, "Administrador");
            return Task.FromResult(AuthResult<AuthSession>.Success(session, "Inicio de sesión exitoso."));
        }

        public Task<AuthResult> ForgotPasswordAsync(string email)
        {
            // Política recomendada: siempre OK para no filtrar existencia de correo
            return Task.FromResult(AuthResult.Success("Si el correo existe, se enviará un enlace de recuperación."));
        }

        public Task<AuthResult> ResetPasswordAsync(string token, string newPassword)
        {
            var tokenCheck = ValidateToken(token);
            if (!tokenCheck.Ok) return Task.FromResult(tokenCheck);

            if (!IsStrongPassword(newPassword))
                return Task.FromResult(AuthResult.Fail(AuthErrorCode.WeakPassword, "La contraseña no cumple con los requisitos."));

            // éxito
            return Task.FromResult(AuthResult.Success("Contraseña actualizada. Ahora puedes iniciar sesión."));
        }

        public Task<AuthResult> ActivateAccountAsync(string token)
        {
            var tokenCheck = ValidateToken(token);
            if (!tokenCheck.Ok) return Task.FromResult(tokenCheck);

            return Task.FromResult(AuthResult.Success("Cuenta activada. Ahora puedes iniciar sesión."));
        }

        private AuthResult ValidateToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return AuthResult.Fail(AuthErrorCode.InvalidToken, "Token inválido.");

            if (_invalidTokens.Contains(token))
                return AuthResult.Fail(AuthErrorCode.InvalidToken, "Token inválido.");

            if (_expiredTokens.Contains(token))
                return AuthResult.Fail(AuthErrorCode.ExpiredToken, "Token expirado.");

            return AuthResult.Success();
        }

        private static bool IsStrongPassword(string password)
        {
            // Ejemplo simple de complejidad: >= 8, mayúscula, minúscula, número y símbolo
            if (string.IsNullOrWhiteSpace(password)) return false;
            if (password.Length < 8) return false;

            var hasUpper = Regex.IsMatch(password, "[A-Z]");
            var hasLower = Regex.IsMatch(password, "[a-z]");
            var hasDigit = Regex.IsMatch(password, "[0-9]");
            var hasSymbol = Regex.IsMatch(password, @"[^a-zA-Z0-9]");

            return hasUpper && hasLower && hasDigit && hasSymbol;
        }
    }
}
