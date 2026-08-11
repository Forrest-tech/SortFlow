FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 复制 API 源代码并还原发布
COPY ./sortflow-api/ ./sortflow-api/
RUN dotnet restore "./sortflow-api/src/SortFlow.Api/SortFlow.Api.csproj"
RUN dotnet publish "./sortflow-api/src/SortFlow.Api/SortFlow.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .

# 精准指向 SortFlow.Api.dll
ENTRYPOINT ["dotnet", "SortFlow.Api.dll"]
