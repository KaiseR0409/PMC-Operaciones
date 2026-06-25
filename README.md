# PMC Dashboard API

Backend ASP.NET Core para consumir el procedimiento almacenado de despachos y exponer endpoints compatibles con el dashboard Vue.

## Configuracion

Crear o editar `.env`:

```env
ConnectionStrings__DashboardDatabase="Server=SERVIDOR;Database=BASE_DATOS;User Id=USUARIO;Password=PASSWORD;TrustServerCertificate=True;"
```

Si se usa autenticacion Windows local:

```env
ConnectionStrings__DashboardDatabase="Server=localhost;Database=PMC;Trusted_Connection=True;TrustServerCertificate=True;"
```

El archivo `.env` esta ignorado por git. Usar `.env.example` como plantilla sin credenciales reales.

## Procedimiento almacenado

El repository ejecuta:

```sql
EXEC dbo.up_sxxx_erl_consulta_despachosPorAno_api
  @Ano = 2026,
  @Mes = 6;
```

## Endpoints iniciales

```txt
GET /analytics/raw?year=2026&month=6
GET /analytics/clients?year=2026&month=6
GET /analytics/summary?year=2026&month=6&client=CNA%20CHILE%20SPA
GET /analytics/client-table?year=2026&month=6&client=CNA%20CHILE%20SPA&turno=Primer
GET /analytics/line-chart?year=2026&month=6&client=CNA%20CHILE%20SPA&product=Maxisacos
GET /analytics/truck-chart?year=2026&month=6&client=CNA%20CHILE%20SPA
```

## Ejecutar

```bash
dotnet run
```

Swagger queda disponible en la URL indicada por la consola, normalmente:

```txt
https://localhost:7088/swagger
http://localhost:5088/swagger
```
