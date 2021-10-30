using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Unity.ComnLib;

namespace Base.Common
{
    /// <summary>
    /// 内存对象的串行化（保存）和并行化（读取）
    /// </summary>
    public static class SerializeHelper
    {
        /// <summary>
        /// webgl反序列化要先把所有的类列一遍，这个用来生成代码
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static string GetAssemblyTypesInitText(Assembly assembly)
        {
            Type[] types = assembly.GetTypes();

            List<Type> tmp = new List<Type>();
            foreach (Type t in types)
            {
#if WINDOWS_UWP
                tmp.Add(t);
#else
                if (!t.IsInterface && !t.IsAbstract && !t.IsEnum)
                {
                    string name = t.FullName;
                    if (name.Contains("<")) continue;
                    if (name.Contains("<>")) continue;
                    if (name.Contains("`")) continue;//泛型
                    tmp.Add(t);
                }
#endif
            }
            string txt = string.Format("//Assembly:{0} |{1}/{2}\r\n", assembly.FullName, tmp.Count, types.Length);
            foreach (Type t in tmp)
            {
                string name = t.FullName;
                if (name.Contains("+"))
                {
                    name = name.Replace("+", ".");
                }
                txt += string.Format("new {0}();\r\n", name);
            }
            //print("//GetAssemblyTypesInitText:\r\n" + txt);
            return txt;
        }

        #region 序列化

        /// <summary>
        /// 将对象保存到文件中
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fileName"></param>
        public static void Save(object obj, string fileName)
        {
            //Serialize(obj, fileName);
            Save(obj, fileName, Encoding.UTF8);
        }

        /// <summary>
        /// 将对象保存到文件中
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fileName"></param>
        /// <param name="encoding"></param>
        public static void Save(object obj, string fileName, Encoding encoding)
        {
            CreateFileSDirectory(fileName);
            string text = GetXmlText(obj, encoding);
            File.WriteAllText(fileName, text, encoding);
        }

        /// <summary>
        ///     序列化一个对象到文件
        /// </summary>
        /// <param name="obj">对象实例</param>
        /// <param name="fileName">保存的文件名</param>
        public static void Serialize(object obj, string fileName)
        {
            //CreateFileSDirectory(fileName);
            //var fs = new FileStream(fileName, FileMode.Create);
            //Serialize(obj, fs);
            //fs.Close();
            //return fs;
            Save(obj, fileName);
        }


        /// <summary>
        /// 序列化对象到内存中
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static Stream Serialize(object obj, Encoding encoding)
        {
            MemoryStream memory = new MemoryStream();
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            TextWriter writer = new StreamWriter(memory, encoding);
            serializer.Serialize(writer, obj);
            memory.Position = 0;
            return memory;
        }

        /// <summary>
        /// 将对象转换为xml文本
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="clearNameSpace">是否清空命名空间的字符串</param>
        /// <returns></returns>
        public static string GetXmlText(this object obj, bool clearNameSpace = false)
        {
            return GetXmlText(obj, Encoding.UTF8, clearNameSpace);
        }
        /// <summary>
        /// 将对象转换为xml文本
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="encoding"></param>
        /// <param name="clearNameSpace">是否清空命名空间的字符串</param>
        /// <returns></returns>
        public static string GetXmlText(this object obj, Encoding encoding, bool clearNameSpace = false)
        {
            if (encoding == null)
            {
                encoding = EncodingHelper.GetDefault();
            }
            Stream stream = Serialize(obj, encoding);
            StreamReader reader = new StreamReader(stream, encoding);
            string text = reader.ReadToEnd();

#if WINDOWS_UWP
            reader.Dispose();
#else
            reader.Close();
#endif

            if (clearNameSpace)
            {
                text = text.Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");
            }

            return text;
        }

