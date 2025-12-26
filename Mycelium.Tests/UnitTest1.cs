namespace Mycelium.Tests;

internal sealed class AddressTests
{
    [Test]
    public void Valid_TryParse_ReturnsTrue()
    {
        const string input = "hello.world:25565";
        
        Assert.Multiple(() =>
        {
            Assert.That(Address.TryParse(input, out var address), Is.True);

            Assert.That(address.Host, Is.EqualTo("hello.world"));
            Assert.That(address.Port, Is.EqualTo(25565));
        });
    }
}