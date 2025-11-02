# UnganaConnect Frontend

ASP.NET Core MVC frontend application for UnganaConnect platform.

## Features

- User Authentication (Login/Register)
- Course Management (CRUD operations)
- Resource Management (CRUD operations)
- Responsive Bootstrap UI
- Session-based authentication
- API integration with backend

## Setup

1. Navigate to Frontend directory
2. Run `dotnet restore`
3. Update `appsettings.json` with correct API URL
4. Run `dotnet run`

## Project Structure

- **Controllers**: Handle HTTP requests and responses
- **Models**: View models for data binding
- **Views**: Razor pages for UI
- **Services**: API communication service
- **wwwroot**: Static files (CSS, JS)

## API Endpoints Connected

- Auth: `/api/auth/login`, `/api/auth/register`
- Courses: `/api/course/get_course`, `/api/course/create_course`, etc.
- Resources: `/api/resource` (CRUD operations)

## Usage

1. Register/Login to access the platform
2. Browse courses and resources
3. Admin users can create/edit/delete courses
4. All authenticated users can manage resources