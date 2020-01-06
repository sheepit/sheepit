# Codename "Sheep It"

This repository contains a prototype of a minimalistic deployment software focused on simplicity and clarity.

Mind, that the prototype is in early stages. Its development is still very dynamic and everything might change in the future, including technology stack, project name or main premise of the software.

## Development

### Using Entity Framework CLI

We use [local CLI tools](https://andrewlock.net/new-in-net-core-3-local-tools/).

```bash
cd source/SheepIt.Api
dotnet tool restore
dotnet dotnet-ef migrations add 'some migration'
```