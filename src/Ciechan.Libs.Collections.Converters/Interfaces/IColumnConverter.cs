using System;

namespace Ciechan.Libs.Collections.Converters.Interfaces
{
    public interface IColumnConverter
    {
        object? Convert(object row, object? value, Type targetType);
    }
}