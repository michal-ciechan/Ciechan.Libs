using System;
using Ciechan.Libs.Collections.Converters.Interfaces;

namespace Ciechan.Libs.Collections.Converters.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnConverterAttribute : Attribute
    {
        public Type Type { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnConverterType">Type inheriting from <see cref="IColumnConverter"/></param>
        public ColumnConverterAttribute(Type columnConverterType)
        {
            Type = columnConverterType;
        }
    }
}