# URL Shortener Service
## Overview
This project implements a simple URL shortener service using ASP.NET Core. The service provides two main functionalities:

1. Shorten URL Endpoint: Accepts a full URL and returns a shortened version.
2. Resolve URL Endpoint: Resolves a shortened URL back to the original URL.

The service uses in-memory storage to store the mappings between original URLs and their shortened versions. Additionally, a cache service is utilized for faster lookups.

## Design Choices
### Core Components
The application is structured around an ASP.NET Core Web API with the following components:

1. Controller (UrlController): Handles HTTP requests and provides the endpoints for shortening and resolving URLs.
2. Service Layer (UrlShortenerService): Contains the business logic for shortening URLs and resolving them.
3. Cache (InMemoryCacheService): Caches the mappings of short URLs to original URLs for quick lookups.
4. Repository Layer (InMemoryUrlRepository): Stores URL mappings in memory using ConcurrentDictionary for thread-safe operations.
5. Validation (UrlRequestValidator): Ensures the provided URL is valid before processing.
6. Middleware (ExceptionHandlingMiddleware): Global exception handler to capture errors and return appropriate responses.

## Short ID Generation Method
To generate a unique short ID, the service uses a GUID-based approach. The original URL is converted to a GUID, then encoded using Base64. The resulting string is truncated to 8 characters to form the short ID. This approach ensures uniqueness, and since GUIDs are inherently random, the risk of collisions is minimal.

## In-Memory Storage
For simplicity and speed, the application uses in-memory storage to map short IDs to original URLs and vice versa. This storage is managed using two _ConcurrentDictionary_ objects:
- One for storing mappings from short IDs to original URLs (__urlMappingsByShortId_).
- Another for storing mappings from original URLs to short IDs (__urlMappingsByOriginalUrl_).
This structure allows fast lookups and thread safety for concurrent operations.

## Cache Service
An in-memory cache is used to store frequently accessed mappings. When a shortened URL is resolved, the service first checks the cache. If the result is not found, it queries the repository and updates the cache for future requests.

## Error Handling
A custom Exception Handling Middleware is used to capture and log exceptions throughout the API. It returns appropriate HTTP status codes based on the exception type:

- 400 Bad Request: Invalid arguments (e.g., empty or malformed URLs).
- 404 Not Found: Resource not found (e.g., unknown short ID).
- 409 Conflict: Duplicate short ID or original URL.
- 500 Internal Server Error: Unexpected server errors.

## Edge Cases Considered
### Invalid URLs
The service ensures that the provided URL is in a valid format by using a regular expression. The _UrlRequestValidator_ class validates URLs and returns a _400 Bad Request_ response if the URL is empty or malformed.

### Duplicate Entries
The service checks for duplicate entries:

- If a URL has already been shortened, it will not be shortened again, and a 409 Conflict will be returned.
- If a short ID already exists in the repository, an exception is thrown to prevent overwriting.

### Empty or Null URLs
The service validates that the URL is not empty or null before processing. A 400 Bad Request is returned for invalid input.

### Shortened URL Not Found
If a shortened URL cannot be resolved (i.e., the short ID does not exist), the service returns a 404 Not Found.

## Instructions for Running the Code Locally
### Prerequisites
Ensure that the following dependencies are installed:
- .NET SDK 8.0 or higher
- Visual Studio or Visual Studio Code (optional, but recommended)

### Steps to Run
1. Clone the repository:
```
git clone https://github.com/abhishekbande/URLShortenerService.git
cd URLShortenerService
```

2. Restore NuGet dependencies:
```
dotnet restore
```

3. Build the project:
```
dotnet build
```

4. Run the application:
```
cd UrlShortner.Api
dotnet run
```

5. The API will be available at http://localhost:5014. You can use tools like Postman or curl to interact with the API.

- POST /api/shorten to shorten a URL.
- GET /api/{short_id} to resolve a shortened URL.

### Sample Requests
POST **/api/shorten**
```
{
  "originalUrl": "https://github.com/"
}
```
GET **/api/{short_id}**
```
https://localhost:5000/api/Url/NzE4NjVio
```

## Dependencies
- FluentValidation: For validating the URL format.
- ASP.NET Core: For building the web API.
- MemoryCache: For in-memory caching of shortened URL mappings.

## Scalability and Reliability Considerations
### Scalability
The current implementation is designed to be simple, using in-memory storage and caching for fast lookups. However, as the service grows and the number of stored URLs increases, the following changes would improve scalability:

### 1. Use a Database:
In-memory storage is not suitable for production environments where persistence and durability are required. A relational database (e.g., SQL Server, PostgreSQL) or a NoSQL database (e.g., MongoDB) can be used to store the URL mappings instead of the _ConcurrentDictionary_.

Advantages:

- Persistent storage, ensuring data is not lost if the application restarts.
- Support for larger data sets and complex queries.
- Better scalability with distributed databases or database clusters.

### 2. Distributed Caching:
Instead of using in-memory caching, a distributed cache like Redis or Memcached can be used to handle scaling across multiple instances of the application. This would ensure that URL mappings are quickly available across all instances.

Advantages:
- High availability and redundancy.
- Scalability across multiple servers or instances.

### 3. Load Balancing and Horizontal Scaling:
As the service scales, it can be deployed on multiple servers with a load balancer distributing the incoming traffic. This will allow the application to handle a larger number of requests simultaneously.

Advantages:
- Increased capacity for handling requests.
- High availability and fault tolerance.

### 4. Rate Limiting:
To prevent abuse and ensure fair usage, implementing rate limiting could be useful. This would restrict the number of requests a user can make in a given time frame.

Implementation:
- Use a middleware to track request rates and reject requests that exceed the allowed limit.

## Reliability
For high availability, consider using a cloud provider like Azure, AWS, or Google Cloud to host the application. Use automated backup mechanisms to ensure the reliability of the database and cache.
Additionally, health checks and monitoring should be set up to track the application's health and catch potential issues early.

## Conclusion
This URL Shortener API demonstrates the implementation of core API features, including URL shortening, resolution, caching, and error handling. By using a simple in-memory repository and cache service, it delivers fast performance for smaller-scale usage. For larger-scale or production environments, suggestions like a database, distributed cache, and horizontal scaling can be adopted.
