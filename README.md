# Practica2_Teoria

Portal académico para gestión de cursos y matrículas universitarias.
Stack: ASP.NET Core MVC + Identity + EF Core + SQLite + Redis

## Pasos para correr localmente

1. Clonar el repositorio:
   git clone https://github.com/fernandoprincipecuellar/Practica2_Teoria.git

2. Crear `appsettings.Development.json` en la raíz del proyecto con este contenido:
   {
   "Redis": {
   "ConnectionString": "redis-19405.c245.us-east-1-3.ec2.cloud.redislabs.com:19405,password=TU_PASSWORD"
   }
   }

3. Ejecutar migraciones:
   dotnet ef database update

4. Correr la app:
   dotnet run

5. Abrir en el navegador: http://localhost:5238

## Variables de entorno necesarias en Render

| Variable                               | Valor                                                                             |
| -------------------------------------- | --------------------------------------------------------------------------------- |
| `ASPNETCORE_ENVIRONMENT`               | `Production`                                                                      |
| `ASPNETCORE_URLS`                      | `http://0.0.0.0:$PORT`                                                            |
| `ConnectionStrings__DefaultConnection` | `DataSource=/app/app.db`                                                          |
| `Redis__ConnectionString`              | `redis-19405.c245.us-east-1-3.ec2.cloud.redislabs.com:19405,password=TU_PASSWORD` |

## Cómo ejecutar migraciones

Crear una nueva migración:
dotnet ef migrations add NombreMigracion

Aplicar migraciones a la base de datos:
dotnet ef database update

## Credenciales de prueba

- Coordinador: coordinador@universidad.edu / Coord123!

## URL de Render

https://practica2-teoria.onrender.com
