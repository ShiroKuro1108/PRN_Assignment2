# FU News Management Models

This document describes the Entity Framework models created for the FUNewsManagement database.

## Models Created

### 1. Category.cs
- **CategoryID** (short, Primary Key)
- **CategoryName** (string, 100 chars, Required)
- **CategoryDesciption** (string, 250 chars, Required)
- **ParentCategoryID** (short?, Foreign Key to Category)
- **IsActive** (bool?)

**Navigation Properties:**
- ParentCategory (self-referencing)
- SubCategories (collection of child categories)
- NewsArticles (collection of news articles in this category)

### 2. SystemAccount.cs
- **AccountID** (short, Primary Key)
- **AccountName** (string?, 100 chars)
- **AccountEmail** (string?, 70 chars, Email validation)
- **AccountRole** (int?)
- **AccountPassword** (string?, 70 chars)

**Navigation Properties:**
- CreatedNewsArticles (articles created by this account)
- UpdatedNewsArticles (articles updated by this account)

### 3. Tag.cs
- **TagID** (int, Primary Key)
- **TagName** (string?, 50 chars)
- **Note** (string?, 400 chars)

**Navigation Properties:**
- NewsTags (many-to-many relationship with NewsArticle)

### 4. NewsArticle.cs
- **NewsArticleID** (string, 20 chars, Primary Key)
- **NewsTitle** (string?, 400 chars)
- **Headline** (string, 150 chars, Required)
- **CreatedDate** (DateTime?)
- **NewsContent** (string?, 4000 chars)
- **NewsSource** (string?, 400 chars)
- **CategoryID** (short?, Foreign Key to Category)
- **NewsStatus** (bool?)
- **CreatedByID** (short?, Foreign Key to SystemAccount)
- **UpdatedByID** (short?, Foreign Key to SystemAccount)
- **ModifiedDate** (DateTime?)

**Navigation Properties:**
- Category
- CreatedBy (SystemAccount)
- UpdatedBy (SystemAccount)
- NewsTags (many-to-many relationship with Tag)

### 5. NewsTag.cs (Junction Table)
- **NewsArticleID** (string, 20 chars, Composite Primary Key)
- **TagID** (int, Composite Primary Key)

**Navigation Properties:**
- NewsArticle
- Tag

## DbContext Configuration

### FUNewsManagementContext.cs
- Configured Entity Framework DbContext
- Proper table name mappings
- Relationship configurations
- Cascade delete behaviors
- Composite primary key for NewsTag

## Database Connection

**Connection String:** Configured in appsettings.json
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(local);Database=FUNewsManagement;Trusted_Connection=true;TrustServerCertificate=true;"
}
```

## Dependencies Added

1. **Microsoft.EntityFrameworkCore.SqlServer** (9.0.6)
2. **Microsoft.EntityFrameworkCore.Tools** (9.0.6)

## Test Page Created

**Categories Page** (`/Categories`)
- Displays all categories from the database
- Shows parent-child relationships
- Demonstrates Entity Framework functionality
- Includes error handling for database connection issues

## Usage

1. Make sure your SQL Server is running
2. Ensure the FUNewsManagement database exists with the provided schema
3. Run the application: `dotnet run`
4. Navigate to `/Categories` to see the models in action

## Next Steps

You can now:
1. Create additional pages for other entities (NewsArticles, Tags, SystemAccounts)
2. Implement CRUD operations
3. Add data validation
4. Create more complex queries using Entity Framework
5. Add authentication and authorization
