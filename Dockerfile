FROM mcr.microsoft.com/dotnet/sdk AS build
WORKDIR /src
COPY ./ ./
RUN dotnet restore Endpoint/Endpoint.csproj && dotnet publish Endpoint/Endpoint.csproj -c Release -o publish


FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
WORKDIR /app
COPY --from=build /src/publish ./
EXPOSE 80
VOLUME /var/run/docker.sock
ENTRYPOINT ["dotnet", "Endpoint.dll"]