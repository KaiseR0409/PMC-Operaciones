# PMC Operaciones

Repositorio del dashboard operativo PMC, organizado en dos proyectos:

- `backend/`: API ASP.NET Core que consulta el procedimiento almacenado y expone endpoints `/analytics`.
- `frontend/`: dashboard Vue/Vite adaptado para consumir la API actual, sin carga de archivos Excel.

## Ejecutar backend

```bash
cd backend
dotnet run
```

URL local esperada:

```txt
http://localhost:5156
```

## Ejecutar frontend

```bash
cd frontend
npm install
npm run dev
```

Crear `frontend/.env` usando `frontend/.env.example`:

```env
VITE_API_URL=http://localhost:5156
VITE_ANALYTICS_TOKEN=TOKEN_ANALYTICS
```

## Flujo actual

El frontend carga automaticamente el anio y mes actuales, muestra un aviso del periodo visible y consulta la API con el header `X-Analytics-Token`. Las consultas de filtros usan debounce para evitar llamadas repetidas mientras el usuario interactua.
