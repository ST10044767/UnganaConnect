# UnganaConnect API Documentation

## Overview
UnganaConnect is a comprehensive web API for African civil society organizations (CSOs) to access ICT training, tools, consulting, and research resources. The API provides centralized training portals, knowledge repositories, consultancy request systems, event management, and community forums.

## Base URL
```
https://localhost:7000/api
```

## Authentication
The API uses JWT (JSON Web Token) authentication. Include the token in the Authorization header:
```
Authorization: Bearer <your-jwt-token>
```

## API Endpoints

### Authentication & User Management

#### Register User
```http
POST /api/Auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "passwordHash": "password123",
  "firstName": "John",
  "lastName": "Doe"
}
```

#### Login
```http
POST /api/Auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "password123"
}
```

#### Get All Users (Admin Only)
```http
GET /api/Auth/users
Authorization: Bearer <admin-token>
```

### Course Management

#### Get All Courses
```http
GET /api/Course/get_course
```

#### Create Course (Admin Only)
```http
POST /api/Course/create_course
Authorization: Bearer <admin-token>
Content-Type: application/json

{
  "title": "Introduction to Digital Literacy",
  "description": "Learn the basics of digital literacy",
  "category": "Digital Skills"
}
```

#### Update Course (Admin Only)
```http
PUT /api/Course/edit_course/{id}
Authorization: Bearer <admin-token>
Content-Type: application/json

{
  "title": "Updated Course Title",
  "description": "Updated description",
  "category": "Updated Category"
}
```

#### Delete Course (Admin Only)
```http
DELETE /api/Course/delete_course/{id}
Authorization: Bearer <admin-token>
```

### Module Management

#### Get Modules by Course
```http
GET /api/Module/course/{courseId}
```

#### Get Single Module
```http
GET /api/Module/{id}
```

#### Create Module (Admin Only)
```http
POST /api/Module
Authorization: Bearer <admin-token>
Content-Type: application/json

{
  "title": "Module 1: Introduction",
  "content": "Module content here",
  "courseId": 1,
  "videoUrl": "https://example.com/video.mp4",
  "fileUrl": "https://example.com/file.pdf"
}
```

#### Update Module (Admin Only)
```http
PUT /api/Module/{id}
Authorization: Bearer <admin-token>
Content-Type: application/json

{
  "title": "Updated Module Title",
  "content": "Updated content",
  "videoUrl": "https://example.com/new-video.mp4"
}
```

#### Delete Module (Admin Only)
```http
DELETE /api/Module/{id}
Authorization: Bearer <admin-token>
```

### Enrollment Management

#### Enroll in Course
```http
POST /api/Enrollment/{courseId}/enroll
Authorization: Bearer <user-token>
```

#### Get User's Enrollments
```http
GET /api/Enrollment/my-enrollments
Authorization: Bearer <user-token>
```

#### Unenroll from Course
```http
DELETE /api/Enrollment/{courseId}/unenroll
Authorization: Bearer <user-token>
```

### Progress Tracking

#### Get Course Progress
```http
GET /api/Progress/course/{courseId}
Authorization: Bearer <user-token>
```

#### Get User Progress Overview
```http
GET /api/Progress/overview
Authorization: Bearer <user-token>
```

#### Mark Module as Completed
```http
POST /api/Progress/complete/{moduleId}
Authorization: Bearer <user-token>
```

#### Mark Module as Incomplete
```http
POST /api/Progress/uncomplete/{moduleId}
Authorization: Bearer <user-token>
```

### Quiz Management

#### Get Quiz by Module
```http
GET /api/Quiz/module/{moduleId}
```

#### Create Quiz (Admin Only)
```http
POST /api/Quiz
Authorization: Bearer <admin-token>
Content-Type: application/json

{
  "title": "Quiz 1",
  "moduleId": 1,
  "questions": [
    {
      "text": "What is digital literacy?",
      "options": [
        {"text": "Option 1"},
        {"text": "Option 2"},
        {"text": "Option 3"},
        {"text": "Option 4"}
      ],
      "correctOptionId": 1
    }
  ]
}
```

#### Submit Quiz
```http
POST /api/Quiz/submit
Authorization: Bearer <user-token>
Content-Type: application/json

{
  "moduleId": 1,
  "userId": "user123",
  "answers": {
    "1": 2,
    "2": 1
  }
}
```

### Certificate Management

#### Get User's Certificates
```http
GET /api/Certificate/my-certificates
Authorization: Bearer <user-token>
```

#### Generate Certificate
```http
POST /api/Certificate/generate/{courseId}
Authorization: Bearer <user-token>
```

#### Download Certificate
```http
GET /api/Certificate/download/{certificateId}
Authorization: Bearer <user-token>
```

#### Get All Certificates (Admin Only)
```http
GET /api/Certificate/all
Authorization: Bearer <admin-token>
```

#### Get Certificate Statistics (Admin Only)
```http
GET /api/Certificate/statistics
Authorization: Bearer <admin-token>
```

### Event Management

#### Get All Events
```http
GET /api/Event?location=Nairobi&startDate=2024-01-01
```

#### Get Single Event
```http
GET /api/Event/{id}
```

#### Create Event (Admin Only)
```http
POST /api/Event
Authorization: Bearer <admin-token>
Content-Type: application/json

{
  "title": "Digital Skills Workshop",
  "description": "Learn essential digital skills",
  "startDate": "2024-02-01T09:00:00Z",
  "endDate": "2024-02-01T17:00:00Z",
  "location": "Nairobi, Kenya"
}
```

