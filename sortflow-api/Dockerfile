FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./sortflow-api/ ./sortflow-api/
WORKDIR /src/sortflow-api
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .

# 修正点：替换为真实的 DLL 名称（注意大小写，Linux 对大小写敏感）
# 例如: SortFlow.API.dll 或 SortFlowApi.dll
ENTRYPOINT ["dotnet", "SortFlow.API.dll"]
