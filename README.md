# Mycelium

An AOT-friendly library for retrieving the status of Minecraft Java/Bedrock servers.

## Usage

Java

```cs
var response = await JavaClient.RequestStatusAsync("mc.hypixel.net:25565");
```

Bedrock

```cs
var response = await BedrockClient.RequestStatusAsync("mco.cubecraft.net:19132");
```

## Plans

* Implement legacy servers.