# PMC Operaciones Frontend

Dashboard Vue/Vite adaptado para consumir la API ASP.NET Core de `../backend`.

## Configuracion

Crear `.env` desde `.env.example`:

```env
VITE_API_URL=http://localhost:5156
VITE_ANALYTICS_TOKEN=TOKEN_ANALYTICS
```

## Comandos

```sh
npm install
npm run dev
```

Build de produccion:

```sh
npm run build
```

## Notas

El dashboard ya no carga Excel. Al abrir, consulta el anio y mes actuales y muestra el periodo visible en la parte superior. Los cambios de filtros usan debounce para evitar varias consultas seguidas.
