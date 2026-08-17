# Back API - JWT, Roles, SQLite, Interceptores y Manejo Global de Errores

API REST en .NET 10 con autenticación JWT, autorización por roles, SQLite, logging global, middleware de errores, interceptores y CRUD de contactos.

## Tabla de contenido

- [Descripción general](#descripción-general)
- [Tecnologías](#tecnologías)
- [Requisitos previos](#requisitos-previos)
- [Estructura del proyecto](#estructura-del-proyecto)
- [Configuración](#configuración)
- [Instalación](#instalación)
- [Ejecución](#ejecución)
- [Endpoints principales](#endpoints-principales)
- [Autenticación y autorización](#autenticación-y-autorización)
- [Base de datos](#base-de-datos)
- [Manejo de errores y logging](#manejo-de-errores-y-logging)
- [Ejemplos de uso](#ejemplos-de-uso)
- [Licencia](#licencia)

## Descripción general

Este proyecto es una API backend en .NET 10 pensada para demostrar buenas prácticas en:

- autenticación con JWT
- autorización por roles
- manejo global de excepciones
- filtros globales para logging y trazabilidad
- SQLite como almacenamiento de usuarios
- arquitectura por capas con controllers, services, repositories y models
- seguridad con claims y policies
- CRUD de contactos con separación de responsabilidades

Incluye:

- login y signup de usuarios
- generación de token JWT
- claim de role en el token
- endpoint admin-only
- middleware global para errores
- filtro global para trazabilidad de requests
- SQLite con EF Core
- base de datos creada automáticamente en desarrollo

## Tecnologías

- .NET 10
- C# 14
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- JWT Bearer Authentication
- ASP.NET Core Authorization Policies
- Scalar API docs
- Dependency Injection
- LINQ / EF Core

## Requisitos previos

Necesitas tener instalado:

1. .NET 10 SDK
2. Visual Studio 2022 / VS Code / Rider
3. Git

## Estructura del proyecto

```text
back/
├── Controllers/
│   ├── AuthController.cs
│   └── ContactoController.cs
├── Core/
│   ├── Interfaces/
│   │   ├── IContactoRepository.cs
│   │   ├── IContactoService.cs
│   │   └── IUserRepository.cs
│   └── Models/
│       ├── Contacto.cs
│       ├── LoginRequest.cs
│       ├── RegisterRequest.cs
│       ├── User.cs
│       └── UserRole.cs
├── Data/
│   └── AppDbContext.cs
├── Filters/
│   └── RequestLoggingActionFilter.cs
├── Middleware/
│   └── GlobalExceptionMiddleware.cs
├── Repositories/
│   ├── ContactoRepository.cs
│   └── UserRepository.cs
├── Services/
│   └── ContactoService.cs
├── app.db
├── appsettings.json
├── back.csproj
├── Program.cs
├── README.md
└── back.http
```

## Configuración

El proyecto usa `appsettings.json` para configurar:

```json
{
 "ConnectionStrings": {
   "DefaultConnection": "Data Source=app.db"
 },
 "JwtSettings": {
   "Issuer": "back-api",
   "Audience": "back-clients",
   "SecretKey": "ThisIsAReallyLongSecretKeyForLocalDevelopment123!"
 }
}
```

### JwtSettings

- `Issuer`: emisor del token
- `Audience`: audiencia esperada del token
- `SecretKey`: clave de firma HMAC para JWT

## Instalación

```bash
git clone <repo-url>
cd <carpeta-del-proyecto>
dotnet restore
```

## Ejecución

Desde la raíz del proyecto:

```bash
dotnet run
```

La API queda disponible en:

- http://localhost:5234
- OpenAPI / Scalar en el entorno de desarrollo

## Endpoints principales

### Auth

#### Signup
```http
POST /api/auth/signup
Content-Type: application/json
```

Body ejemplo:
```json
{
 "name": "Ana",
 "email": "ana@demo.com",
 "password": "P@ssw0rd123"
}
```

#### Login
```http
POST /api/auth/login
Content-Type: application/json
```

Body ejemplo:
```json
{
 "email": "ana@demo.com",
 "password": "P@ssw0rd123"
}
```

Respuesta:
```json
{
 "success": true,
 "token": "eyJ...",
 "expiresAt": "2026-08-16T20:03:54Z",
 "user": "ana@demo.com",
 "role": "User"
}
```

#### Admin check
```http
GET /api/auth/admin-check
Authorization: Bearer <token>
```

Solo disponible para usuarios con rol `Admin`.

### Contactos

```http
GET /api/contacto
GET /api/contacto/{id}
POST /api/contacto
PUT /api/contacto/{id}
DELETE /api/contacto/{id}
```

Todos los endpoints de contactos están protegidos con JWT por política global.

## Autenticación y autorización

### JWT

Se configura en `Program.cs` con `AddAuthentication` y `AddJwtBearer`.

Se valida:

- issuer
- audience
- expiración del token
- firma digital

### Roles

Se usa el claim `ClaimTypes.Role`.

Los roles soportados son:

- `User`
- `Admin`

### Política global

La API aplica un filtro global de autorización por defecto.

Se define en `Program.cs`:

```csharp
builder.Services.AddAuthorization(options =>
{
   options.AddPolicy("JwtPolicy", policy =>
   {
       policy.RequireAuthenticatedUser();
   });

   options.AddPolicy("AdminOnly", policy =>
   {
       policy.RequireAuthenticatedUser();
       policy.RequireRole(nameof(UserRole.Admin));
   });
});
```

Además, el login y signup son públicos con `[AllowAnonymous]`.

## Base de datos

El proyecto usa SQLite para almacenar usuarios.

### Configuración

En `appsettings.json`:

```json
"ConnectionStrings": {
 "DefaultConnection": "Data Source=app.db"
}
```

### DbContext

Archivo: `Data/AppDbContext.cs`

Define la tabla `Users` con:

- Id GUID
- Name
- Email
- PasswordHash
- Role
- CreatedAt
- unique index en Email

### Seed de admin

Se crea automáticamente un usuario administrador en desarrollo:

- email: `admin@demo.com`
- password: `P@ssw0rd`
- role: `Admin`

## Manejo de errores y logging

### Middleware global

Archivo: `Middleware/GlobalExceptionMiddleware.cs`

Captura excepciones no controladas y devuelve un JSON estructurado con:

- success
- statusCode
- message
- detail (solo en algunos casos)

### Filtro global de requests

Archivo: `Filters/RequestLoggingActionFilter.cs`

Registra cada request con:

- método HTTP
- ruta
- duración
- status code
- request id

También agrega headers:

- `X-Request-Id`
- `X-Response-Time-ms`

## Ejemplos de uso

### Login admin
```bash
curl -X POST http://localhost:5234/api/auth/login \
 -H "Content-Type: application/json" \
 -d '{"email":"admin@demo.com","password":"P@ssw0rd"}'
```

### Signup nuevo usuario
```bash
curl -X POST http://localhost:5234/api/auth/signup \
 -H "Content-Type: application/json" \
 -d '{"name":"Ana","email":"ana@demo.com","password":"P@ssw0rd123"}'
```

### Obtener contactos con token
```bash
curl http://localhost:5234/api/contacto \
 -H "Authorization: Bearer <TOKEN>"
```

### Admin only
```bash
curl http://localhost:5234/api/auth/admin-check \
 -H "Authorization: Bearer <TOKEN>"
```

## Licencia

Este proyecto se distribuye bajo la licencia MIT.

---

Hecho con .NET 10 y ASP.NET Core.

