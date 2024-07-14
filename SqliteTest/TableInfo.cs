//using LocalUtilities.SimpleScript.Serialization;
//using SqliteTest;
//using System.Reflection;

//namespace SqliteTest;

//public class TableInfo
//{
//    public class CreateInfo(Volume tableName, Field[] fields)
//    {
//        public Volume TableName { get; } = tableName;

//        public Field[] Fields { get; } = fields;
//    }

//    public static CreateInfo CreateTable(Type type)
//    {
//        var attribute = type.GetCustomAttribute<Table>();
//        var tableName = new Volume(attribute?.Name ?? type.Name);
//        var fields = new List<Field>();
//        foreach (var property in type.GetProperties())
//        {
//            if (property.GetCustomAttribute<TableIgnore>() is not null)
//                continue;
//            var fieldName = property.GetCustomAttribute<TableField>()?.Name ?? property.Name;
//            fields.Add(new(fieldName));
//        }
//        return new(tableName, fields.ToArray());
//    }

//    public class InsertInfo(Volume tableName, Volume[] fieldValues)
//    {
//        public Volume TableName { get; } = tableName;

//        public Volume[] FieldValues { get; } = fieldValues;
//    }

//    public static InsertInfo InsertItem(object obj)
//    {
//        var type = obj.GetType();
//        var attribute = type.GetCustomAttribute<Table>();
//        var tableName = new Volume(attribute?.Name ?? type.Name);
//        var fieldValues = new List<Volume>();
//        foreach (var property in type.GetProperties())
//        {
//            if (property.GetCustomAttribute<TableIgnore>() is not null)
//                continue;
//            var value = property.GetValue(obj) switch
//            {
//                ISsSerializable iss => iss.ToSsString(),
//                object o => o.ToString(),
//                _ => null
//            };

//        }
//    }
//}