        /// <summary>
        /// 将对象转换为byte[]
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] GetXmlBytes(object obj)
        {
            MemoryStream stream = new MemoryStream();
            XmlSerialize(obj, stream);
            stream.Position = 0;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        /// <summary>
        /// 将对象序列化成byte数组
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] GetBytes(object obj)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, obj);
            stream.Position = 0;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        /// <summary>
        /// 序列化核心，可以序列化到MemoryStream等其他类型的Stream
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="stream"></param>
        public static void XmlSerialize(object obj, Stream stream)
        {
            var xs = new XmlSerializer(obj.GetType());
            xs.Serialize(stream, obj);
        }

        /// <summary>
        ///     创建一个文件的文件夹，确保不会出现因为不存在文件夹导致文件无法创建
        /// </summary>
        /// <param name="fileName">文件路径</param>
        public static bool CreateFileSDirectory(string fileName)
        {
            try
            {
                var fileInfo = new FileInfo(fileName);
                if (fileInfo.Directory != null && !fileInfo.Directory.Exists)
                    fileInfo.Directory.Create();
                return true;
            }
            catch (Exception ex) //文件路径不合法的话
            {
                ExceptionText = ex.ToString();
                return false;
            }
        }

        #endregion

        #region 反序列化
        /// <summary>
        /// 从文件获取xml,并反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T LoadFromFile<T>(string fileName)
        {
            return DeserializeFromFile<T>(fileName);
        }

        /// <summary>
        ///     并行化一个文件为一个内存对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="fileName">读取的文件名</param>
        /// <returns></returns>
        public static T DeserializeFromFile<T>(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return CreateDefault<T>();
            }
            var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            return DeserializeXml<T>(fs);
        }

        private static Dictionary<Type, XmlSerializer> Serializers = new Dictionary<Type, XmlSerializer>();

        private static XmlSerializer GetXmlSerializer(Type type)
        {
            //lock (Serializers)
            //{
            //    if (Serializers.ContainsKey(type))
            //    {
            //        return Serializers[type];
            //    }
            //    var xs = new XmlSerializer(type);
            //    Serializers.Add(type, xs);
            //    return xs;
            //}

            return new XmlSerializer(type);
        }

        /// <summary>
        /// 反序列化一个Xml的Stream
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fs"></param>
        /// <returns></returns>
        public static T DeserializeXml<T>(Stream fs)
        {
            try
            {
                fs.Position = 0; //这里不能省略，可能导致无数据

                var xs = GetXmlSerializer(typeof(T));
                var newSetting = (T)xs.Deserialize(fs);
                CloseStream(fs);
                return newSetting;
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex);
                ExceptionText = ex.ToString();
                if (EnableLog)
                    Log.Error("SerializeHelper.Deserialize", "Xml:" + LastXml + "\n" + ex);
                CloseStream(fs);
                return CreateDefault<T>();
                //return default(T);
            }
        }

        /// <summary>
        /// 关闭一个Stream
        /// </summary>
        /// <param name="fs"></param>
        public static void CloseStream(Stream fs)
        {
#if WINDOWS_UWP
                        fs.Flush();
                        fs.Dispose();
#else
            fs.Close();
#endif
        }

        /// <summary>
        /// 异常信息
        /// </summary>
        public static string ExceptionText = "";

        /// <summary>
        /// 反序列化一个Xml的byte[]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static T DeserializeXml<T>(byte[] buffer)
        {
            MemoryStream stream = new MemoryStream();
            stream.Write(buffer, 0, buffer.Length);
            return DeserializeXml<T>(stream);
        }

        /// <summary>
        /// 反序列化一个Xml的byte[]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static T LoadFromXmlBytes<T>(byte[] buffer)
        {
            return DeserializeXml<T>(buffer);
        }

        /// <summary>
        /// 反序列化一个对象的byte[]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static T LoadFromBytes<T>(byte[] buffer)
        {
            MemoryStream stream = new MemoryStream();
            stream.Write(buffer, 0, buffer.Length);
            BinaryFormatter formatter = new BinaryFormatter();
            T data = (T)formatter.Deserialize(stream);
            return data;
        }

        /// <summary>
        /// 从字符串创建对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        /// <param name="checkValid">检查合法性，空格和gb2312转utf-8</param>
        /// <param name="showLog">打印日志信息</param>
        /// <returns></returns>
        public static T LoadFromText<T>(this string text, bool checkValid = true, bool showLog = true)
        {
            return LoadFromText<T>(text, Encoding.UTF8, checkValid, showLog);
        }

        /// <summary>
        /// 是否打印日志
        /// </summary>
        public static bool EnableLog = true;

        /// <summary>
        /// 从字符串创建对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        /// <param name="encoding"></param>
        /// <param name="checkValid">检查合法性，空格和gb2312转utf-8</param>
        /// <param name="showLog">打印日志信息</param>
        /// <returns></returns>
        public static T LoadFromText<T>(string text, Encoding encoding, bool checkValid = true, bool showLog = true)
        {
            if (showLog && EnableLog)
            {
                Log.Info("SerializeHelper.LoadFromText Start", string.Format("Length:{0},Type:{1}", text.Length, typeof(T)));
            }

            T result = default(T);
            if (string.IsNullOrEmpty(text)) return result;
            LastXml = text;
            TimeSpan time = TimeCounter.Run(() =>
            {
                if (checkValid)
                {
                    text = text.Trim(); //xml开头不能有空格
                    text = text.Replace("gb2312", "utf-8");
                    text = text.Replace("GB2312", "utf-8");
                }
                if (encoding == null)
                {
                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(text);
                        string encodingText = ((XmlDeclaration)doc.FirstChild).Encoding;
                        if (string.IsNullOrEmpty(encodingText))
                        {
                            encoding = Encoding.UTF8;
                        }
                        else
                        {
                            encoding = Encoding.GetEncoding(encodingText);
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionText = ex.ToString();
                        if (EnableLog)
                            Log.Error("SerializeHelper.LoadFromText", ex);
                        result = default(T);
                    }
                }
                byte[] buffer = encoding.GetBytes(text);
                result = DeserializeXml<T>(buffer);
            });
            if (showLog && EnableLog)
            {
                Log.Info("SerializeHelper.LoadFromText End", string.Format("Length:{0},Time:{1}ms,Type:{2}", text.Length, time.TotalMilliseconds, typeof(T)));
            }
            return result;
        }


        /// <summary>
        /// 最后一次序列化时的Xml
        /// </summary>
        public static string LastXml = "";

        /// <summary>
        /// 创建默认对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateDefault<T>()
        {
            try
            {
                return (T)Activator.CreateInstance(typeof(T));
            }
            catch
            {
                return default(T);
            }
        }

        #endregion

        /// <summary>
        /// 将对象转换为Base64文本
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string GetBase64Text(object obj, Encoding encoding)
        {
            string xml = GetXmlText(obj, encoding);
            byte[] bytes = Encoding.UTF8.GetBytes(xml);
            string base64 = Convert.ToBase64String(bytes);
            return base64;
        }

        /// <summary>
        /// 从Base64格式的字符串加载对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="txt"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static T LoadFromBase64Text<T>(string txt, Encoding encoding)
        {
            byte[] bytes = Convert.FromBase64String(txt);
            string xml = encoding.GetString(bytes);
            return LoadFromText<T>(xml, encoding);
        }

        /// <summary>
        /// 将对象转换为Json文本
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetJsonText(object obj)
        {
            //string json=JsonConvert.SerializeObject(obj);
            //return json;
            return null;
        }

        /// <summary>
        /// 数组转换，实现类转换成接口
        /// </summary>
        /// <typeparam name="IT"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IList<IT> ConverTo<IT, T>(List<T> list) where T : IT
        {
            if (list == null) return null;
#if WINDOWS_UWP
            IList<IT> result = new List<IT>();
            foreach(T item in list)
            {
                result.Add((IT)item);
            }
            return result;
#else
            return list.ConvertAll(new Converter<T, IT>(input => input));
#endif
        }

        /// <summary>
        /// 从文件反序列化列表对象
        /// </summary>
        /// <typeparam name="IT">接口</typeparam>
        /// <typeparam name="T">实现类</typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static IList<IT> GetListFromFile<IT, T>(string fileName) where T : IT
        {
            List<T> list = LoadFromFile<List<T>>(fileName);
            return ConverTo<IT, T>(list);
        }

        /// <summary>
        /// 从Xml反序列化列表对象
        /// </summary>
        /// <typeparam name="IT">接口</typeparam>
        /// <typeparam name="T">实现类</typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static IList<IT> GetListFromXml<IT, T>(string xml) where T : IT
        {
            List<T> list = LoadFromText<List<T>>(xml, null);
            return ConverTo<IT, T>(list);
        }
    }
}
