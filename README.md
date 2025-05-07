# 📚 BookStoreApp

一個簡易的線上書店系統，具備前台瀏覽與後台管理功能。

需搭配前端專案 [BookStoreApp Frontend](https://github.com/mockingbird48763/BookStoreApp-Frontend) 使用。

## 🌍 線上範例
[線上範例](http://34.80.36.77)

## 🔑 登入帳號資訊

### 管理者帳號
- 帳號：`admin@example.com`  
- 密碼：`abcd1234`

### 一般使用者帳號
- 帳號：`user@example.com`  
- 密碼：`abcd1234`

## ⚙️ 技術棧
- NET 8
- Entity Framework Core
- SQL Server（線上）、LocalDB（本地）
- Swagger
- JWT 認證
- Google Cloud Storage — 圖片儲存（線上）
- Docker

## 🚀 專案啟動
恢復 NuGet 套件：
```
dotnet restore
```

初始化資料庫（僅第一次啟動或重置資料庫需要）
```
dotnet run --project BookStore.API/BookStore.API.csproj --seed
```

啟動專案：
```
dotnet run --project BookStore.API/BookStore.API.csproj
```

## 🧩 ER Model
![er-model](https://github.com/user-attachments/assets/7b96d1b8-9a71-4713-b3cd-c530524f5796)


