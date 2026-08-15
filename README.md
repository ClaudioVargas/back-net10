# 🚀 ProyectoSuperAPI (.NET 10)

[![.NET Version](https://shields.io)](https://dotnet.microsoft.com/)
[![Build Status](https://github.com)](https://github.com)
[![License: MIT](https://shields.io)](LICENSE)

> Web API de ejemplo construida con .NET 10. Proyecto porfatolios para demostrar conocimientos en desarrollo backend moderno, el proyecto contiene un CRUD de contacto con sus responsabilidades correctamente separadas
---

## 📋 Tabla de Contenidos

- [Características](#-caracteristicas)
- [Prerrequisitos](#-prerrequisitos)
- [Instalación y Configuración](#-instalación-y-configuración)
- [Ejecución](#-ejecución)
- [Licencia](#-licencia)
- [Parte de la pauta cubierta](#-parte-de-la-pauta-cubierta)

---

## ✨ Características

*   ⚡ **.NET 10 & C# 14:** Aprovecha las últimas mejoras de rendimiento y sintaxis.
*   🚀 **Minimal APIs:** Estructura ligera y rápida.
*   📊 **Entity Framework Core 10:**.
*   🧪 **Unit Testing:** Cobertura de pruebas con MSTest.

---

## 🛠️ Prerrequisitos

Para ejecutar este proyecto, necesitas:

1.  **[.NET 10 SDK](https://microsoft.com)** instalado.
2.  **IDE:** Visual Studio 2022 (17.12+), VS Code, o JetBrains Rider.

---

## 🚀 Instalación y Configuración

1.  **Clonar el repositorio:**
    ```bash
    git clone https://github.com/ClaudioVargas/PruebaTecnicaCarsales.git
    cd PruebaTecnicaCarsales/back
    ```

2.  **Restaurar dependencias:**
    ```bash
    dotnet restore
    ```

---

## 💻 Ejecución

### Desde la línea de comandos
```bash
dotnet run
```


La API estará disponible en `http://localhost:5074/swagger/index.html`.
Tambien se puede probar desde el archivo back.http dando click "Enviar Solicitud" en cada endpoint, donde ya vienen con datos de prueba

---


## 📄 Licencia

Este proyecto está licenciado bajo la licencia MIT. Consulta el archivo `LICENSE` para más detalles.

---
*Hecho con ❤️ en .NET 10*

##  Parte de la pauta cubierta
*   Agregar validaciones obligatorias en Contacto para que Nombre y Teléfono no acepten valores nulos, vacíos o solo espacios.
*   Validar en POST /api/contacto que no exista ya otro contacto con el mismo teléfono.
*   Cambiar el POST para que responda con 201 Created y devuelva el recurso creado o su ubicación.
*   Devolver 400 Bad Request cuando el payload sea inválido.
*   Devolver 409 Conflict cuando se intente crear un contacto duplicado por teléfono.
*   Hacer que GET /api/contacto retorne siempre una colección, incluso vacía, en vez de Ok() sin cuerpo.
*   Separar mejor responsabilidades creando una capa Service entre controller y repository.
*   Inyectar la interfaz IContactoRepository en el controlador, no la implementación concreta.
*   Registrar la interfaz en DI, por ejemplo AddSingleton<IContactoRepository, ContactoRepository>().
*   Resolver la concurrencia del almacenamiento en memoria para evitar carreras al crear contactos en *     paralelo.
*   Proteger la generación de Id y la escritura de la colección con una estrategia thread-safe.
*   Agregar al menos 1 test unitario para reglas como validación o duplicados.
*   Agregar al menos 1 test de integración para endpoints HTTP.
*   Verificar que OpenAPI quede realmente accesible y, si es posible, habilitar Swagger UI para facilitar la prueba manual.
*   Alinear el README del backend con la solución real, incluyendo versión requerida del SDK, pasos de *   ejecución, URL base y ejemplos de endpoints.
*   Corregir la inconsistencia entre el README que pide .NET 8 y el proyecto que apunta a net10.0.
*   Como mejora de puntaje, implementar middleware global de errores.

