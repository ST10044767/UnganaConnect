# UnganaConnect API

A comprehensive web API for African civil society organizations (CSOs) to access ICT training, tools, consulting, and research resources.

## 🎯 Project Overview

UnganaConnect digitizes the processes of Ungana-Afrika to enable:
- **Broader reach** through online platforms
- **Efficient resource management** with centralized systems
- **Stronger collaboration** between Ungana-Afrika and CSOs

## 🧩 System Modules

### 0. Authentication & Administration
- User profiles and registration
- JWT-based authentication
- Admin panel with analytics

### 1. Training & Learning
- Course catalog and enrollment
- Interactive quizzes and assessments
- Completion certificates
- Video streaming and downloadable resources
- Progress tracking

### 2. Resource Repository
- Document upload and categorization
- Search functionality
- Download tracking and engagement metrics

### 3. Event Management
- Event publishing and management
- Online registration system
- Event reminders and notifications
- Post-event resource sharing

### 4. Community Forum
- Discussion threads and replies
- Upvoting system
- Moderator tools and content approval

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK
- PostgreSQL database
- Azure Storage Account (for file storage)

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd UnganaConnect
   ```

2. **Install dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure environment variables**
   Create a `.env` file in the project root:
   ```env
   DefaultConnection=Host=localhost;Database=UnganaConnect;Username=your_username;Password=your_password
   Jwt__Key=your-super-secret-jwt-key-here
   Jwt__Issuer=UnganaConnect
   Jwt__Audience=UnganaConnect-Users
   Jwt__ExpireMinutes=60
   AzureStorage__ConnectionString=your-azure-storage-connection-string
   ```

4. **Run database migrations**
   ```bash
   dotnet ef database update
   ```

5. **Start the application**
   ```bash
   dotnet run
   ```

The API will be available at `https://localhost:7000`

## 📚 API Documentation

Comprehensive API documentation is available in [API_DOCUMENTATION.md](./API_DOCUMENTATION.md)

### Swagger UI
When running in development mode, Swagger UI is available at:
```
https://localhost:7000/swagger
```

## 🏗️ Project Structure

```
UnganaConnect/
├── Controllers/           # API Controllers
│   ├── Auth/             # Authentication endpoints
│   ├── Course/           # Course management
│   ├── Events/           # Event management
│   ├── Forum/            # Community forum
│   └── Resources/        # Resource management
├── Data/                 # Database context
├── Models/               # Data models
│   ├── Event_Management/
│   ├── Forum/
│   ├── Resources_Repo/
│   ├── Training___Learning/
│   └── Users/
├── Migrations/           # Database migrations
├── Service/              # Business logic services
├── Middleware/           # Custom middleware
└── wwwroot/             # Static files
```

## 🔧 Key Features

### Authentication & Security
- JWT-based authentication
- Role-based authorization (Admin, Member)
- Password hashing with BCrypt
- Global exception handling
- Input validation

### Course Management
- Create and manage courses
- Module-based course structure
- Video and file content support
- Quiz system with automatic grading
- Progress tracking
- Certificate generation

### Event Management
- Event creation and management
- Online registration
- Event analytics
- User event history

### Community Forum
- Thread creation and management
- Reply system
- Upvoting mechanism
- Content moderation
- Admin approval workflow

### Resource Management
- File upload and storage
- Course-specific resource organization
- Download tracking
- File type validation

### Admin Dashboard
- User management
- Analytics and statistics
- Content moderation
- System monitoring

## 🛠️ Technologies Used

- **.NET 8.0** - Backend framework
- **Entity Framework Core** - ORM
- **PostgreSQL** - Database
- **JWT** - Authentication
- **Azure Blob Storage** - File storage
- **QuestPDF** - Certificate generation
- **Serilog** - Logging
- **Swagger** - API documentation

## 📊 Database Schema

The application uses the following main entities:
- **Users** - User accounts and profiles
- **Courses** - Training courses
- **Modules** - Course modules
- **Enrollments** - User course enrollments
- **Quizzes** - Course assessments
- **Certificates** - Course completion certificates
- **Events** - Training events and workshops
- **Threads/Replies** - Forum discussions
- **Resources** - Course materials

## 🔒 Security Features

- JWT token authentication
- Role-based access control
- Password hashing
- Input validation
- CORS configuration
- Global exception handling
- Request logging

## 📈 Monitoring & Logging

- Serilog integration for structured logging
- Console and file logging
- Request/response logging
- Error tracking
- Performance monitoring

## 🚀 Deployment

### Docker Support
The application includes Docker configuration for containerized deployment.

### Environment Configuration
- Development: `appsettings.Development.json`
- Production: `appsettings.json`
- Environment variables via `.env` file

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📞 Support

For support and questions, please contact the development team or create an issue in the repository.

## 🔄 Version History

- **v1.0.0** - Initial release with core functionality
  - Authentication system
  - Course management
  - Event management
  - Community forum
  - Resource management
  - Admin dashboard
