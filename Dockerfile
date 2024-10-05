# Use the ASP.NET 7.0 runtime image as the base layer.
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base

# Set the working directory inside the container to /app. All subsequent commands will run inside this directory.
WORKDIR /app

# Expose port 8081 to the outside, allowing access to the application running on this port.
EXPOSE 8081

# Set an environment variable to specify the URL that ASP.NET Core will bind to. In this case, it's binding to port 8081.
# ENV ASPNETCORE_URLS=http://+:8081

# Use the .NET SDK 7.0 image to build the application. This image includes the necessary tools for building .NET applications.
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Define an argument for the build configuration. The default is "Release".
ARG configuration=Release

# Set the working directory to /src for the build process.
WORKDIR /src

# Copy the project file (finTrack.csproj) to the container. This is needed to restore dependencies.
COPY ["finTrack.csproj", "./"]

# Restore the project dependencies specified in the .csproj file.
RUN dotnet restore "finTrack.csproj"

# Copy the rest of the project files to the container for the build process.
COPY . .

# Change the working directory to the project source directory.
WORKDIR "/src/."

# Build the project in the specified configuration (Release or Debug) and output the result to the /app/build directory.
RUN dotnet build "finTrack.csproj" -c $configuration -o /app/build

# Create a separate stage for publishing the project.
FROM build AS publish

# Define the same argument for the build configuration in the publish stage.
ARG configuration=Release

# Publish the project to the /app/publish directory without the app host (which creates a small output).
RUN dotnet publish "finTrack.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

# Use the base image (ASP.NET runtime) as the final image.
FROM base AS final

# Set the working directory inside the final container to /app.
WORKDIR /app

# Copy the published project from the /app/publish directory of the publish stage to the /app directory in the final image.
COPY --from=publish /app/publish .

# Define the entry point for the container. When the container starts, it will run the finTrack.dll file using the dotnet command.
ENTRYPOINT ["dotnet", "finTrack.dll"]
