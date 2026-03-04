namespace Eppoi.API.Services;

public static class EmailTemplates
{
    public static string EmailVerification(string name, string verificationLink) => $@"
        <html>
        <body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
            <h2 style='color: #333;'>Benvenuto su Eppoi, {name}!</h2>
            <p>Grazie per esserti registrato. Clicca il link qui sotto per verificare la tua email:</p>
            <a href='{verificationLink}' 
               style='background-color: #4CAF50; color: white; padding: 12px 24px; 
                      text-decoration: none; border-radius: 4px; display: inline-block;'>
                Verifica Email
            </a>
            <p style='color: #999; margin-top: 24px; font-size: 12px;'>
                Se non hai creato un account su Eppoi, ignora questa email.
            </p>
        </body>
        </html>";

    public static string PasswordReset(string name, string resetLink) => $@"
        <html>
        <body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
            <h2 style='color: #333;'>Reset password — Eppoi</h2>
            <p>Ciao {name}, hai richiesto il reset della tua password.</p>
            <p>Clicca il link qui sotto per impostare una nuova password. Il link scade tra 1 ora.</p>
            <a href='{resetLink}' 
               style='background-color: #e53935; color: white; padding: 12px 24px; 
                      text-decoration: none; border-radius: 4px; display: inline-block;'>
                Reset Password
            </a>
            <p style='color: #999; margin-top: 24px; font-size: 12px;'>
                Se non hai richiesto il reset della password, ignora questa email.
            </p>
        </body>
        </html>";
}