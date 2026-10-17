# Mycelium

A library for retrieving the status of Minecraft servers.

## Usage

Breaking changes are to be expected.

Java

```cs
var response = await JavaClient.RequestStatusAsync("mc.hypixel.net:25565", JavaVersion.Modern);
```

Legacy Java (1.6, 1.4 to 1.5, and Beta 1.8 to 1.3)

```cs
var response = await JavaClient.RequestStatusAsync("localhost:25565", JavaVersion.LegacyHost);
var response = await JavaClient.RequestStatusAsync("localhost:25565", JavaVersion.LegacyExtended);
var response = await JavaClient.RequestStatusAsync("localhost:25565", JavaVersion.LegacyBeta);
```

Bedrock

```cs
var response = await BedrockClient.RequestStatusAsync("mco.cubecraft.net:19132");
```