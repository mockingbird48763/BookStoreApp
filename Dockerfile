# 基礎運行環境（Runtime Image）
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# 構建階段（Build & Publish）
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# 複製解決方案檔與專案檔案
COPY ["BookStoreApp.sln", "."]
COPY ["BookStore.API/BookStore.API.csproj", "BookStore.API/"]
COPY ["BookStore.Data/BookStore.Data.csproj", "BookStore.Data/"]
COPY ["BookStore.DTO/BookStore.DTO.csproj", "BookStore.DTO/"]
COPY ["BookStore.Models/BookStore.Models.csproj", "BookStore.Models/"]
COPY ["BookStore.Repositories/BookStore.Repositories.csproj", "BookStore.Repositories/"]
COPY ["BookStore.Services/BookStore.Services.csproj", "BookStore.Services/"]
COPY ["BookStore.Services.Tests/BookStore.Services.Tests.csproj", "BookStore.Services.Tests/"]
COPY ["Core/BookStore.Core.csproj", "Core/"]

# 還原 NuGet 套件
RUN dotnet restore "BookStoreApp.sln"

# 複製所有原始碼
COPY . .

# 編譯 API 專案
WORKDIR /src
RUN dotnet build BookStoreApp.sln -c Release -o /app/build

# 發布 API 專案
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# 最終映像（Runtime + 發布內容）
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookStore.API.dll"]
