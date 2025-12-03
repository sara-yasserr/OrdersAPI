# Orders API

A RESTful API for managing orders with Redis caching, built with .NET 9 and following clean architecture principles.

## 📋 Overview

This project is a simple yet robust Orders Management API that demonstrates:
- Clean Architecture with layered separation (API, Core, Infrastructure)
- Redis caching with 5-minute TTL for improved performance
- Entity Framework Core with SQL Server
- Comprehensive logging with Serilog
- Global exception handling
- Input validation with DTOs
- Swagger/OpenAPI documentation

## 🏗️ Architecture

The project follows Clean Architecture principles with three main layers:

```
OrdersAPI/
├── OrdersAPI.API/          # Presentation Layer
│   ├── Controllers/        # API Controllers
│   ├── DTOs/              # Data Transfer Objects
│   ├── Middleware/        # Custom middleware
│   └── Mappings/          # AutoMapper profiles
├── OrdersAPI.Core/        # Domain Layer
│   ├── Models/            # Domain entities
│   └── Interfaces/        # Repository & service contracts
└── OrdersAPI.Infrastructure/ # Data Layer
    ├── Data/              # DbContext
    ├── Repositories/      # Repository implementations
    └── Services/          # Service implementations
```

## 🚀 Technologies Used

- **.NET 9.0** - Web API Framework
- **Entity Framework Core 9.0** - ORM for database operations
- **SQL Server** - Relational database
- **Redis** - In-memory caching
- **AutoMapper** - Object-to-object mapping
- **Serilog** - Structured logging
- **Swagger/OpenAPI** - API documentation
- **StackExchange.Redis** - Redis client library

## 📦 Prerequisites

Before running this application, ensure you have the following installed:

