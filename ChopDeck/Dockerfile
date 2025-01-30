# Use the official image as a base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Use SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the .csproj file to the app folder inside the container
COPY ["ChopDeck.csproj", "./"]

# Restore dependencies
RUN dotnet restore "ChopDeck.csproj"

# Copy the rest of the application files into the container
COPY . .

# Build the app
RUN dotnet build "ChopDeck.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChopDeck.csproj" -c Release -o /app/publish

# Copy published app to base image
FROM base AS final
WORKDIR /app

# Copy the published app from the publish stage to the final image
COPY --from=publish /app/publish /app

# Start the app
ENTRYPOINT ["dotnet", "ChopDeck.dll"]
