using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Xerris.DotNet.Data;

public sealed class DateTimeConverter() : ValueConverter<DateTime, DateTime>(v => v.ToUniversalTime(), v => DateTime.SpecifyKind(v, DateTimeKind.Utc));