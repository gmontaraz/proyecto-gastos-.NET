# FinTrack

Aplicación para controlar gastos e ingresos personales. La he hecho para practicar ASP.NET Core, Entity Framework y JWT. Puedes probar la demo gratuita aqui: https://proyecto-gastos-net.onrender.com/login.html

Puedes registrarte, apuntar movimientos con su categoría y ver un resumen del mes con una gráfica de en qué te gastas el dinero.

## Tecnologías

- .NET 9 (ASP.NET Core Web API)
- Entity Framework Core con SQLite
- Autenticación con JWT y contraseñas con BCrypt
- Swagger
- xUnit para los tests
- Frontend con HTML, JavaScript, Bootstrap y Chart.js

## Cómo ejecutarlo

```
dotnet run --project src/FinTrack.Api
```

Y abrir http://localhost:5103

La base de datos se crea sola al arrancar y se mete un usuario de prueba:

- Email: demo@fintrack.com
- Contraseña: demo123

Swagger: http://localhost:5103/swagger

Para los tests:

```
dotnet test
```

También se puede ejecutar con Docker:

```
docker build -t fintrack .
docker run -p 8080:8080 fintrack
```

## Endpoints

| Método | Ruta | |
|---|---|---|
| POST | /api/auth/register | Registro |
| POST | /api/auth/login | Login (devuelve el token) |
| GET | /api/categories | Mis categorías |
| POST | /api/categories | Crear categoría |
| DELETE | /api/categories/{id} | Borrar categoría |
| GET | /api/transactions?year=&month= | Movimientos de un mes |
| GET | /api/transactions/{id} | Un movimiento |
| POST | /api/transactions | Crear movimiento |
| PUT | /api/transactions/{id} | Editar movimiento |
| DELETE | /api/transactions/{id} | Borrar movimiento |
| GET | /api/transactions/summary?year=&month= | Resumen del mes |

Todos menos los de auth necesitan el token en la cabecera `Authorization: Bearer <token>`.

## Cosas por hacer

- Editar movimientos desde la web (en la API ya está)
- Presupuestos por categoría
- Gráfica de varios meses
- Exportar a Excel
