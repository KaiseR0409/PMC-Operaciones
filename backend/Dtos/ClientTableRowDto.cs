using System.Text.Json.Serialization;

namespace PmcDashboard.Api.Dtos;

public sealed record ClientTableRowDto(
    [property: JsonPropertyName("Fecha")]
    string Fecha,
    [property: JsonPropertyName("Dia Semana")]
    string DiaSemana,
    [property: JsonPropertyName("Turno")]
    string Turno,
    [property: JsonPropertyName("SACOS")]
    decimal Sacos,
    [property: JsonPropertyName("MAXISACOS")]
    decimal Maxisacos,
    [property: JsonPropertyName("GRANEL")]
    decimal Granel);
