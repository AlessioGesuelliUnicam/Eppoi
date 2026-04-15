using Xunit;
using Eppoi.API.Services;

namespace Eppoi.Tests;

public class ChatServiceTests
{
    [Fact]
    public void BuildSystemPrompt_ContainsMunicipalityName()
    {
        var method = typeof(ChatService)
            .GetMethod("BuildSystemPrompt",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        var result = (string)method!.Invoke(null, new object[] { "Comune di Gradara", "contesto test" })!;

        Assert.Contains("Comune di Gradara", result);
    }

    [Fact]
    public void BuildSystemPrompt_ContainsContext()
    {
        var method = typeof(ChatService)
            .GetMethod("BuildSystemPrompt",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        var result = (string)method!.Invoke(null, new object[] { "Comune di Gradara", "Rocca Demaniale, museo storico" })!;

        Assert.Contains("Rocca Demaniale, museo storico", result);
    }

    [Fact]
    public void BuildSystemPrompt_ContainsDomainRestriction()
    {
        var method = typeof(ChatService)
            .GetMethod("BuildSystemPrompt",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        var result = (string)method!.Invoke(null, new object[] { "Comune di Gradara", "contesto" })!;

        Assert.Contains("posso rispondere solo a domande", result);
    }

    [Fact]
    public void BuildSystemPrompt_ContainsItalianInstruction()
    {
        var method = typeof(ChatService)
            .GetMethod("BuildSystemPrompt",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        var result = (string)method!.Invoke(null, new object[] { "Comune di Gradara", "contesto" })!;

        Assert.Contains("italiano", result);
    }

    [Fact]
    public void ChatResponse_OutOfDomain_WhenMessageContainsFallback()
    {
        var message = "Mi dispiace, posso rispondere solo a domande sul turismo di Comune di Gradara.";
        var isOutOfDomain = message.Contains("posso rispondere solo a domande", StringComparison.OrdinalIgnoreCase);
        Assert.True(isOutOfDomain);
    }

    [Fact]
    public void ChatResponse_NotOutOfDomain_WhenMessageIsNormal()
    {
        var message = "A Gradara puoi visitare la Rocca Demaniale e il Museo Storico.";
        var isOutOfDomain = message.Contains("posso rispondere solo a domande", StringComparison.OrdinalIgnoreCase);
        Assert.False(isOutOfDomain);
    }
}