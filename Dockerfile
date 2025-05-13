# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copia os arquivos e restaura dependências
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o /out

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /out .

# Porta padrão (ajuste se necessário)
EXPOSE 80

ENTRYPOINT ["dotnet", "BrasileiraoAPI.dll"]