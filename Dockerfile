FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file from subdirectory
COPY EducationPlatformBackend/*.csproj EducationPlatformBackend/
RUN dotnet restore EducationPlatformBackend/EducationPlatformBackend.csproj

# Copy everything else
COPY EducationPlatformBackend/ EducationPlatformBackend/
WORKDIR /src/EducationPlatformBackend
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080
ENTRYPOINT ["./EducationPlatformBackend"]