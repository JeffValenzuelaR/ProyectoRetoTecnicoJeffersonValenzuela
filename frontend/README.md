# frontend — Registrar Evento

React 19 + TypeScript + Vite + Tailwind CSS v4. Una sola pantalla: un formulario para
registrar un evento con sus zonas, que llama a `POST /events` de EventService.

## Configurar

```bash
cp .env.example .env
```

Solo hace falta `VITE_API_URL` (por defecto `http://localhost:8090`). El token JWT lo pide
el propio frontend a `POST /auth/token` y lo renueva solo; no hay que pegarlo a mano.

## Correr

```bash
npm install
npm run dev
```

Abre `http://localhost:5173`.

## Build

```bash
npm run build
```
