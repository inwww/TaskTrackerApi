# Task Tracker API - Microservice

A distributed task management microservice built with **ASP.NET Core 8**, demonstrating advanced C# features and microservice architecture patterns.

## Features

### ✨ Block 1: Domain Model & Events

- Abstract base class `BaseTask` with read-only `Id` and `CreatedAt` properties
- Derived classes: `BugReportTask` and `FeatureRequestTask` with specialized properties
- Event-driven architecture with `OnTaskCompleted` event
- Static `TaskFilterService` with LINQ-based task filtering and analysis
- Modern C# features: Records, Pattern Matching, Init-only Properties

### 🚀 Block 2: Web API & Architecture

- RESTful API endpoints for task management
- Dependency Injection pattern with `ITaskRepository` interface
- In-memory repository implementation with sample data
- Clean separation of concerns: Controller → Service → Repository layers
- Comprehensive logging with structured logging support
- Swagger/OpenAPI documentation

### 📧 Block 3: Integration Patterns

- Detailed analysis document for NotificationService integration
- Recommendation: **Asynchronous event-driven architecture** using RabbitMQ
- Design patterns: Pub/Sub, Dead Letter Queues, Message Persistence
- Technology recommendations: RabbitMQ, SendGrid, AWS SES

### 🐳 Containerization

- Multi-stage Dockerfile for optimized image size
- `docker-compose.yml` with RabbitMQ integration setup
- Health checks for production readiness
- Supports both HTTP and HTTPS

---

## API Endpoints

### Task Management

| Method   | Endpoint                      | Description                                                |
| -------- | ----------------------------- | ---------------------------------------------------------- |
| `GET`    | `/api/tasks`                  | Retrieve all tasks                                         |
| `GET`    | `/api/tasks/{id}`             | Get specific task by ID                                    |
| `POST`   | `/api/tasks/bug`              | Create new bug report                                      |
| `POST`   | `/api/tasks/feature`          | Create new feature request                                 |
| `PUT`    | `/api/tasks/{id}/complete`    | Mark task as completed                                     |
| `DELETE` | `/api/tasks/{id}`             | Delete a task                                              |
| `GET`    | `/api/tasks/analysis/summary` | Get analysis of high-severity bugs & total estimated hours |

### Infrastructure

| Method | Endpoint   | Description                   |
| ------ | ---------- | ----------------------------- |
| `GET`  | `/health`  | Health check endpoint         |
| `GET`  | `/swagger` | Interactive API documentation |

---

## Project Structure

```
TaskTrackerApi/
├── Models/
│   ├── BaseTask.cs              # Abstract base class with event system
│   ├── BugReportTask.cs         # Bug report implementation
│   ├── FeatureRequestTask.cs    # Feature request implementation
│   └── SeverityLevel.cs         # Severity enumeration
├── Services/
│   ├── ITaskRepository.cs       # Repository interface
│   ├── InMemoryTaskRepository.cs# In-memory implementation
│   └── TaskFilterService.cs     # LINQ-based filtering
├── Controllers/
│   └── TasksController.cs       # API endpoints
├── Properties/
│   └── launchSettings.json      # Launch configuration
├── Program.cs                   # Application entry point with DI setup
├── Dockerfile                   # Multi-stage Docker build
├── docker-compose.yml           # Container orchestration
├── appsettings.json            # Production settings
├── appsettings.Development.json # Development settings
├── TaskTrackerApi.csproj       # Project configuration
├── NOTIFICATION_PATTERN_ANALYSIS.md # Integration analysis
├── .gitignore                  # Git ignore patterns
└── README.md                   # This file
```

---

## Technology Stack

- **.NET Runtime**: .NET 8.0
- **Framework**: ASP.NET Core 8.0
- **Language**: C# 12 (with modern features: Records, Init-only Properties, Pattern Matching)
- **API Documentation**: Swagger/OpenAPI
- **Containerization**: Docker, Docker Compose
- **Package Manager**: NuGet

### Dependencies

- `Swashbuckle.AspNetCore` - Swagger/OpenAPI support
- `Microsoft.AspNetCore.OpenApi` - OpenAPI integration

