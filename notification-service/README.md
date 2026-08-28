# NotificationService

Consume el mensaje `EventCreated` de RabbitMQ y manda un correo. Guarda cada intento en
MongoDB (colección `notificationJobs`) para tener registro de lo que se envió.

.NET 9, mismas capas que EventService. El consumer de MassTransit corre dentro del mismo
proceso de la API; no hice un worker separado.

## Flujo

1. Llega `EventCreated`.
2. Busca el job por `MessageId`. Si ya está en `Sent`, no hace nada (mensaje repetido).
3. Si no existe, lo inserta en `Processing`. El índice único sobre `messageId` en Mongo evita
   duplicados si el mensaje llega dos veces casi al mismo tiempo.
4. Manda el correo (MailKit → MailHog en desarrollo). Si sale bien queda en `Sent`; si falla
   queda en `Failed` y se relanza la excepción para que MassTransit reintente con backoff, y
   mande a la DLQ si se agotan los intentos.

La comprobación de "ya procesado" se hace contra el estado `Sent`, no contra la existencia de
la fila, para que un intento fallido se pueda reintentar en vez de quedar bloqueado.

## Correr solo este servicio

Necesita MongoDB, RabbitMQ y MailHog. Lo normal es usar el `docker-compose.yml` de la raíz.
Suelto:

```bash
cd NotificationService.Api
dotnet run
```

Overrides por variable de entorno: `MongoDb__ConnectionString`, `RabbitMq__Host`, `Smtp__Host`.

## Endpoint de diagnóstico

`GET /notifications` — lista los últimos jobs. Sin autenticación, es solo para revisar el
flujo sin entrar a Mongo a mano.
