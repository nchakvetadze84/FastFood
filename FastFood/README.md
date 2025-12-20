# FastFood Database Migration Guide

## მიმოხილვა

ეს პაკეტი შეიცავს FastFood პროექტის Microsoft SQL Server-დან PostgreSQL-ზე მიგრაციისთვის საჭირო ყველა კომპონენტს.

## ფაილების აღწერა

| ფაილი | აღწერა |
|-------|--------|
| `DatabaseProvider.cs` | Database Provider აბსტრაქცია SQL Server და PostgreSQL-ისთვის |
| `UserSecure.cs` | განახლებული User კლასი SQL Injection-ის გარეშე |
| `PostgreSQL_Schema.sql` | PostgreSQL მონაცემთა ბაზის სქემა |
| `App.config.template` | კონფიგურაციის შაბლონი |

## მიგრაციის ნაბიჯები

### 1. Prerequisites

```bash
# PostgreSQL-ის ინსტალაცია
# Windows: https://www.postgresql.org/download/windows/
# ან Docker-ით:
docker run --name fastfood-postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=fastfood -p 5432:5432 -d postgres
```

### 2. NuGet Package-ების დამატება

```powershell
# Visual Studio Package Manager Console-ში:
Install-Package Npgsql
Install-Package BCrypt.Net-Next  # პაროლების ჰეშირებისთვის (რეკომენდებული)
```

### 3. PostgreSQL ბაზის შექმნა

```bash
# psql-ით შესვლა და სქემის შექმნა
psql -U postgres -d fastfood -f PostgreSQL_Schema.sql
```

### 4. კოდის ინტეგრაცია

#### 4.1 DatabaseProvider.cs-ის დამატება

1. დაამატეთ `DatabaseProvider.cs` FastFood პროექტში
2. დარწმუნდით რომ namespace შეესაბამება: `FastFood`

#### 4.2 Registry-ში კონფიგურაციის დამატება

```csharp
// Program.cs-ში Main მეთოდის დასაწყისში:
using Microsoft.Win32;

// PostgreSQL-ზე გადართვისთვის:
RegistryKey rk = Registry.CurrentUser.CreateSubKey("Connection");
rk.SetValue("ConnectionString", "Host=localhost;Port=5432;Database=fastfood;Username=postgres;Password=postgres");
rk.SetValue("DatabaseType", "PostgreSql");

// SQL Server-ზე დასაბრუნებლად:
// rk.SetValue("DatabaseType", "SqlServer");
```

#### 4.3 ძველი DBObject-ის ჩანაცვლება

```csharp
// შეცვალეთ ძველი DBObject გამოძახებები:
// ძველი:
// DataTable dt = DBObject.InvokeTString("SELECT * FROM Users");

// ახალი:
DataTable dt = DBObjectNew.InvokeTString("SELECT * FROM Users");
```

### 5. მონაცემების მიგრაცია

#### ვარიანტი A: Manual Export/Import

1. SQL Server Management Studio-დან Export Data
2. CSV ფაილების გენერაცია
3. PostgreSQL-ში import:

```sql
\copy "Users" FROM 'users.csv' WITH CSV HEADER;
```

#### ვარიანტი B: pgLoader (რეკომენდებული)

```bash
# pgLoader-ის ინსტალაცია
# https://pgloader.io/

pgloader mssql://user:pass@localhost/FastFood postgresql://postgres:postgres@localhost/fastfood
```

### 6. ტესტირება

```csharp
// ტესტის მაგალითი:
[TestMethod]
public void TestDatabaseConnection()
{
    DBObjectNew.Initialize(DatabaseType.PostgreSql, 
        "Host=localhost;Port=5432;Database=fastfood;Username=postgres;Password=postgres");
    
    var result = DBObjectNew.InvokeTString("SELECT 1 as test");
    Assert.IsNotNull(result);
    Assert.AreEqual(1, result.Rows.Count);
}
```

## SQL სინტაქსის განსხვავებები

| SQL Server | PostgreSQL | შენიშვნა |
|------------|------------|----------|
| `GETDATE()` | `NOW()` | მიმდინარე დრო |
| `ISNULL(a,b)` | `COALESCE(a,b)` | NULL შემოწმება |
| `TOP 10` | `LIMIT 10` | რაოდენობის შეზღუდვა |
| `[Column]` | `"Column"` | იდენტიფიკატორის quoting |
| `BIT` | `BOOLEAN` | ლოგიკური ტიპი |
| `IDENTITY` | `SERIAL` | Auto-increment |

## Rollback გეგმა

თუ PostgreSQL-ზე მიგრაცია პრობლემურია:

1. Registry-ში შეცვალეთ `DatabaseType` = `SqlServer`
2. აპლიკაცია ავტომატურად გადაერთვება SQL Server-ზე

## ცნობილი შეზღუდვები

1. Crystal Reports: შეიძლება საჭიროებდეს დამატებით კონფიგურაციას PostgreSQL ODBC-სთვის
2. Stored Procedures: საჭიროებს ხელით გადაწერას PL/pgSQL-ზე
3. BLOB/Image fields: შეამოწმეთ binary data-ს კორექტული მიგრაცია

## დამატებითი რესურსები

- [Npgsql Documentation](https://www.npgsql.org/doc/)
- [PostgreSQL vs SQL Server Syntax](https://wiki.postgresql.org/wiki/Microsoft_SQL_Server_to_PostgreSQL_Migration)
- [pgLoader Guide](https://pgloader.readthedocs.io/)

## კონტაქტი

ტექნიკური კითხვებისთვის მიმართეთ დეველოპმენტის გუნდს.

---
*დოკუმენტის ვერსია: 1.0*
*თარიღი: 2025-12-20*
