using System.Data;
using Microsoft.Data.SqlClient;
using PmcDashboard.Api.Models;

namespace PmcDashboard.Api.Repositories;

public sealed class SqlDashboardRepository(IConfiguration configuration) : IDashboardRepository
{
    private readonly string _connectionString = GetRequiredConnectionString(configuration);
    private readonly string _procedureName = GetRequiredProcedureName(configuration);

    public async Task<IReadOnlyList<DespachoRawRow>> GetDespachosAsync(
        int ano,
        int mes,
        CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(_procedureName, connection)
        {
            CommandType = CommandType.StoredProcedure,
            CommandTimeout = 120
        };

        command.Parameters.Add(new SqlParameter("@Ano", SqlDbType.Int) { Value = ano });
        command.Parameters.Add(new SqlParameter("@Mes", SqlDbType.Int) { Value = mes });

        await connection.OpenAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var rows = new List<DespachoRawRow>();
        while (await reader.ReadAsync(cancellationToken))
        {
            rows.Add(MapRow(reader));
        }

        return rows;
    }

    private static DespachoRawRow MapRow(SqlDataReader reader)
    {
        return new DespachoRawRow
        {
            Sucursal = reader.GetStringOrEmpty("Sucursal"),
            Bodega = reader.GetStringOrEmpty("Bodega"),
            NGuia = reader.GetStringOrEmpty("NGuia"),
            Producto = reader.GetStringOrEmpty("Producto"),
            Mezcla = reader.GetStringOrEmpty("Mezcla"),
            Envase = reader.GetStringOrEmpty("Envase"),
            Sacos = reader.GetDecimalOrDefault("Sacos"),
            FormaEntrega = reader.GetStringOrEmpty("Forma Entrega"),
            TipoProducto = reader.GetStringOrEmpty("Tipo Producto"),
            Ano = reader.GetIntOrDefault("A\u00f1o"),
            Mes = reader.GetStringOrEmpty("Mes"),
            Fecha = reader.GetDateTimeOrDefault("Fecha"),
            CodTurno = reader.GetStringOrEmpty("codTurno"),
            Turno = reader.GetStringOrEmpty("Turno"),
            Manifiesto = reader.GetStringOrEmpty("Manifiesto"),
            Ddi = reader.GetStringOrEmpty("DDI"),
            Rt = reader.GetDecimalOrDefault("RT"),
            Drm = reader.GetDecimalOrDefault("DRM"),
            Patente = reader.GetStringOrEmpty("Patente"),
            Conductor = reader.GetStringOrEmpty("Conductor"),
            Despacho = reader.GetDecimalOrDefault("Despacho"),
            Destino = reader.GetStringOrEmpty("Destino"),
            Ticket = reader.GetStringOrEmpty("Ticket"),
            NPedido = reader.GetStringOrEmpty("NPedido"),
            Usuario = reader.GetStringOrEmpty("Usuario"),
            Camiones = reader.GetIntOrDefault("camiones"),
            SemanaAno = reader.GetIntOrDefault("semanaAno"),
            SemanaMes = reader.GetIntOrDefault("semanaMes"),
            Formato = reader.GetStringOrEmpty("formato"),
            Dia = reader.GetIntOrDefault("dia"),
            TipoDespacho = reader.GetStringOrEmpty("tipoDespacho"),
            Correlativo = reader.GetIntOrDefault("Correlativo")
        };
    }

    private static string GetRequiredConnectionString(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DashboardDatabase");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Missing ConnectionStrings:DashboardDatabase. Set it in .env using ConnectionStrings__DashboardDatabase.");
        }

        return connectionString;
    }

    private static string GetRequiredProcedureName(IConfiguration configuration)
    {
        var procedureName = configuration["StoredProcedures:DashboardDespachos"];
        if (string.IsNullOrWhiteSpace(procedureName))
        {
            throw new InvalidOperationException(
                "Missing StoredProcedures:DashboardDespachos. Set it in .env using StoredProcedures__DashboardDespachos.");
        }

        return procedureName;
    }
}
