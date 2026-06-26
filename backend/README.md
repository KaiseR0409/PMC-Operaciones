# PMC Dashboard API

Backend ASP.NET Core para consumir el procedimiento almacenado de despachos y exponer endpoints compatibles con el dashboard Vue.

## Configuracion

`appsettings.json` mantiene la llave de configuracion, pero no guarda el secreto:

```json
{
  "ConnectionStrings": {
    "DashboardDatabase": ""
  },
  "StoredProcedures": {
    "DashboardDespachos": ""
  },
  "apiAnalytics": {
    "analyticsToken": ""
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:5173",
      "http://127.0.0.1:5173"
    ]
  }
}
```

El valor real se carga desde `.env`. En ASP.NET, el doble guion bajo `__` representa niveles del JSON. Por eso:

```env
ConnectionStrings__DashboardDatabase="..."
StoredProcedures__DashboardDespachos="dbo.up_sxxx_erl_consulta_despachosPorAno_api"
apiAnalytics__analyticsToken="CHANGE_ME_TO_A_LONG_RANDOM_ANALYTICS_TOKEN"
Cors__AllowedOrigins__0=http://localhost:5173
Cors__AllowedOrigins__1=http://127.0.0.1:5173
```

equivale a configurar:

```json
{
  "ConnectionStrings": {
    "DashboardDatabase": "..."
  }
}
```

Crear o editar `.env`:

```env
ConnectionStrings__DashboardDatabase="Server=SERVIDOR;Database=BASE_DATOS;User Id=USUARIO;Password=PASSWORD;TrustServerCertificate=True;"
StoredProcedures__DashboardDespachos="dbo.up_sxxx_erl_consulta_despachosPorAno_api"
apiAnalytics__analyticsToken="CHANGE_ME_TO_A_LONG_RANDOM_ANALYTICS_TOKEN"
```

Si se usa autenticacion Windows local:

```env
ConnectionStrings__DashboardDatabase="Server=localhost;Database=PMC;Trusted_Connection=True;TrustServerCertificate=True;"
StoredProcedures__DashboardDespachos="dbo.up_sxxx_erl_consulta_despachosPorAno_api"
apiAnalytics__analyticsToken="CHANGE_ME_TO_A_LONG_RANDOM_ANALYTICS_TOKEN"
```

El archivo `.env` esta ignorado por git. Usar `.env.example` como plantilla sin credenciales reales.

## Procedimiento almacenado

El nombre del procedimiento se lee desde:

```env
StoredProcedures__DashboardDespachos="dbo.up_sxxx_erl_consulta_despachosPorAno_api"
```

El repository ejecuta ese procedimiento con parametros:

```sql
EXEC [StoredProcedures__DashboardDespachos]
  @Ano = 2026,
  @Mes = 6;
```

## Endpoints iniciales

Cada request a `/analytics/*` debe enviar:

```http
X-Analytics-Token: ANALYTICS_TOKEN_CONFIGURADO
```

Ejemplo con Axios:

```js
api.defaults.headers.common["X-Analytics-Token"] =
  import.meta.env.VITE_ANALYTICS_TOKEN
```

```txt
GET /analytics/clients?year=2026&month=6
GET /analytics/summary?year=2026&month=6&client=CNA%20CHILE%20SPA
GET /analytics/client-table?year=2026&month=6&client=CNA%20CHILE%20SPA&turno=Primer
GET /analytics/line-chart?year=2026&month=6&client=CNA%20CHILE%20SPA&product=Maxisacos
GET /analytics/truck-chart?year=2026&month=6&client=CNA%20CHILE%20SPA
```

En produccion cambiar `apiAnalytics__analyticsToken` por un valor largo, aleatorio y privado.

## Ejecutar

```bash
dotnet run
```

Swagger queda disponible en la URL indicada por la consola, normalmente:

```txt
https://localhost:7222/swagger
http://localhost:5156/swagger
```
