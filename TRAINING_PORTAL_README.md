# UnganaConnect Training Portal
## Comprehensive Documentation

---

## 📋 **Table of Contents**
1. [Overview](#overview)
2. [Architecture](#architecture)
3. [Models](#models)
4. [Controllers](#controllers)
5. [Views](#views)
6. [Services](#services)
7. [Database Schema](#database-schema)
8. [Features](#features)
9. [Setup & Configuration](#setup--configuration)
10. [API Endpoints](#api-endpoints)

---

## 🎯 **Overview**

UnganaConnect is a comprehensive training portal built with ASP.NET Core 8.0 MVC. It provides a complete learning management system with course management, user authentication, resource sharing, forums, events, and blogging capabilities.

### **Key Features**
- **Learning Management**: Courses, modules, quizzes, certificates
- **User Management**: Authentication, profiles, role-based access
- **Content Management**: Resources, blogs, forums
- **Event Management**: Training events, registrations
- **File Storage**: Azure Blob Storage integration

---

## 🏗️ **Architecture**

### **Technology Stack**
- **Framework**: ASP.NET Core 8.0 MVC
- **Database**: PostgreSQL with Entity Framework Core 9.0
- **Cloud Storage**: Azure Blob Storage
- **Frontend**: Razor Views + Bootstrap CSS
- **Logging**: Serilog
- **Authentication**: Session-based

### **Project Structure**
```
UnganaConnect/
├── Controllers/          # MVC Controllers
├── Models/              # Data Models & ViewModels
├── Views/               # Razor View Templates
├── Services/            # Business Logic Services
├── Data/                # Database Context
├── Migrations/          # EF Core Migrations
├── wwwroot/             # Static Files
└── Properties/          # Configuration
```

---

## 📊 **Models**

### **Core Entities**

#### **User Model**
```csharp
public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Role { get; set; } = "Member"; // Admin/Member
    
    // Profile Information
    public string? ProfilePictureUrl { get; set; }
    public string? Bio { get; set; }
    public string? Phone { get; set; }
    public string? Organization { get; set; }
    public string? Location { get; set; }
    public string? Website { get; set; }
    
    // Relationships
    public ICollection<CourseEnrollment> Enrollments { get; set; }
}
```

#### **Course Model**
```csharp
public class Course
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public string Duration { get; set; }
    public string Level { get; set; }
    public double Rating { get; set; }
    public int Enrolled { get; set; }
    public string Status { get; set; } = "available";
    
    // Media
    public string ThumbnailUrl { get; set; }
    public string ThumbnailFileName { get; set; }
    
    // Relationships
    public ICollection<Module> Modules { get; set; }
}
```

#### **Forum Models**
```csharp
public class ForumCategory
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Color { get; set; }
    public List<ForumTopic> Topics { get; set; }
}

public class ForumTopic
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string Author { get; set; }
    public DateTime CreatedAt { get; set; }
    public int Replies { get; set; }
    public int Views { get; set; }
    public bool IsPinned { get; set; }
    public bool IsAnswered { get; set; }
    public string Tags { get; set; }
    public int CategoryId { get; set; }
    public List<ForumReply> RepliesList { get; set; }
}
```

#### **Event Model**
```csharp
public class Event
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Type { get; set; }
    public string Format { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan Time { get; set; }
    public string Duration { get; set; }
    public string Price { get; set; }
    public string Instructor { get; set; }
    public int Participants { get; set; }
    public int MaxParticipants { get; set; }
    public string Level { get; set; }
    public string Location { get; set; }
    public string Description { get; set; }
    
    // JSON Lists
    public List<string> Agenda { get; set; }
    public List<string> Materials { get; set; }
    public List<string> Tags { get; set; }
    
    public List<EventRegistration> Registrations { get; set; }
}
```

#### **Blog Model**
```csharp
public class BlogPost
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string? Excerpt { get; set; }
    public string? FeaturedImageUrl { get; set; }
    public string Category { get; set; } = "General";
    public string? Tags { get; set; }
    public string Status { get; set; } = "Draft";
    public int Views { get; set; }
    public bool IsFeatured { get; set; }
    public bool AllowComments { get; set; }
    public Guid AuthorId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
}
```

---

## 🎮 **Controllers**

### **AuthController**
- **Purpose**: User authentication and registration
- **Actions**:
  - `Login()` - User login form and processing
  - `Register()` - User registration
  - `Logout()` - Session termination

### **HomeController**
- **Purpose**: Dashboard and main navigation
- **Actions**:
  - `Index()` - Landing page
  - `Member()` - Member dashboard with stats
  - `Admin()` - Admin dashboard with system stats

### **CourseController**
- **Purpose**: Course management and learning
- **Actions**:
  - `Index()` - Course listing
  - `Details(id)` - Course details and enrollment
  - `Create()` - Course creation (Admin)
  - `Edit(id)` - Course editing (Admin)
  - `Learn(id)` - Course learning interface
  - `CreateModule(courseId)` - Module creation
  - `CreateQuiz(moduleId)` - Quiz creation
  - `TakeQuiz(quizId)` - Quiz taking interface
  - `Certificate(courseId)` - Certificate generation

### **ResourceController**
- **Purpose**: Resource library management
- **Actions**:
  - `Index()` - Resource listing
  - `Details(id)` - Resource details
  - `Create()` - Resource upload
  - `Edit(id)` - Resource editing

### **ForumController**
- **Purpose**: Discussion forum management
- **Actions**:
  - `Index()` - Forum categories and topics
  - `Topic(id)` - Topic details and replies
  - `CreateTopic()` - New topic creation
  - `EditTopic(id)` - Topic editing
  - `DeleteTopic(id)` - Topic deletion

### **EventController**
- **Purpose**: Event management and registration
- **Actions**:
  - `Index()` - Event listing
  - `Details(id)` - Event details
  - `Create()` - Event creation (Admin)
  - `Edit(id)` - Event editing (Admin)
  - `Register(id)` - Event registration
  - `Delete(id)` - Event deletion (Admin)

### **BlogController**
- **Purpose**: Blog post management
- **Actions**:
  - `Index()` - Blog post listing
  - `Details(id)` - Blog post details
  - `Create()` - Blog post creation
  - `Edit(id)` - Blog post editing
  - `Delete(id)` - Blog post deletion

### **ProfileController**
- **Purpose**: User profile management
- **Actions**:
  - `Index()` - Profile view
  - `Edit()` - Profile editing

---

## 🖼️ **Views**

### **Layout Structure**
- **_Layout.cshtml** - Main layout template
- **_NavbarAdmin.cshtml** - Admin navigation
- **_NavbarMember.cshtml** - Member navigation
- **_ValidationScriptsPartial.cshtml** - Client-side validation

### **View Organization**

#### **Auth Views**
- `Login.cshtml` - Login form
- `Register.cshtml` - Registration form

#### **Course Views**
- `Index.cshtml` - Course catalog
- `Details.cshtml` - Course information
- `Create.cshtml` - Course creation form
- `Edit.cshtml` - Course editing form
- `Learn.cshtml` - Learning interface
- `CreateModule.cshtml` - Module creation
- `CreateQuiz.cshtml` - Quiz creation
- `TakeQuiz.cshtml` - Quiz interface
- `Certificate.cshtml` - Certificate display

#### **Forum Views**
- `Index.cshtml` - Forum categories and topics
- `Topic.cshtml` - Topic discussion
- `CreateTopic.cshtml` - Topic creation form
- `EditTopic.cshtml` - Topic editing form

#### **Event Views**
- `Index.cshtml` - Event calendar
- `Details.cshtml` - Event information
- `Create.cshtml` - Event creation form
- `Register.cshtml` - Event registration form

#### **Blog Views**
- `Index.cshtml` - Blog listing
- `Details.cshtml` - Blog post view
- `Create.cshtml` - Blog creation form
- `Edit.cshtml` - Blog editing form

#### **Dashboard Views**
- `Home/Member.cshtml` - Member dashboard
- `Home/Admin.cshtml` - Admin dashboard

---

## 🔧 **Services**

### **ApiService**
```csharp
public class ApiService
{
    // HTTP client for external API communication
    public async Task<T?> GetAsync<T>(string endpoint, string? token = null)
    public async Task<HttpResponseMessage> PostAsync<T>(string endpoint, T data, string? token = null)
    public async Task<HttpResponseMessage> PutAsync<T>(string endpoint, T data, string? token = null)
    public async Task<HttpResponseMessage> DeleteAsync(string endpoint, string? token = null)
}
```

### **AzureBlobService**
```csharp
public class AzureBlobService
{
    // File upload/download to Azure Storage
    public async Task<string> UploadFileAsync(IFormFile file, string containerName)
    public async Task<Stream> DownloadFileAsync(string fileName, string containerName)
    public async Task DeleteFileAsync(string fileName, string containerName)
}
```

---

## 🗄️ **Database Schema**

### **Core Tables**
- **Users** - User accounts and profiles
- **Courses** - Course information
- **Modules** - Course modules/lessons
- **Quizzes** - Quiz definitions
- **QuizQuestions** - Quiz questions
- **QuizOptions** - Multiple choice options
- **QuizAttempts** - User quiz attempts
- **CourseEnrollments** - User course enrollments
- **Resources** - Resource library items
- **ForumCategories** - Forum categories
- **ForumTopics** - Forum topics
- **ForumReplies** - Topic replies
- **Events** - Training events
- **EventRegistrations** - Event registrations
- **BlogPosts** - Blog articles

### **Relationships**
- User → CourseEnrollments (1:Many)
- Course → Modules (1:Many)
- Module → Quizzes (1:Many)
- Quiz → QuizQuestions (1:Many)
- ForumCategory → ForumTopics (1:Many)
- ForumTopic → ForumReplies (1:Many)
- Event → EventRegistrations (1:Many)
- User → BlogPosts (1:Many)

---

## ✨ **Features**

### **Learning Management**
- **Course Catalog**: Browse and search courses
- **Enrollment System**: Enroll in courses
- **Module-based Learning**: Sequential content delivery
- **Quiz System**: Assessments with multiple attempts
- **Progress Tracking**: Course completion tracking
- **Certificates**: Downloadable completion certificates

### **User Management**
- **Authentication**: Login/Register system
- **Role-based Access**: Admin and Member roles
- **User Profiles**: Comprehensive profile management
- **Session Management**: Secure session handling

### **Content Management**
- **Resource Library**: File upload and sharing
- **Blog System**: Article publishing
- **Forum Discussions**: Community interaction
- **Event Management**: Training event organization

### **File Storage**
- **Azure Integration**: Cloud file storage
- **Media Support**: Images, videos, documents
- **Secure Upload**: File validation and security

---

## ⚙️ **Setup & Configuration**

### **Prerequisites**
- .NET 8.0 SDK
- PostgreSQL Database
- Azure Storage Account

### **Installation**
1. **Clone Repository**
   ```bash
   git clone <repository-url>
   cd UnganaConnect
   ```

2. **Install Dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure Environment**
   ```bash
   # Create .env file
   DefaultConnection=Host=localhost;Database=unganaconnect;Username=user;Password=pass
   AzureBlobStorage=DefaultEndpointsProtocol=https;AccountName=...
   ```

4. **Database Migration**
   ```bash
   dotnet ef database update
   ```

5. **Run Application**
   ```bash
   dotnet run
   ```

### **Configuration Files**
- **appsettings.json** - Application settings
- **appsettings.Development.json** - Development settings
- **.env** - Environment variables
- **Program.cs** - Application startup

---

## 🌐 **API Endpoints**

### **Authentication**
- `POST /Auth/Login` - User login
- `POST /Auth/Register` - User registration
- `POST /Auth/Logout` - User logout

### **Courses**
- `GET /Course` - List courses
- `GET /Course/Details/{id}` - Course details
- `POST /Course/Create` - Create course (Admin)
- `PUT /Course/Edit/{id}` - Update course (Admin)
- `POST /Course/Enroll/{id}` - Enroll in course

### **Resources**
- `GET /Resource` - List resources
- `POST /Resource/Create` - Upload resource
- `GET /Resource/Download/{id}` - Download resource

### **Forums**
- `GET /Forum` - Forum index
- `GET /Forum/Topic/{id}` - Topic details
- `POST /Forum/CreateTopic` - Create topic
- `POST /Forum/Reply` - Reply to topic

### **Events**
- `GET /Event` - List events
- `POST /Event/Register/{id}` - Register for event
- `POST /Event/Create` - Create event (Admin)

### **Blog**
- `GET /Blog` - List blog posts
- `GET /Blog/Details/{id}` - Blog post details
- `POST /Blog/Create` - Create blog post

---

## 🔒 **Security Features**

### **Authentication & Authorization**
- Session-based authentication
- Role-based access control (Admin/Member)
- Secure password hashing
- HTTPS enforcement

### **Data Protection**
- Environment variable configuration
- Secure file upload validation
- SQL injection prevention (EF Core)
- XSS protection (Razor encoding)

### **File Security**
- File type validation
- Size limitations
- Secure Azure Blob storage
- Access control

---

## 📈 **Performance & Scalability**

### **Optimization Features**
- Async/await patterns
- Database connection pooling
- Static file caching
- Lazy loading for relationships

### **Monitoring & Logging**
- Serilog structured logging
- Request/response logging
- Error tracking
- Performance metrics

### **Deployment**
- Docker containerization
- Environment-based configuration
- Cloud-ready architecture
- Horizontal scaling support

---

## 🚀 **Getting Started**

1. **Access the Portal**: Navigate to the application URL
2. **Register Account**: Create a new user account
3. **Explore Courses**: Browse the course catalog
4. **Enroll & Learn**: Enroll in courses and start learning
5. **Participate**: Join forum discussions and events
6. **Track Progress**: Monitor your learning progress

### **Admin Features**
- Course management
- User administration
- Content moderation
- System analytics

This comprehensive training portal provides a complete learning management solution with modern web technologies and cloud integration.