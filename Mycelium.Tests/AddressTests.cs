using Mycelium.Features;

namespace Mycelium.Tests;

internal sealed class AddressTests
{
    [Test]
    public void Address_TryParseSingleDomain_IsCorrect()
    {
        const string input = "hello.world:25565";

        Assert.Multiple(() =>
        {
            Assert.That(Address.TryParse(input, out var address), Is.True);
            Assert.That(address.Host, Is.EqualTo("hello.world"));
            Assert.That(address.Port, Is.EqualTo(25565));
        });
    }
    
    [Test]
    public void Address_TryParseMultipleDomains_IsCorrect()
    {
        const string input = "foo.bar.qux:25565";

        Assert.Multiple(() =>
        {
            Assert.That(Address.TryParse(input, out var address), Is.True);
            Assert.That(address.Host, Is.EqualTo("foo.bar.qux"));
            Assert.That(address.Port, Is.EqualTo(25565));
        });
    }

    [Test]
    public void Address_TryParseNoPort_IsFalse()
    {
        const string input = "hello.world";

        Assert.That(Address.TryParse(input, out _), Is.False);
    }
    
    [Test]
    public void Address_TryParseLargePort_IsFalse()
    {
        const string input = "hello.world:2556525565";

        Assert.That(Address.TryParse(input, out _), Is.False);
    }
    
    [Test]
    public void Address_TryParseEmpty_IsFalse()
    {
        const string input = "";

        Assert.That(Address.TryParse(input, out _), Is.False);
    }
}