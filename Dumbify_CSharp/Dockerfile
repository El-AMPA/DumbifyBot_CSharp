#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["Dumbify_CSharp/Dumbify_CSharp.csproj", "Dumbify_CSharp/"]
RUN dotnet restore "Dumbify_CSharp/Dumbify_CSharp.csproj"
COPY . .
WORKDIR "/src/Dumbify_CSharp"
RUN dotnet build "Dumbify_CSharp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Dumbify_CSharp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ./Dumbify_CSharp/assets ./assets
ENTRYPOINT ["dotnet", "Dumbify_CSharp.dll"]