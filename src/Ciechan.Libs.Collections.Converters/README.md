## Multi Dimensional Array Converter

Helper class which uses `FastMember` to convert a multi-dimensional array (`object[][]`) into a class using a column ordinals.

Useful when you get data separate from column declaration.

### Example

Given a class

```csharp
public class Sample
{
    public int First { get; set; }
    public string Second { get; set; }
}
```

and data

```csharp
var rows = new[]
  {
      new object[]{1.1, "1.2"}, 
      new object[]{2.1, "2.2"}, 
  };
    
var columns = new[] {"First", "Second"};
```

You can convert this to the sample class using:

```csharp
var samples = rows.Deserialize<Sample>(columns).ToList();
```

This will convert the rows jagged array into a list of sample classes.

### ColumnName Attribute

You can decorate Properties with the `ColumnNameAttribute` to specify which column to look for in the columns array. This is useful when the column contains non valid C# property name characters

The following property

```csharp
[ColumnName("Column.1")]
public int First { get; set; }
```

Will try to bind to a column called `Column.1` rather than `First` .

### DefaultColumnConverter

By default, an instance of MultiDimensionalArrayConverter.DefaultColumnConverter is used.

You can adjust the default conversion settings by either setting `MultiDimensionalArrayConverter.DefaultColumnConverter` to a custom `IColumnConverter` or editing the properties of `MultiDimensionalArrayConverter.ColumnConverter.Instance` which are as follows: 

**- IgnoreInvalidNullableColumnValues**

Set this to true to make the converter handle exceptions when trying to convert into a nullable column.

```csharp
MultiDimensionalArrayConverter.ColumnConverter.Instance.IgnoreInvalidNullableColumnValues = true;
```

### ColumnConverter Attribute

You can decorate Properties with the `ColumnConverterAttribute` passing in a `IColumnConverter` type to use to convert raw row values before setting a property. This is usefull to ensure a "cell" value is of correct type, and/or to convert it.   

For Example, the following decorated property

```csharp
[ColumnConverter(typeof(StringToIntConverter))]
public int First { get; set; }
```

And this converter:

```csharp
public class StringToIntConverter : IColumnConverter
{
    public object Convert(object row, object value, Type targetType)
    {
        return Int32.Parse(value.ToString()!);
    }
}
```

Will call `Convert` on the `StringToIntConverter` before setting the `First` property.

This is useful for converting, coercing, checking, validating column values before the converter attempts to set them.

