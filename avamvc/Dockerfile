# 使用 .NET SDK 建構映像
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# 複製 csproj 並還原相依套件
COPY *.csproj ./
RUN dotnet restore

# 複製其他檔案並建構專案
COPY . ./
RUN dotnet publish -c Release -o out

# 建立執行階段映像
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# 設定 ASP.NET Core 預設啟動埠（Render 用的）
ENV ASPNETCORE_URLS=http://+:80

# 啟動應用
ENTRYPOINT ["dotnet", "avamvc.dll"]
