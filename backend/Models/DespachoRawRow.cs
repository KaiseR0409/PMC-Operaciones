namespace PmcDashboard.Api.Models;

public sealed class DespachoRawRow
{
    public string Sucursal { get; set; } = string.Empty;
    public string Bodega { get; set; } = string.Empty;
    public string NGuia { get; set; } = string.Empty;
    public string Producto { get; set; } = string.Empty;
    public string Mezcla { get; set; } = string.Empty;
    public string Envase { get; set; } = string.Empty;
    public decimal Sacos { get; set; }
    public string FormaEntrega { get; set; } = string.Empty;
    public string TipoProducto { get; set; } = string.Empty;
    public int Ano { get; set; }
    public string Mes { get; set; } = string.Empty;
    public DateTime? Fecha { get; set; }
    public string CodTurno { get; set; } = string.Empty;
    public string Turno { get; set; } = string.Empty;
    public string Manifiesto { get; set; } = string.Empty;
    public string Ddi { get; set; } = string.Empty;
    public decimal Rt { get; set; }
    public decimal Drm { get; set; }
    public string Patente { get; set; } = string.Empty;
    public string Conductor { get; set; } = string.Empty;
    public decimal Despacho { get; set; }
    public string Destino { get; set; } = string.Empty;
    public string Ticket { get; set; } = string.Empty;
    public string NPedido { get; set; } = string.Empty;
    public string Usuario { get; set; } = string.Empty;
    public int Camiones { get; set; }
    public int SemanaAno { get; set; }
    public int SemanaMes { get; set; }
    public string Formato { get; set; } = string.Empty;
    public int Dia { get; set; }
    public string TipoDespacho { get; set; } = string.Empty;
    public int Correlativo { get; set; }
}
