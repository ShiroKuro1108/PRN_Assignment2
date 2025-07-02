# FU News Management System - PRN222 Assignment 02

## Project Overview

This is a comprehensive News Management System built with ASP.NET Core Razor Pages and SignalR, implementing a 3-layer architecture with Repository and Singleton patterns. The system provides role-based authentication and real-time communication for news management operations.

## 🏗️ Architecture

### 3-Layer Architecture
- **Presentation Layer**: ASP.NET Core Razor Pages (`QuangThienDungRazorPages`)
- **Business Logic Layer**: Service classes with validation (`QuangThienDung.Business`)
- **Data Access Layer**: Repository pattern with Entity Framework (`QuangThienDung.DataAccess`)

### Design Patterns Implemented
- **Repository Pattern**: Abstraction layer for data access
- **Unit of Work Pattern**: Manages transactions across repositories
- **Singleton Pattern**: Applied to service registrations
- **Dependency Injection**: Used throughout the application

## 🔐 Authentication & Authorization

### Default Admin Account
- **Email**: admin@FUNewsManagementSystem.org
- **Password**: @@abc123@@
- **Role**: Admin (configured in appsettings.json)

### User Roles
1. **Admin (Role = 3)**
   - Manage account information
   - Create statistics reports by date range
   - View all system data

2. **Staff (Role = 1)**
   - Manage categories (CRUD with business rules)
   - Manage news articles with tags (CRUD with real-time updates)
   - Manage personal profile
   - View personal news history

3. **Lecturer (Role = 2)**
   - View active news articles only
   - Search and filter news content

## 🚀 Key Features

### Real-time Communication (SignalR)
- Live notifications for news article operations
- Real-time updates when articles are created, updated, or deleted
- Connected users receive instant notifications

### CRUD Operations with Popup Dialogs
- Create/Update operations use modal dialogs
- Delete operations include confirmation dialogs
- Form validation with client and server-side validation

### Search and Filtering
- Search functionality across all entities
- Advanced filtering options
- Date range filtering for reports

### Data Validation
- Comprehensive validation rules
- Business logic validation
- Email uniqueness checks
- Category dependency validation

## 📊 Database Schema

### Tables
- **SystemAccount**: User accounts with role-based access
- **Category**: News categories with hierarchical structure
- **NewsArticle**: Main news content with metadata
- **Tag**: Article tags for categorization
- **NewsTag**: Many-to-many relationship between news and tags

### Key Relationships
- Categories can have parent-child relationships
- News articles belong to one category
- News articles can have multiple tags
- Users can create and update multiple news articles

## 🛠️ Technology Stack

- **Framework**: ASP.NET Core 8.0
- **UI**: Razor Pages with Bootstrap 5
- **Database**: SQL Server with Entity Framework Core
- **Real-time**: SignalR
- **Authentication**: Cookie Authentication
- **Icons**: Font Awesome 6
- **Client Libraries**: jQuery, Bootstrap JS

## 📁 Project Structure

```
QuangThienDung_SE18B.NET_A02/
├── QuangThienDung.DataAccess/
│   ├── Models/                 # Entity models
│   ├── Data/                   # DbContext
│   └── Repository/             # Repository interfaces and implementations
├── QuangThienDung.Business/
│   └── Services/               # Business logic services
└── QuangThienDungRazorPages/
    ├── Pages/
    │   ├── Admin/              # Admin-only pages
    │   ├── Staff/              # Staff-only pages
    │   ├── Lecturer/           # Lecturer-only pages
    │   └── Shared/             # Shared layouts
    ├── Hubs/                   # SignalR hubs
    └── wwwroot/
        └── js/                 # JavaScript files
```

## 🔧 Configuration

### Connection String
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(local);Database=FUNewsManagement;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

### Admin Account Configuration
```json
{
  "AdminAccount": {
    "Email": "admin@FUNewsManagementSystem.org",
    "Password": "@@abc123@@",
    "Role": 3
  }
}
```

## 🚦 Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server 2012 or later
- Visual Studio 2019 or later

### Setup Instructions
1. **Clone the repository**
2. **Create the database** using the provided SQL script
3. **Update connection string** in appsettings.json
4. **Build the solution**: `dotnet build`
5. **Run the application**: `dotnet run`
6. **Access the application** at `http://localhost:5252`

### First Login
Use the default admin credentials to access the system:
- Navigate to the login page (default page)
- Enter admin credentials
- Explore role-based features

## 📋 Features by Role

### Admin Features
- **Account Management**: Full CRUD operations for user accounts
- **Reports**: Generate statistics reports with date filtering
- **Export**: Download reports in CSV format
- **System Overview**: View all system statistics

### Staff Features
- **Category Management**: CRUD with dependency validation
- **News Management**: Full CRUD with real-time updates
- **Profile Management**: Update personal information
- **News History**: View personal article history with filtering

### Lecturer Features
- **News Viewing**: Browse active news articles
- **Search & Filter**: Find specific content
- **Detailed View**: Read full article content

## 🔄 Real-time Features

The system implements SignalR for real-time communication:
- **News Creation**: Instant notifications to all connected users
- **News Updates**: Live updates when articles are modified
- **News Deletion**: Immediate removal notifications
- **User Activity**: Track who performed which actions

## 🛡️ Security Features

- **Role-based Authorization**: Strict access control
- **Anti-forgery Tokens**: CSRF protection
- **Input Validation**: Comprehensive data validation
- **Secure Authentication**: Cookie-based authentication
- **Business Rule Enforcement**: Server-side validation

## 📈 Performance Considerations

- **Lazy Loading**: Efficient data loading
- **Caching**: Optimized database queries
- **Pagination**: Large dataset handling
- **Async Operations**: Non-blocking operations

## 🧪 Testing

The application includes:
- **Model Validation**: Entity validation rules
- **Business Logic Testing**: Service layer validation
- **Integration Testing**: End-to-end functionality
- **Real-time Testing**: SignalR communication

## 📝 Assignment Requirements Compliance

✅ **3-Layer Architecture**: Implemented with clear separation
✅ **Repository Pattern**: Complete implementation
✅ **Singleton Pattern**: Applied to service registrations
✅ **Role-based Authentication**: Admin, Staff, Lecturer roles
✅ **SignalR Real-time**: News management operations
✅ **CRUD with Popups**: Modal dialogs for operations
✅ **Search Functionality**: Comprehensive search features
✅ **Data Validation**: Client and server-side validation
✅ **Default Admin Account**: Configured in appsettings.json
✅ **Database Integration**: Entity Framework Core
✅ **Business Rules**: Category deletion validation

## 🎯 Future Enhancements

- File upload for news images
- Email notifications
- Advanced reporting features
- Mobile responsive improvements
- API endpoints for external integration

---

**Developed by**: Quang Thien Dung  
**Course**: PRN222  
**Assignment**: 02  
**Framework**: ASP.NET Core Razor Pages with SignalR
