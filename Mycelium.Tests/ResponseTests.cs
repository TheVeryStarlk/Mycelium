using System.Buffers;
using System.Text;
using Mycelium.Features.Bedrock;
using Mycelium.Features.Java.Response;

namespace Mycelium.Tests;

internal sealed class ResponseTests
{
    [Test]
    public void BedrockResponse_TryCreate_IsCorrect()
    {
        const string input = "MCPE;Dedicated Server;390;1.14.60;0;10;13253860892328930865;Bedrock level;Survival;1;19132;19133;";

        Assert.Multiple(() =>
        {
            Assert.That(BedrockResponse.TryCreate(input, out var response), Is.True);

            Assert.That(response?.Description[0], Is.EqualTo("Dedicated Server"));
            Assert.That(response?.Description[1], Is.EqualTo("Bedrock level"));

            Assert.That(response?.Name, Is.EqualTo("1.14.60"));
            Assert.That(response?.Version, Is.EqualTo(390));

            Assert.That(response?.Maximum, Is.EqualTo(10));
            Assert.That(response?.Online, Is.EqualTo(0));
        });
    }
    
    [Test]
    public void BedrockResponse_TryCreateIncomplete_IsFalse()
    {
        const string input = "MCPE;390;1.14.60;0;10;13253860892328930865;Bedrock level;Survival;1;19132;19133;";

        Assert.That(BedrockResponse.TryCreate(input, out _), Is.False);
    }

    [Test]
    public void JavaResponse_TryCreate_IsCorrect()
    {
        const string input = """{"version":{"name":"1.21.2","protocol":768},"players":{"max":100,"online":5},"description":{"text":"Hello, world!"}}""";

        Assert.Multiple(() =>
        {
            Assert.That(JavaResponse.TryCreate(new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(input)), out var response), Is.True);

            Assert.That(response?.Description, Is.EqualTo("""{"text":"Hello, world!"}"""));

            Assert.That(response?.Name, Is.EqualTo("1.21.2"));
            Assert.That(response?.Version, Is.EqualTo(768));

            Assert.That(response?.Maximum, Is.EqualTo(100));
            Assert.That(response?.Online, Is.EqualTo(5));
        });
    }
    
    [Test]
    public void JavaResponse_TryCreateIncomplete_IsFalse()
    {
        const string input = """{"version":{"protocol":768},"players":{"max":100,"online":5},"description":{"text":"Hello, world!"}}""";

        Assert.That(JavaResponse.TryCreate(new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(input)), out _), Is.False);
    }
}