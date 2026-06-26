using System.Text.Json.Serialization;

namespace PmcDashboard.Api.Dtos;

public sealed record SummaryDto(
    [property: JsonPropertyName("total_sacos")]
    decimal TotalSacos,
    [property: JsonPropertyName("total_maxisacos")]
    decimal TotalMaxisacos,
    [property: JsonPropertyName("total_granel")]
    decimal TotalGranel,
    [property: JsonPropertyName("clientes_activos")]
    int ClientesActivos,
    [property: JsonPropertyName("registros_totales")]
    int RegistrosTotales,
    [property: JsonPropertyName("variations")]
    SummaryVariationsDto Variations);

public sealed record SummaryVariationsDto(
    [property: JsonPropertyName("sacos")]
    decimal Sacos,
    [property: JsonPropertyName("maxisacos")]
    decimal Maxisacos,
    [property: JsonPropertyName("granel")]
    decimal Granel,
    [property: JsonPropertyName("clientes")]
    decimal Clientes,
    [property: JsonPropertyName("registros")]
    decimal Registros);
