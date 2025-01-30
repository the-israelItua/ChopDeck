# Use the official image as a base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Use SDK image to build app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ChopDeck.csproj", "./"]
RUN dotnet restore "ChopDeck.csproj"
COPY . .
WORKDIR "/src/ChopDeck"
RUN dotnet build "ChopDeck.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChopDeck.csproj" -c Release -o /app/publish

# Copy published app to base image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish 

# Start the app
ENTRYPOINT ["dotnet", "ChopDeck.dll"]
