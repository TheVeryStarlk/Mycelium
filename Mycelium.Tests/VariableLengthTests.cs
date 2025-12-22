using Mycelium.Features.Java.Packets;

namespace Mycelium.Tests;

internal sealed class VariableLengthTests
{
    [Test]
    public void Integer_GetByteCount_IsCorrect()
    {
        Assert.Multiple(() =>
        {
            Assert.That(VariableLength.GetByteCount(0), Is.EqualTo(1));
            Assert.That(VariableLength.GetByteCount(-1), Is.EqualTo(5));
        });
    }

    [Test]
    public void VariableInteger_Writing_IsCorrect()
    {
        var actual = new byte[sizeof(int)];

        actual = actual[..VariableLength.Write(actual, short.MaxValue)];

        var expected = new byte[]
        {
            255,
            255,
            1
        };

        Assert.Multiple(() =>
        {
            Assert.That(actual, Has.Length.EqualTo(3));
            Assert.That(actual, Is.EqualTo(expected));
        });
    }
}