#### Update Event (Admin Only)
```http
PUT /api/Event/{id}
Authorization: Bearer <admin-token>
Content-Type: application/json

{
  "title": "Updated Event Title",
  "description": "Updated description",
  "startDate": "2024-02-01T09:00:00Z",
  "endDate": "2024-02-01T17:00:00Z",
  "location": "Updated Location"
}
```

#### Delete Event (Admin Only)
```http
DELETE /api/Event/{id}
Authorization: Bearer <admin-token>
```

#### Register for Event
```http
POST /api/Event/{eventId}/register
Authorization: Bearer <user-token>
```

#### Unregister from Event
```http
DELETE /api/Event/{eventId}/unregister
Authorization: Bearer <user-token>
```

#### Get Event Registrations (Admin Only)
```http
GET /api/Event/{eventId}/registrations
Authorization: Bearer <admin-token>
```

#### Get User's Registered Events
```http
GET /api/Event/my-registrations
Authorization: Bearer <user-token>
```

### Community Forum

#### Get All Threads
```http
GET /api/Forum/threads
```

#### Get Single Thread
```http
GET /api/Forum/threads/{id}
```

#### Create Thread
```http
POST /api/Forum/threads
Authorization: Bearer <user-token>
Content-Type: application/json

{
  "title": "Discussion Topic",
  "content": "Thread content here"
}
```

#### Update Thread
```http
PUT /api/Forum/threads/{id}
Authorization: Bearer <user-token>
Content-Type: application/json

{
  "title": "Updated Title",
  "content": "Updated content"
}
```

#### Delete Thread (Admin Only)
```http
DELETE /api/Forum/threads/{id}
Authorization: Bearer <admin-token>
```

#### Add Reply to Thread
```http
POST /api/Forum/threads/{threadId}/reply
Authorization: Bearer <user-token>
Content-Type: application/json

{
  "content": "Reply content here"
}
```

#### Update Reply
```http
PUT /api/Forum/replies/{id}
Authorization: Bearer <user-token>
Content-Type: application/json

{
  "content": "Updated reply content"
}
```

#### Delete Reply (Admin Only)
```http
DELETE /api/Forum/replies/{id}
Authorization: Bearer <admin-token>
```

#### Upvote Thread
```http
POST /api/Forum/threads/{id}/upvote
Authorization: Bearer <user-token>
```

#### Upvote Reply
```http
POST /api/Forum/replies/{id}/upvote
Authorization: Bearer <user-token>
```

#### Get Pending Threads (Admin Only)
```http
GET /api/Forum/admin/pending
Authorization: Bearer <admin-token>
```

#### Approve Thread (Admin Only)
```http
PUT /api/Forum/admin/approve/{id}
Authorization: Bearer <admin-token>
```

#### Lock Thread (Admin Only)
```http
PUT /api/Forum/admin/lock/{id}
Authorization: Bearer <admin-token>
```

### Resource Management

#### Create Course Folder (Admin Only)
```http
POST /api/Resource/create-folder/{courseName}
Authorization: Bearer <admin-token>
```

#### Upload Resource (Admin Only)
```http
POST /api/Resource/upload/{courseId}
Authorization: Bearer <admin-token>
Content-Type: multipart/form-data

file: [file]
```

#### Get Course Resources
```http
GET /api/Resource/{courseId}
```

#### Download Resource
```http
GET /api/Resource/{courseId}/download/{fileName}
```

#### Delete Resource (Admin Only)
```http
DELETE /api/Resource/{courseId}/delete/{fileName}
Authorization: Bearer <admin-token>
```

### Admin Dashboard

#### Get Dashboard Statistics
```http
GET /api/Admin/dashboard
Authorization: Bearer <admin-token>
```

#### Get All Users (Paginated)
```http
GET /api/Admin/users?page=1&pageSize=10
Authorization: Bearer <admin-token>
```

#### Update User Role
```http
PUT /api/Admin/users/{userId}/role
Authorization: Bearer <admin-token>
Content-Type: application/json

{
  "role": "Admin"
}
```

#### Get Course Analytics
```http
GET /api/Admin/analytics/courses
Authorization: Bearer <admin-token>
```

#### Get Event Analytics
```http
GET /api/Admin/analytics/events
Authorization: Bearer <admin-token>
```

#### Get Forum Analytics
```http
GET /api/Admin/analytics/forum
Authorization: Bearer <admin-token>
```

#### Delete User (Admin Only)
```http
DELETE /api/Admin/users/{userId}
Authorization: Bearer <admin-token>
```

## Response Format

### Success Response
```json
{
  "data": { ... },
  "message": "Success message"
}
```

### Error Response
```json
{
  "error": {
    "message": "Error message",
    "details": "Detailed error information",
    "timestamp": "2024-01-01T00:00:00Z"
  }
}
```

## Status Codes

- `200 OK` - Request successful
- `201 Created` - Resource created successfully
- `400 Bad Request` - Invalid request data
- `401 Unauthorized` - Authentication required
- `403 Forbidden` - Insufficient permissions
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

## Rate Limiting
The API implements rate limiting to ensure fair usage. Limits are applied per user and endpoint.

## CORS
The API supports Cross-Origin Resource Sharing (CORS) for frontend integration.

## Logging
All API requests and responses are logged for monitoring and debugging purposes.

## Security
- JWT tokens expire after 60 minutes
- Passwords are hashed using BCrypt
- All endpoints require appropriate authentication and authorization
- Input validation is enforced on all endpoints
