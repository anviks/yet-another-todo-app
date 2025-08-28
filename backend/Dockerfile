FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["YetAnotherTodoApp.WebApp/YetAnotherTodoApp.WebApp.csproj", "YetAnotherTodoApp.WebApp/"]
RUN dotnet restore "YetAnotherTodoApp.WebApp/YetAnotherTodoApp.WebApp.csproj"

COPY . .
WORKDIR "/src/YetAnotherTodoApp.WebApp"
RUN dotnet build "YetAnotherTodoApp.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "YetAnotherTodoApp.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "YetAnotherTodoApp.WebApp.dll"]
