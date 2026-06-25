namespace PmcDashboard.Api.Dtos;

public sealed record LineChartPointDto(
    string Fecha,
    int Semana,
    decimal Cantidad);

public sealed record TruckChartPointDto(
    string Fecha,
    int Semana,
    int Camiones);