---

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- Docker & Docker Compose (for containerized deployment)
- Git

### Local Development

#### 1. Clone the repository

```bash
git clone https://github.com/yourusername/TaskTrackerApi.git
cd TaskTrackerApi
```

#### 2. Restore dependencies

```bash
dotnet restore
```

#### 3. Build the project

```bash
dotnet build
```

#### 4. Run the application

```bash
dotnet run
```

The API will be available at `http://localhost:5000` with Swagger UI at `http://localhost:5000/swagger`

#### 5. Run tests (when implemented)

```bash
dotnet test
```

---

## Docker Deployment

### Build and Run with Docker Compose

```bash
# Build the image
docker-compose build

# Start all services
docker-compose up -d

# View logs
docker-compose logs -f task-api

# Stop services
docker-compose down
```

#### Services Started:

- **Task API**: http://localhost:8080
- **Swagger UI**: http://localhost:8080/swagger
- **RabbitMQ Management**: http://localhost:15672 (guest/guest)

---

## Code Examples

### Creating a Bug Report

```bash
curl -X POST http://localhost:5000/api/tasks/bug \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Database connection timeout",
    "severityLevel": 3
  }'
```

### Completing a Task

```bash
curl -X PUT http://localhost:5000/api/tasks/{taskId}/complete
```

### Getting Task Analysis

```bash
curl -X GET http://localhost:5000/api/tasks/analysis/summary
```

---

## Advanced C# Features Used

### 1. **Records**

```csharp
public record BugReportTask : BaseTask { ... }
```

- Immutable data carriers with built-in equality
- Concise syntax for DTOs
- Pattern matching support

### 2. **Init-only Properties**

```csharp
public Guid Id { get; init; }
public DateTime CreatedAt { get; init; }
```

- Encapsulation: set only during object construction
- Prevents accidental modifications after creation

### 3. **Pattern Matching**

```csharp
return task switch
{
    BugReportTask bug => new BugReportTaskDto { ... },
    FeatureRequestTask feature => new FeatureRequestTaskDto { ... },
    _ => new TaskDto { ... }
};
```

### 4. **Required Members**

```csharp
public required string Title { get; set; }
```

- Compiler ensures property is always initialized
- Better null safety

### 5. **Events and Delegates**

```csharp
public delegate void TaskCompletedEventHandler(object sender, TaskCompletedEventArgs e);
public event TaskCompletedEventHandler OnTaskCompleted;
```

### 6. **Dependency Injection**

```csharp
public TasksController(ITaskRepository repository, ILogger<TasksController> logger)
{
    _repository = repository;
    _logger = logger;
}
```

---

## Notification Service Integration

This project demonstrates **asynchronous event-driven integration** for future notification services.

See [NOTIFICATION_PATTERN_ANALYSIS.md](./NOTIFICATION_PATTERN_ANALYSIS.md) for detailed analysis of:

- Why asynchronous over synchronous HTTP
- RabbitMQ message broker integration
- Implementation patterns and code examples
- Deployment considerations

---

## Future Enhancements

- [ ] Entity Framework Core with PostgreSQL database
- [ ] Authentication & Authorization with JWT
- [ ] RabbitMQ event publishing implementation
- [ ] Notification Service consumer app
- [ ] Unit & Integration test suite
- [ ] API versioning (v1, v2)
- [ ] Rate limiting & throttling
- [ ] Distributed tracing with OpenTelemetry
- [ ] Performance caching with Redis
- [ ] Kubernetes deployment manifests

---

## Project Statistics

- **Language**: C# 12
- **Lines of Code**: ~600 (core logic)
- **Endpoints**: 7 REST endpoints
- **Classes/Records**: 12
- **Interfaces**: 1 main repository interface
- **Test Coverage**: Ready for comprehensive unit tests

---

## License

This project is provided for educational purposes.

---

## Author

Created as a comprehensive demonstration of:

- Advanced C# and ASP.NET Core features
- Microservice architecture patterns
- Event-driven design
- Containerization best practices
- Professional code organization

---

## Contact & Support

For questions or suggestions about this microservice architecture, feel free to open an issue.

---

**Happy coding! 🚀**
