using Mycelium.Features;

namespace Mycelium.Tests;

internal sealed class AddressTests
{
    [Test]
    public void Address_TryParse_IsCorrect()
    {
        const string input = "mc.hypixel.net:25565";

        Assert.Multiple(() =>
        {
            Assert.That(Address.TryParse(input, out var address), Is.True);
            Assert.That(address.Host, Is.EqualTo("mc.hypixel.net"));
            Assert.That(address.Port, Is.EqualTo(25565));
        });
    }
}