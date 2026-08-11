FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 拷贝代码并编译发布
COPY ./sortflow-api/ ./sortflow-api/
WORKDIR /src/sortflow-api
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "sortflow-api.dll"]
