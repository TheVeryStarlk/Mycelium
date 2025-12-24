using System.Diagnostics.CodeAnalysis;

namespace Mycelium.Clients.Bedrock;

public sealed class BedrockResponse
{
    public string[] Description { get; set; } =
    [
        string.Empty,
        string.Empty
    ];

    public string? Name { get; set; }

    public int Version { get; set; }

    public int Maximum { get; set; }

    public int Online { get; set; }

    public static bool TryCreate(ReadOnlySpan<char> input, [NotNullWhen(true)] out BedrockResponse? response)
    {
        response = new BedrockResponse();
        
        var parts = input.Split(':');
        var index = 0;

        foreach (var part in parts)
        {
            switch (index)
            {
                case 1:
                    response.Description[0] = input[part].ToString();
                    break;

                case 2:
                    if (!int.TryParse(input[part], out var version))
                    {
                        return false;
                    }

                    response.Version = version;
                    
                    break;

                case 3:
                    response.Name = input[part].ToString();
                    break;

                case 4:
                    if (!int.TryParse(input[part], out var online))
                    {
                        return false;
                    }

                    response.Online = online;

                    break;

                case 5:
                    if (!int.TryParse(input[part], out var maximum))
                    {
                        return false;
                    }

                    response.Maximum = maximum;
                    
                    break;

                case 7:
                    response.Description[1] = input[part].ToString();
                    break;
            }

            index++;
        }

        // Have all parts been read?
        return index > 7;
    }
}