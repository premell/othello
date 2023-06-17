FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["othello_api/othello_api.csproj", "othello_api/"]
RUN dotnet restore "othello_api/othello_api.csproj"
COPY . .
WORKDIR "/src/othello_api"
RUN dotnet build "othello_api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "othello_api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "othello_api.dll"]
