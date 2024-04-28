//using YamlDotNet.Serialization;

//namespace test
//{
//    /// <summary>
//    /// 序列化和反序列化yaml文件
//    /// </summary>
//    public static class YamlSaverLoader
//    {

//        public static void SaveToYaml<T>(this T obj, string filePath)
//        {
//            var yamlWriter = File.CreateText(filePath);
//            var yamlSerializer = new Serializer();
//            yamlSerializer.Serialize(yamlWriter, obj);
//            yamlWriter.Close();
//        }

//        public static T LoadFromYaml<T>(string filePath)
//        {
//            if (!File.Exists(filePath))
//            {
//                throw new FileNotFoundException();
//            }
//            var yamlReader = File.OpenText(filePath);
//            var yamlDeserializer = new Deserializer();

//            //读取持久化对象  
//            var info = yamlDeserializer.Deserialize<T>(yamlReader);
//            yamlReader.Close();
//            return info;
//        }
//    }
//}