FROM ubuntu:22.04

# Set environment variables
ENV DEBIAN_FRONTEND=noninteractive
ENV DOTNET_SDK_VERSION=8.0

# Install required packages for C#/Dotnet projects
RUN apt-get update && apt-get install -y \
    wget \
    apt-transport-https \
    ca-certificates \
    gnupg \
    software-properties-common \
    && wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && rm packages-microsoft-prod.deb \
    && apt-get update && apt-get install -y \
    dotnet-sdk-$DOTNET_SDK_VERSION \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

# Set the working directory
WORKDIR /app

# Copy the project files to the container
COPY . .

# # Restore the dependencies
RUN dotnet restore

# # Build the project
RUN dotnet build --configuration Release

# # Default command to run when the container starts
CMD ["dotnet", "run"]
