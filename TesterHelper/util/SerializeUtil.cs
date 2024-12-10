using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace TesterHelper.util
{
    public class SerializeUtil
    {
        public static T deserialization<T>(string absoluteFilePath)
        {
            
            if (string.IsNullOrEmpty(absoluteFilePath))
            {
                return default;
            }

            T obj;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                FileStream fs = File.Open(absoluteFilePath, FileMode.Open);
                using (var reader = new StreamReader(fs, Encoding.UTF8))
                {
                    obj = (T)serializer.Deserialize(reader);
                }
            }
            catch (Exception e)
            {
                return default;
            }
            return obj;
        }
        /// <summary>
        /// 序列化对象到文件中
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="absoluteFilePath">目标文件位置</param>
        public static bool serialization(Object obj, string absoluteFilePath)
        {
            
            if (obj == null || string.IsNullOrEmpty(absoluteFilePath))
            {
                return false;
            }
            // 去除命名空间
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            try
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                using (var s = new FileStream(absoluteFilePath, FileMode.Create))
                {
                    using (var writer = new StreamWriter(s, Encoding.UTF8))
                    {
                        serializer.Serialize(writer, obj, ns);// 序列化
                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }
    }
}