using Microsoft.Data.SqlClient;

namespace PmcDashboard.Api.Repositories;

internal static class SqlDataReaderExtensions
{
    public static string GetStringOrEmpty(this SqlDataReader reader, string columnName)
    {
        var value = reader.GetValueOrNull(columnName);
        return value?.ToString()?.Trim() ?? string.Empty;
    }

    public static int GetIntOrDefault(this SqlDataReader reader, string columnName)
    {
        var value = reader.GetValueOrNull(columnName);
        if (value is null)
        {
            return 0;
        }

        return Convert.ToInt32(value);
    }

    public static decimal GetDecimalOrDefault(this SqlDataReader reader, string columnName)
    {
        var value = reader.GetValueOrNull(columnName);
        if (value is null)
        {
            return 0;
        }

        return Convert.ToDecimal(value);
    }

    public static DateTime? GetDateTimeOrDefault(this SqlDataReader reader, string columnName)
    {
        var value = reader.GetValueOrNull(columnName);
        if (value is null)
        {
            return null;
        }

        if (value is DateTime dateTime)
        {
            return dateTime;
        }

        return DateTime.TryParse(value.ToString(), out var parsed) ? parsed : null;
    }

    private static object? GetValueOrNull(this SqlDataReader reader, string columnName)
    {
        var ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? null : reader.GetValue(ordinal);
    }
}
