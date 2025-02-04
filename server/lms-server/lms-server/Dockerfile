# Use base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use SDK image for building and applying migrations
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files explicitly (excluding tests)
COPY ["lms-server/lms-server.csproj", "lms-server/"]
COPY ["lms.Abstractions/lms.Abstractions.csproj", "lms.Abstractions/"]
COPY ["lms.Services/lms.Services.csproj", "lms.Services/"]
COPY ["lms.Peer/lms.Peer.csproj", "lms.Peer/"]

# Restore dependencies
WORKDIR /src/lms-server
RUN dotnet restore "lms-server.csproj"

# Copy all remaining source files excluding tests
COPY . . 
RUN rm -rf ./lms-server/lms.Tests # Remove the test folder

# Install EF Core CLI tools
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# Apply EF migrations
RUN dotnet ef database update || true

# Build the project (without explicitly setting output path)
RUN dotnet build "lms-server.csproj" -c $BUILD_CONFIGURATION /p:UseAppHost=false

# Publish the project (ensuring apphost is disabled)
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "lms-server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Create final runtime image
FROM base AS final
WORKDIR /app

# Copy the published files from the build stage
COPY --from=publish /app/publish .

# Copy the books.json file to the appropriate location
COPY lms-server/books.json /app/books.json

# Set the entry point for the application
ENTRYPOINT ["dotnet", "lms-server.dll"]
