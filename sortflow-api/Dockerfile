FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 拷贝 sortflow-api 源码并发布
COPY ./sortflow-api/ ./sortflow-api/
WORKDIR /src/sortflow-api
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .

# 使用 shell 形式自动查找并运行目录下匹配的入口 dll（排除带有 .Views.dll 等辅助 dll）
ENTRYPOINT ["sh", "-c", "exec dotnet $(ls *.dll | grep -i 'sortflow' | grep -v 'deps' | grep -v 'runtimeconfig' | head -n 1)"]
