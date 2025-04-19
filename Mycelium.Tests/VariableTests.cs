using Mycelium.Features.Java.Packets;

namespace Mycelium.Tests;

internal sealed class VariableTests
{
    [Test]
    public void Integer_GetByteCount_IsCorrect()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Variable.GetByteCount(0), Is.EqualTo(1));
            Assert.That(Variable.GetByteCount(-1), Is.EqualTo(5));
        });
    }
}