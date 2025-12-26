using Mycelium.Java;

namespace Mycelium.Tests;

internal sealed class JavaResponseTests
{
    [Test]
    public void JavaResponse_TryCreate_IsCorrect()
    {
        const string input = """{"version":{"name":"1.21.2","protocol":768},"players":{"max":100,"online":5},"description":{"text":"Hello, world!"}}""";

        Assert.Multiple(() =>
        {
            Assert.That(JavaResponse.TryCreate(input, out var response), Is.True);

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

        Assert.That(JavaResponse.TryCreate(input, out _), Is.False);
    }
}