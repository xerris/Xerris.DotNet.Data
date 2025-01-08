using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Xerris.DotNet.Core.Extensions;
using Xerris.DotNet.Data.Extensions;

namespace Xerris.DotNet.Data.Converters;

public class EnumConverter<TEnum> : ValueConverter<TEnum, string> where TEnum : Enum
{
    public EnumConverter() 
        : base(
            v => v.GetDescription(), 
            v => v.ToEnum<TEnum>())
    {
    }
}