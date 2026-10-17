using Mycelium.Java;

namespace Mycelium.Tests;

internal sealed class JavaResponseTests
{
    [Test]
    public void JavaResponse_TryCreateModern_IsCorrect()
    {
        const string input = """{"version":{"name":"1.21.2","protocol":768},"players":{"max":100,"online":5},"description":{"text":"Hello, world!"}}""";

        Assert.Multiple(() =>
        {
            Assert.That(JavaResponse.TryCreate(input, JavaVersion.Modern, out var response), Is.True);

            Assert.That(response?.Description, Is.EqualTo("""{"text":"Hello, world!"}"""));

            Assert.That(response?.Name, Is.EqualTo("1.21.2"));
            Assert.That(response?.Version, Is.EqualTo(768));

            Assert.That(response?.Maximum, Is.EqualTo(100));
            Assert.That(response?.Online, Is.EqualTo(5));
        });
    }

    [Test]
    public void JavaResponse_TryCreateModernIncomplete_IsFalse()
    {
        const string input = """{"version":{"protocol":768},"players":{"max":100,"online":5},"description":{"text":"Hello, world!"}}""";

        Assert.That(JavaResponse.TryCreate(input, JavaVersion.Modern, out _), Is.False);
    }

    [Test]
    public void JavaResponse_TryCreateLegacyHost_IsCorrect()
    {
        const string input = "§1\078\01.6.4\0A Minecraft Server\05\020";

        Assert.Multiple(() =>
        {
            Assert.That(JavaResponse.TryCreate(input, JavaVersion.LegacyHost, out var response), Is.True);

            Assert.That(response?.Description, Is.EqualTo("A Minecraft Server"));

            Assert.That(response?.Name, Is.EqualTo("1.6.4"));
            Assert.That(response?.Version, Is.EqualTo(78));

            Assert.That(response?.Maximum, Is.EqualTo(20));
            Assert.That(response?.Online, Is.EqualTo(5));
        });
    }

    [Test]
    public void JavaResponse_TryCreateLegacyExtended_IsCorrect()
    {
        const string input = "§1\061\01.5.2\0Legacy Server\02\050";

        Assert.Multiple(() =>
        {
            Assert.That(JavaResponse.TryCreate(input, JavaVersion.LegacyExtended, out var response), Is.True);

            Assert.That(response?.Description, Is.EqualTo("Legacy Server"));

            Assert.That(response?.Name, Is.EqualTo("1.5.2"));
            Assert.That(response?.Version, Is.EqualTo(61));

            Assert.That(response?.Maximum, Is.EqualTo(50));
            Assert.That(response?.Online, Is.EqualTo(2));
        });
    }

    [Test]
    public void JavaResponse_TryCreateLegacyHostMultilineDescription_IsCorrect()
    {
        const string input = "§1\0127\01.21.4\0§aFirst line\n§bSecond line\00\0100";

        Assert.Multiple(() =>
        {
            Assert.That(JavaResponse.TryCreate(input, JavaVersion.LegacyHost, out var response), Is.True);

            Assert.That(response?.Description, Is.EqualTo("§aFirst line\n§bSecond line"));

            Assert.That(response?.Name, Is.EqualTo("1.21.4"));
            Assert.That(response?.Version, Is.EqualTo(127));

            Assert.That(response?.Maximum, Is.EqualTo(100));
            Assert.That(response?.Online, Is.EqualTo(0));
        });
    }

    [Test]
    public void JavaResponse_TryCreateLegacyBeta_IsCorrect()
    {
        const string input = "A Beta Server§3§16";

        Assert.Multiple(() =>
        {
            Assert.That(JavaResponse.TryCreate(input, JavaVersion.LegacyBeta, out var response), Is.True);

            Assert.That(response?.Description, Is.EqualTo("A Beta Server"));

            Assert.That(response?.Name, Is.Null);
            Assert.That(response?.Version, Is.EqualTo(-1));

            Assert.That(response?.Maximum, Is.EqualTo(16));
            Assert.That(response?.Online, Is.EqualTo(3));
        });
    }

    [Test]
    public void JavaResponse_TryCreateLegacyBetaFormatted_IsCorrect()
    {
        const string input = "§aColorful §bBeta Server§3§16";

        Assert.Multiple(() =>
        {
            Assert.That(JavaResponse.TryCreate(input, JavaVersion.LegacyBeta, out var response), Is.True);

            Assert.That(response?.Description, Is.EqualTo("§aColorful §bBeta Server"));

            Assert.That(response?.Name, Is.Null);
            Assert.That(response?.Version, Is.EqualTo(-1));

            Assert.That(response?.Maximum, Is.EqualTo(16));
            Assert.That(response?.Online, Is.EqualTo(3));
        });
    }

    [Test]
    public void JavaResponse_TryCreateLegacyBetaEmptyDescription_IsCorrect()
    {
        const string input = "§0§20";

        Assert.Multiple(() =>
        {
            Assert.That(JavaResponse.TryCreate(input, JavaVersion.LegacyBeta, out var response), Is.True);

            Assert.That(response?.Description, Is.Empty);

            Assert.That(response?.Maximum, Is.EqualTo(20));
            Assert.That(response?.Online, Is.EqualTo(0));
        });
    }

    [Test]
    [TestCase("§1\078\01.6.4\0A Minecraft Server\05")]
    [TestCase("§1\078\01.6.4\0A Minecraft Server\05\020\0Extra")]
    [TestCase("§1\0Version\01.6.4\0A Minecraft Server\05\020")]
    [TestCase("§1\078\01.6.4\0A Minecraft Server\0Five\020")]
    [TestCase("§1\078\01.6.4\0A Minecraft Server\05\0")]
    [TestCase("§1\0")]
    [TestCase("78\01.6.4\0A Minecraft Server\05\020")]
    [TestCase("A Beta Server§3§16")]
    public void JavaResponse_TryCreateLegacyHostMalformed_IsFalse(string input)
    {
        Assert.That(JavaResponse.TryCreate(input, JavaVersion.LegacyHost, out _), Is.False);
    }

    [Test]
    [TestCase("")]
    [TestCase("A Beta Server")]
    [TestCase("A Beta Server§16")]
    [TestCase("A Beta Server§3§")]
    [TestCase("A Beta Server§Three§16")]
    [TestCase("Outdated client!")]
    public void JavaResponse_TryCreateLegacyBetaMalformed_IsFalse(string input)
    {
        Assert.That(JavaResponse.TryCreate(input, JavaVersion.LegacyBeta, out _), Is.False);
    }

    [Test]
    public void JavaResponse_TryCreateUndefinedVersion_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => JavaResponse.TryCreate("", (JavaVersion) 42, out _));
    }
}