1. **.NET 9 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/9.0)
2. **SQL Server** (LocalDB, Express, or Full version)
3. **Docker** (Recommended for Redis) - [Download here](https://www.docker.com/get-started)
   - Alternative: Install Redis locally
     - Windows: [Download Redis](https://github.com/microsoftarchive/redis/releases)
     - Linux/Mac: `brew install redis` or `apt-get install redis-server`
4. **Visual Studio 2022** or **VS Code** (optional)

## ⚙️ Configuration

### 1. Database Connection

Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=OrdersDB_Dev;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 2. Redis Configuration

Ensure Redis is running on `localhost:6379` or update the configuration:

```json
{
  "Redis": {
    "Configuration": "localhost:6379",
    "InstanceName": "OrdersAPI_"
  }
}
```

## 🔧 Installation & Setup

### 1. Clone the Repository

```bash
git clone https://github.com/sara-yasserr/OrdersAPI.git
cd OrdersAPI
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Apply Database Migrations

```bash
cd OrdersAPI.API
dotnet ef database update
```

If migrations don't exist, create them:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 4. Start Redis Server

#### Option A: Using Docker (Recommended)

**Pull and run Redis container:**
```bash
docker run -d --name redis-orders -p 6379:6379 redis:latest
```

**Verify Redis is running:**
```bash
docker ps
```

You should see the `redis-orders` container in the list.

**Test Redis connection:**
```bash
docker exec -it redis-orders redis-cli ping
```

Expected response: `PONG`

**Access Redis CLI:**
```bash
docker exec -it redis-orders redis-cli
```

Inside Redis CLI, test some commands:
```redis
127.0.0.1:6379> SET test "Hello Redis"
OK
127.0.0.1:6379> GET test
"Hello Redis"
127.0.0.1:6379> DEL test
(integer) 1
127.0.0.1:6379> exit
```

**Stop Redis container:**
```bash
docker stop redis-orders
```

**Start existing Redis container:**
```bash
docker start redis-orders
```

**Remove Redis container:**
```bash
docker stop redis-orders
docker rm redis-orders
```

#### Option B: Using Local Redis Installation

**Windows:**
```bash
redis-server
```

**Linux/Mac:**
```bash
redis-server
# or
sudo service redis-server start
```

**Test Redis:**
```bash
redis-cli ping
# Expected: PONG
```

### 5. Run the Application

```bash
dotnet run --project OrdersAPI.API
```

The API will be available at:
- **HTTP**: `http://localhost:5130`
- **Swagger UI**: `http://localhost:5130` (opens automatically in development)

## 📡 API Endpoints

### Create Order
```http
POST /api/orders
Content-Type: application/json

{
  "customerName": "John Doe",
  "product": "Laptop",
  "amount": 1299.99
}
```

**Response:** `201 Created`
```json
{
  "orderId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "customerName": "John Doe",
  "product": "Laptop",
  "amount": 1299.99,
  "createdAt": "2024-12-03T10:30:00Z"
}
```

### Get Order by ID
```http
GET /api/orders/{id}
```

**Response:** `200 OK` (from cache if available, otherwise from database)

### Get All Orders
```http
GET /api/orders
```

**Response:** `200 OK` (always from database)

### Delete Order
```http
DELETE /api/orders/{id}
```

**Response:** `204 No Content` (removes from both database and cache)

## 🎯 Features Implemented

### ✅ Task 1 - Orders API
- [x] POST /orders - Create an order
- [x] GET /orders/{id} - Fetch order with Redis cache (5 min TTL)
- [x] GET /orders - List all orders
- [x] DELETE /orders/{id} - Delete from DB and Redis

### ✅ Task 2 - Redis Cache Service
- [x] Get(key) - Retrieve cached data
- [x] Set(key, value, TTL) - Cache data with expiration
- [x] Remove(key) - Delete cached data

### ✅ Task 3 - Database
- [x] Entity Framework Core integration
- [x] SQL Server database
- [x] Database migrations
- [x] Proper entity configuration

### ✅ Task 4 - Code Quality
- [x] Clean architecture structure
- [x] Dependency injection
- [x] Async/await throughout
- [x] Serilog logging (console + file)
- [x] Global exception handling middleware
- [x] Input validation with Data Annotations
- [x] AutoMapper for DTO mapping

## 🧪 Testing the API

### Using Swagger UI
1. Navigate to `http://localhost:5130` in your browser
2. Swagger UI will display all available endpoints
3. Use the "Try it out" button to test each endpoint

### Using cURL

**Create Order:**
```bash
curl -X POST "http://localhost:5130/api/orders" \
  -H "Content-Type: application/json" \
  -d '{
    "customerName": "John Doe",
    "product": "Laptop",
    "amount": 1299.99
  }'
```

**Get Order:**
```bash
curl -X GET "http://localhost:5130/api/orders/{orderId}"
```

## 📝 Logging

Logs are written to:
- **Console** - Real-time logging output
- **File** - `logs/orderapi-YYYYMMDD.txt` (rolling daily)

Log levels can be configured in `appsettings.json`.

## 🔍 Caching Behavior

- **GET /orders/{id}**: First checks Redis cache, falls back to database if not found
- **Cache TTL**: 5 minutes
- **Cache Invalidation**: Automatic on DELETE operations
- **Cache Key Pattern**: `order_{orderId}`

### Verify Caching is Working

You can verify Redis caching by checking the cache directly:

**1. Create an order via API (note the orderId in response)**

**2. Check if it's cached in Redis:**
```bash
# Using Docker
docker exec -it redis-orders redis-cli

# Inside Redis CLI
127.0.0.1:6379> KEYS order_*
127.0.0.1:6379> GET order_{your-order-id}
127.0.0.1:6379> TTL order_{your-order-id}
```

**3. Check application logs:**
- First GET: `"Order {OrderId} retrieved from database and cached"`
- Second GET (within 5 min): `"Order {OrderId} retrieved from cache"`

**4. Wait 5+ minutes and GET again:**
- Cache expired, should retrieve from database again


## 🤝 Contact

**Sara Yasser**  
Email: sarahyasser979@gmail.com  
GitHub: [@sara-yasserr](https://github.com/sara-yasserr)

## 📄 License

This project is for educational/demonstration purposes.

---

*Built with ❤️ using .NET 9 and Redis*