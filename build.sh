#!/bin/bash

# Baixa o script oficial de instalação do .NET
curl -sSL https://dot.net/v1/dotnet-install.sh > dotnet-install.sh
chmod +x dotnet-install.sh

# Instala o .NET 10 em uma pasta oculta local
./dotnet-install.sh -c 10.0 -InstallDir ./.dotnet

# Publica o projeto Blazor na pasta 'release'
./.dotnet/dotnet publish -c Release -o release