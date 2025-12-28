# Mycelium

A library for retrieving the status of Minecraft servers.

## Usage

Breaking changes are to be expected.

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