# Use the official .NET 8 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy everything
COPY . .

# Restore and build inside the subfolder
WORKDIR /app/UnganaConnect
RUN dotnet restore
RUN dotnet publish -c Release -o /out

# Use a lightweight runtime image for production
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out .

# Expose the port Render/Koyeb expects
ENV ASPNETCORE_URLS=http://+:8000
EXPOSE 8000

# Run the app
ENTRYPOINT ["dotnet", "UnganaConnect.dll"]
