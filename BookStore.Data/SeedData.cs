using Azure.Core;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Data
{
    public static class SeedData
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Roles
            var admin = new Role { Name = "Admin" };
            var user = new Role { Name = "User" };
            context.Roles.AddRange(admin, user);
            // await context.SaveChangesAsync();

            // Members
            context.Members.AddRange(
                    new Member
                    {
                        Email = "admin@example.com",
                        Password = "$2a$11$VLzTLJL/pl7VDrEOJrS3quKL1zpRqtW5vSAwP46I/ILdsDPNLQU4m",
                        Roles = [admin]
                    },
                    new Member
                    {
                        Email = "user@example.com",
                        Password = "$2a$11$VLzTLJL/pl7VDrEOJrS3quKL1zpRqtW5vSAwP46I/ILdsDPNLQU4m",
                        Roles = [user]
                    },
                    new Member
                    {
                        Email = "super@example.com",
                        Password = "$2a$11$VLzTLJL/pl7VDrEOJrS3quKL1zpRqtW5vSAwP46I/ILdsDPNLQU4m",
                        Roles = [admin, user]
                    }
            );
            // await context.SaveChangesAsync();


            // Author
            var authorNames = new List<string>
            {
                "張天宇",
                "李曉晨",
                "王思涵",
                "陳子睿",
                "趙欣怡"
            };
            var authors = authorNames.Select(name => new Author { Name = name }).ToList();
            context.Authors.AddRange(authors);
            // await context.SaveChangesAsync();


            // Publisher
            var publisherNames = new List<string>
            {
                "智慧之光",
                "創意文化",
                "書海出版集團",
                "明日文創",
                "青木書坊"
            };
            var publisher = publisherNames.Select(name => new Publisher { Name = name }).ToList();
            context.Publishers.AddRange(publisher);
            // await context.SaveChangesAsync();


            // Book
            context.Books.AddRange(
                new Book
                {
                    Isbn = "978-1-234567-910",
                    Title = "開放原始碼軟體",
                    Description = "本書介紹開放原始碼軟體的基本概念與優勢，並探討常見的開放原始碼工具，如 Git、Linux、Apache 等，適合對開放原始碼有興趣的讀者。",
                    ListPrice = 650,
                    Discount = 12,
                    Stock = 6,
                    PublicationDate = new DateOnly(2020, 12, 10),
                    ImagePath = "example/20000000_000000_0001.png",
                    Author = authors[0],
                    Publisher = publisher[0],
                },
                new Book
                {
                    Isbn = "978-1-234567-911",
                    Title = "改善程式效能",
                    Description = "這本書聚焦於提高程式效能，介紹如何優化代碼，改進算法，減少計算時間與資源消耗，並探討常見的效能瓶頸問題。",
                    ListPrice = 550,
                    Discount = 15,
                    Stock = 1,
                    PublicationDate = new DateOnly(2021, 4, 30),
                    ImagePath = "example/20000000_000000_0002.png",
                    Author = authors[1],
                    Publisher = publisher[1],
                },
                new Book
                {
                    Isbn = "978-1-234567-912",
                    Title = "人工智慧應用實務",
                    Description = "本書介紹人工智慧的實際應用場景，包括影像辨識、語音處理、自然語言處理等，並提供實作範例與程式碼，適合希望將 AI 技術應用於實際項目的讀者。",
                    ListPrice = 780,
                    Discount = 20,
                    Stock = 9,
                    PublicationDate = new DateOnly(2022, 2, 14),
                    ImagePath = "example/20000000_000000_0003.png",
                    Author = authors[2],
                    Publisher = publisher[2],
                },
                new Book
                {
                    Isbn = "978-1-234567-913",
                    Title = "大數據與雲端運算",
                    Description = "本書深入解析大數據與雲端運算的核心技術與應用，探討如何在雲端環境中處理和分析海量數據，為讀者提供未來技術的全面理解。",
                    ListPrice = 850,
                    Discount = 18,
                    Stock = 3,
                    PublicationDate = new DateOnly(2021, 6, 1),
                    ImagePath = "example/20000000_000000_0004.png",
                    Author = authors[3],
                    Publisher = publisher[3],
                },
                new Book
                {
                    Isbn = "978-1-234567-914",
                    Title = "深度學習基礎",
                    Description = "本書介紹深度學習的基礎知識與技術，包含神經網絡、反向傳播、卷積神經網絡等，並提供簡單的實作範例，適合想學習深度學習的讀者。",
                    ListPrice = 950,
                    Discount = 10,
                    Stock = 7,
                    PublicationDate = new DateOnly(2021, 8, 25),
                    ImagePath = "example/20000000_000000_0005.png",
                    Author = authors[4],
                    Publisher = publisher[4],
                },
                new Book
                {
                    Isbn = "978-1-234567-915",
                    Title = "資料結構與演算法實務",
                    Description = "內容涵蓋各種常見資料結構與演算法，搭配圖解與 C# 實作範例，幫助讀者強化邏輯思維與程式能力。",
                    ListPrice = 720,
                    Discount = 25,
                    Stock = 2,
                    PublicationDate = new DateOnly(2023, 3, 12),
                    ImagePath = "example/20000000_000000_0006.png",
                    Author = authors[0],
                    Publisher = publisher[0],
                },
                new Book
                {
                    Isbn = "978-1-234567-916",
                    Title = "軟體開發流程管理",
                    Description = "書中說明敏捷開發、看板與 Scrum 的實際應用，並結合團隊溝通與版本控制，適合團隊領導與 PM 參考。",
                    ListPrice = 620,
                    Discount = 30,
                    Stock = 10,
                    PublicationDate = new DateOnly(2022, 9, 10),
                    ImagePath = "example/20000000_000000_0007.png",
                    Author = authors[1],
                    Publisher = publisher[1],
                },
                new Book
                {
                    Isbn = "978-1-234567-917",
                    Title = "設計模式入門",
                    Description = "以簡單易懂的方式介紹經典的 23 種設計模式，並提供各種實際應用範例與常見錯誤解析。",
                    ListPrice = 680,
                    Discount = 22,
                    Stock = 4,
                    PublicationDate = new DateOnly(2021, 11, 5),
                    ImagePath = "example/20000000_000000_0008.png",
                    Author = authors[2],
                    Publisher = publisher[2],
                },
                new Book
                {
                    Isbn = "978-1-234567-918",
                    Title = "資料庫設計實務",
                    Description = "本書從資料庫正規化出發，深入探討 ER 模型與 SQL 實作，適合初學者到中階開發人員使用。",
                    ListPrice = 590,
                    Discount = 18,
                    Stock = 0,
                    PublicationDate = new DateOnly(2020, 7, 15),
                    ImagePath = "example/20000000_000000_0009.png",
                    Author = authors[3],
                    Publisher = publisher[3],
                },
                new Book
                {
                    Isbn = "978-1-234567-919",
                    Title = "C# 入門教學",
                    Description = "從最基本的語法開始介紹，逐步帶領讀者學習 C# 程式語言，包含控制流程、物件導向等主題。",
                    ListPrice = 520,
                    Discount = 10,
                    Stock = 8,
                    PublicationDate = new DateOnly(2023, 1, 8),
                    ImagePath = "example/20000000_000000_0010.png",
                    Author = authors[4],
                    Publisher = publisher[4],
                },
                new Book
                {
                    Isbn = "978-1-234567-920",
                    Title = "網頁前端開發實戰",
                    Description = "以 HTML、CSS、JavaScript 為基礎，結合實務專案與版面設計技巧，協助讀者快速上手前端開發。",
                    ListPrice = 750,
                    Discount = 27,
                    Stock = 2,
                    PublicationDate = new DateOnly(2022, 4, 22),
                    ImagePath = "example/20000000_000000_0011.png",
                    Author = authors[0],
                    Publisher = publisher[0],
                },
                new Book
                {
                    Isbn = "978-1-234567-921",
                    Title = "雲端架構設計",
                    Description = "介紹微服務、容器化技術與 DevOps 流程，深入說明如何設計可擴充的雲端系統。",
                    ListPrice = 980,
                    Discount = 15,
                    Stock = 5,
                    PublicationDate = new DateOnly(2021, 12, 12),
                    ImagePath = "example/20000000_000000_0012.png",
                    Author = authors[1],
                    Publisher = publisher[1],
                },
                new Book
                {
                    Isbn = "978-1-234567-922",
                    Title = "Linux 系統管理實務",
                    Description = "涵蓋 Linux 指令、檔案系統、使用者管理、排程與自動化等主題，適合系統管理初學者。",
                    ListPrice = 630,
                    Discount = 20,
                    Stock = 1,
                    PublicationDate = new DateOnly(2023, 6, 5),
                    ImagePath = "example/20000000_000000_0013.png",
                    Author = authors[2],
                    Publisher = publisher[2],
                },
                new Book
                {
                    Isbn = "978-1-234567-923",
                    Title = "Python 資料分析入門",
                    Description = "從 Python 基礎語法開始，帶領讀者學習 pandas、matplotlib 與數據視覺化技巧。",
                    ListPrice = 710,
                    Discount = 16,
                    Stock = 6,
                    PublicationDate = new DateOnly(2022, 10, 1),
                    ImagePath = "example/20000000_000000_0014.png",
                    Author = authors[3],
                    Publisher = publisher[3],
                },
                new Book
                {
                    Isbn = "978-1-234567-924",
                    Title = "RESTful API 設計指南",
                    Description = "介紹如何設計一致且易於維護的 RESTful API，包含錯誤處理、安全性與版本控管實作。",
                    ListPrice = 640,
                    Discount = 19,
                    Stock = 0,
                    PublicationDate = new DateOnly(2023, 2, 15),
                    ImagePath = "example/20000000_000000_0015.png",
                    Author = authors[4],
                    Publisher = publisher[4],
                },
                new Book
                {
                    Isbn = "978-1-234567-925",
                    Title = "軟體測試實務",
                    Description = "深入介紹單元測試、整合測試、自動化測試等主題，並提供範例程式碼與測試策略。",
                    ListPrice = 670,
                    Discount = 14,
                    Stock = 9,
                    PublicationDate = new DateOnly(2021, 7, 20),
                    ImagePath = "example/20000000_000000_0016.png",
                    Author = authors[0],
                    Publisher = publisher[0],
                },
                new Book
                {
                    Isbn = "978-1-234567-926",
                    Title = "敏捷開發實戰",
                    Description = "以實例導向方式講解 Scrum、看板與持續整合，適合團隊實作參考使用。",
                    ListPrice = 600,
                    Discount = 12,
                    Stock = 7,
                    PublicationDate = new DateOnly(2022, 3, 28),
                    ImagePath = "example/20000000_000000_0017.png",
                    Author = authors[1],
                    Publisher = publisher[1],
                },
                new Book
                {
                    Isbn = "978-1-234567-927",
                    Title = "UI/UX 設計基礎",
                    Description = "本書結合使用者心理學與設計實務，深入探討介面設計的原則與最佳實踐。",
                    ListPrice = 540,
                    Discount = 20,
                    Stock = 3,
                    PublicationDate = new DateOnly(2023, 5, 14),
                    ImagePath = "example/20000000_000000_0018.png",
                    Author = authors[2],
                    Publisher = publisher[2],
                },
                new Book
                {
                    Isbn = "978-1-234567-928",
                    Title = "TypeScript 實戰入門",
                    Description = "從 JavaScript 過渡到 TypeScript，介紹類型系統與應用場景，搭配前端框架範例。",
                    ListPrice = 700,
                    Discount = 25,
                    Stock = 4,
                    PublicationDate = new DateOnly(2022, 11, 3),
                    ImagePath = "example/20000000_000000_0019.png",
                    Author = authors[0],
                    Publisher = publisher[0],
                },
                new Book
                {
                    Isbn = "978-1-234567-929",
                    Title = "資安入門與攻防實務",
                    Description = "帶你了解常見的資安攻擊與防禦策略，包含 XSS、SQL Injection 與資安測試技巧。",
                    ListPrice = 800,
                    Discount = 28,
                    Stock = 10,
                    PublicationDate = new DateOnly(2021, 10, 10),
                    ImagePath = "example/20000000_000000_0020.png",
                    Author = authors[1],
                    Publisher = publisher[1],
                },
                new Book
                {
                    Isbn = "978-1-234567-931",
                    Title = "跨平台 App 開發實務",
                    Description = "本書介紹如何使用 Flutter 與 React Native 進行跨平台 App 開發，搭配實作範例幫助讀者快速上手。",
                    ListPrice = 760,
                    Discount = 24,
                    Stock = 1,
                    PublicationDate = new DateOnly(2023, 7, 18),
                    ImagePath = "example/20000000_000000_0021.png",
                    Author = authors[2],
                    Publisher = publisher[2],
                },
                new Book
                {
                    Isbn = "978-1-234567-932",
                    Title = "演算法視覺化入門",
                    Description = "透過圖解方式說明常見演算法的運作原理，幫助初學者更容易理解排序、搜尋等邏輯。",
                    ListPrice = 690,
                    Discount = 17,
                    Stock = 8,
                    PublicationDate = new DateOnly(2022, 8, 9),
                    ImagePath = "example/20000000_000000_0022.png",
                    Author = authors[3],
                    Publisher = publisher[3],
                }
            );
            await context.SaveChangesAsync();
        }
    }
}
