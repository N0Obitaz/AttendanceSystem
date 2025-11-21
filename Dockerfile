# Use official .NET 8 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Install Node.js + npm (required for Tailwind or any npm run steps)
RUN apt-get update && \
    apt-get install -y curl && \
    curl -fsSL https://deb.nodesource.com/setup_18.x | bash - && \
    apt-get install -y nodejs

# Copy everything
COPY . .

# Install npm packages (package.json must exist)
RUN npm install

# Build your CSS (this is the command failing before)
RUN npm run css:build

# Publish the .NET app
RUN dotnet publish -c Release -o /app


# Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .

# Expose port 8080 for Render
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "AttendanceSystem.dll"]
