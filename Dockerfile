# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY *.sln ./
COPY UnganaConnect/*.csproj ./UnganaConnect/
RUN dotnet restore

# Copy everything else and build
COPY . .
WORKDIR /src/UnganaConnect
RUN dotnet publish -c Release -o /app/out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy published output
COPY --from=build /app/out .

# Optional environment variables
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_ENVIRONMENT=Production

# Expose port Render expects
EXPOSE 10000

# Run the app
ENTRYPOINT ["dotnet", "UnganaConnect.dll"]
