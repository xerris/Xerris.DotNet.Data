using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Xerris.DotNet.Data.Extensions;

namespace Xerris.DotNet.Data;

public class EnumToDescriptionConverter<TEnum>() : 
    ValueConverter<TEnum, string>(v => v.GetDescription(), v => v.FromDescription<TEnum>()) where TEnum : Enum;