FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /MoneyHumanizer.Service

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Run unit tests
RUN dotnet test
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0

EXPOSE 1025

WORKDIR /MoneyHumanizer.Service
COPY --from=build-env /MoneyHumanizer.Service/out .

ENTRYPOINT ["dotnet", "MoneyHumanizer.Service.dll"]
