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
}