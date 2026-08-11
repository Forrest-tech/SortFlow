FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 拷贝包含所有依赖项目的 API 源码
COPY ./sortflow-api/ ./sortflow-api/

# 还原并编译指定项目
RUN dotnet restore "./sortflow-api/src/SortFlow.Api/SortFlow.Api.csproj"
RUN dotnet publish "./sortflow-api/src/SortFlow.Api/SortFlow.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .

# 修正点：准确指向刚才发布的主入口文件 SortFlow.Api.dll
ENTRYPOINT ["dotnet", "SortFlow.Api.dll"]
