# EventService

API para crear y consultar eventos con sus zonas. Al crear un evento publica un mensaje
`EventCreated` a RabbitMQ para que NotificationService envíe el correo.

.NET 9, separado en capas: Domain (entidades y reglas, sin dependencias externas),
Application (casos de uso con MediatR, validación con FluentValidation, interfaces),
Infrastructure (EF Core + Postgres, Redis, MassTransit) y Api (controllers, JWT, Swagger).

## Endpoints

- `POST /auth/token` — devuelve un JWT de prueba a partir de `{username, role}`. No valida
  credenciales, es solo para poder probar los endpoints protegidos.
- `POST /events` — crea un evento con sus zonas. Requiere rol `Admin`.
- `GET /events` — lista los eventos (cacheado en Redis). Requiere token.
- `GET /events/{id}` — detalle.
- `GET /health` — chequea Postgres y Redis.
- `GET /swagger` — documentación.

## Correr solo este servicio

Necesita Postgres, Redis y RabbitMQ. Lo más simple es levantar todo con el `docker-compose.yml`
de la raíz. Si se corre suelto:

```bash
cd EventService.Api
dotnet run
```

`appsettings.json` usa los nombres de host de docker-compose (`postgres`, `redis`, `rabbitmq`).
Para apuntar a `localhost` se sobreescriben con variables de entorno:
`ConnectionStrings__Postgres`, `RabbitMq__Host`, `Redis__Connection`.

## Base de datos

El esquema está en `db/init-event-service.sql` (generado con EF Core migrations), y Postgres
lo aplica al primer arranque del contenedor. Para regenerarlo tras cambiar entidades:

```bash
dotnet ef migrations add <Nombre> --project EventService.Infrastructure --startup-project EventService.Api
dotnet ef migrations script --project EventService.Infrastructure --startup-project EventService.Api --output ../db/init-event-service.sql --idempotent
```

## Decisiones

- **Outbox transaccional (MassTransit + EF Core):** el mensaje `EventCreated` se guarda en la
  misma transacción que el evento, así no se pierde si RabbitMQ está caído en ese momento.
- **JWT propio (HS256):** el reto lo permite. En un caso real iría contra un IdP con OIDC; el
  middleware de validación no cambiaría, solo la firma.
- **`GET /events` pide token** en vez de dejarlo público, para ser consistente con el resto.

## Docker

El build context es la raíz del repo (no la carpeta del servicio), porque este proyecto
referencia `shared/Contracts`:

```bash
docker build -f event-service/EventService.Api/Dockerfile -t eventservice-api .
```
