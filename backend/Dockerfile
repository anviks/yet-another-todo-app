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
RUN dotnet publish "YetAnotherTodoApp.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/webapp /p:UseAppHost=false

WORKDIR "/src/YetAnotherTodoApp.Seeder"
RUN dotnet publish "YetAnotherTodoApp.Seeder.csproj" -c $BUILD_CONFIGURATION -o /app/seeder /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=build /app/webapp ./webapp
COPY --from=build /app/seeder ./seeder

COPY seeder.sh /app/seeder.sh
RUN chmod +x /app/seeder.sh

ENTRYPOINT ["dotnet", "YetAnotherTodoApp.WebApp.dll"]
