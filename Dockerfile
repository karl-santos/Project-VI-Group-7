# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore dependencies
COPY Speedrun/Speedrun/*.csproj ./Speedrun/
RUN dotnet restore ./Speedrun/Speedrun.csproj

# Copy everything else and build
COPY Speedrun/Speedrun/ ./Speedrun/
RUN dotnet publish ./Speedrun/Speedrun.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Expose port 80
EXPOSE 80

# Run the application
ENTRYPOINT ["dotnet", "Speedrun.dll"]