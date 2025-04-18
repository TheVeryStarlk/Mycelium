# Mycelium

An ASP.NET Core Web API project for retrieving the status of any Minecraft Java (and soon Bedrock) server.

## Usage

Mycelium currently exposes one endpoint for getting the status of a Minecraft Java edition server.\
An example of a Java server status response.

```json
{
  "description": "A Minecraft Server",
  "name": "PaperSpigot 1.8.8",
  "version": 47,
  "maximum": 20,
  "online": 0
}
```

## Plans

* Refactor status response to use JSON reader.
* Implement pinging.
* Add support for Bedrock edition.
* Write tests.