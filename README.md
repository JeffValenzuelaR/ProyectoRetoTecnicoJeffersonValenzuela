# Cómo correr el proyecto

Tutorial de **Jefferson Valenzuela**.

Esta es una plataforma de eventos con dos partes: el **backend** (APIs + base de datos,
todo en Docker) y el **frontend** (una página web para registrar eventos). Seguí los pasos
en orden.

---

## Requisitos

Instalá esto antes de empezar:

1. **Docker Desktop** — y dejalo abierto/corriendo.
2. **Node.js 20 o superior** — solo para el frontend.

---

## Paso 1 — Levantar el backend

Abrí una terminal **en la carpeta del proyecto** (la que contiene `docker-compose.yml`) y ejecutá:

```bash
docker compose up -d
```

La primera vez descarga y arma todo; puede tardar unos minutos. Cuando termina, revisá que
todo esté arriba:

```bash
docker compose ps
```

Esperá a que las filas de `postgres`, `mongo`, `redis` y `rabbitmq` digan **healthy**.

---

## Paso 2 — Levantar el frontend

En **otra terminal**, entrá a la carpeta `frontend` y ejecutá:

```bash
cd frontend
npm install
npm run dev
```

Cuando aparezca `Local: http://localhost:5173/`, abrí esa dirección en el navegador.

---

## Cómo probarlo

1. En la página vas a ver el formulario **"Registrar Evento"**.
2. Completá nombre, fecha, lugar y al menos una zona (nombre, precio, capacidad).
3. Hacé clic en **Guardar**.
4. Si todo está bien, aparece el mensaje *"Evento registrado correctamente"*.

---

## Dónde ver los resultados

| Qué | Dirección |
|-----|-----------|
| Documentación y prueba de la API | http://localhost:8090/swagger |
| Correos de notificación (se capturan, no se envían) | http://localhost:8025 |
| Consola de mensajería RabbitMQ (usuario y clave: `guest`) | http://localhost:15672 |

Cada evento que registrás dispara automáticamente un correo de notificación que vas a ver
en http://localhost:8025.

---

## Detener todo

Frená el frontend con `Ctrl + C` en su terminal. Para el backend:

```bash
docker compose down
```

Los datos quedan guardados. La próxima vez arrancás de nuevo con `docker compose up -d`.
