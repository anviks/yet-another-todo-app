FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["YetAnotherTodoApp.WebApp/YetAnotherTodoApp.WebApp.csproj", "YetAnotherTodoApp.WebApp/"]
COPY ["YetAnotherTodoApp.Seeder/YetAnotherTodoApp.Seeder.csproj", "YetAnotherTodoApp.Seeder/"]

RUN dotnet restore "YetAnotherTodoApp.WebApp/YetAnotherTodoApp.WebApp.csproj"
RUN dotnet restore "YetAnotherTodoApp.Seeder/YetAnotherTodoApp.Seeder.csproj"

COPY . .

WORKDIR "/src/YetAnotherTodoApp.WebApp"
RUN dotnet build "YetAnotherTodoApp.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish-webapp
RUN dotnet publish "YetAnotherTodoApp.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/webapp /p:UseAppHost=false

FROM build AS publish-seeder
WORKDIR "/src/YetAnotherTodoApp.Seeder"
RUN dotnet publish "YetAnotherTodoApp.Seeder.csproj" -c $BUILD_CONFIGURATION -o /app/seeder /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=publish-webapp /app/webapp .
COPY --from=publish-seeder /app/seeder ./seeder

ENTRYPOINT ["dotnet", "YetAnotherTodoApp.WebApp.dll"]
