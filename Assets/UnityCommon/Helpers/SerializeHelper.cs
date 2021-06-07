using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

public static class SerializeHelper
{
	/// <summary>
	/// 将对象保存到文件中
	/// </summary>
	/// <param name="obj"></param>
	/// <param name="fileName"></param>
	public static void Save(object obj, string fileName)
	{
		Serialize(obj, fileName);
	}

    /// <summary>
    /// 将对象保存到文件中
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="fileName"></param>
    public static void Save(object obj, string fileName,Encoding encoding)
    {
        string text = GetXmlText(obj, encoding);
        File.WriteAllText(fileName, text);
    }
	
	/// <summary>
	///     序列化一个对象到文件
	/// </summary>
	/// <param name="obj">对象实例</param>
	/// <param name="fileName">保存的文件名</param>
	public static FileStream Serialize(object obj, string fileName)
	{
		var fs = new FileStream(fileName, FileMode.Create);
		Serialize(obj, fs);
		fs.Close();
		return fs;
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
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static string GetXmlText(object obj, bool clear = false)
    {
        return GetXmlText(obj, Encoding.UTF8, clear);
    }

	/// <summary>
	/// 将对象转换为xml文本
	/// </summary>
	/// <param name="obj"></param>
	/// <param name="encoding"></param>
	/// <returns></returns>
    public static string GetXmlText(object obj, Encoding encoding, bool clear = false)
	{
        if (encoding == null)
        {
            encoding = Encoding.Default;
        }
		Stream stream = Serialize(obj, encoding);
		StreamReader reader = new StreamReader(stream, encoding);
		string text = reader.ReadToEnd();
		reader.Close();
        if (clear)
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
	public static byte[] GetBytes(object obj)
	{
		MemoryStream stream = new MemoryStream();
		Serialize(obj, stream);
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
	public static void Serialize(object obj, Stream stream)
	{
		var xs = new XmlSerializer(obj.GetType());
		xs.Serialize(stream, obj);
	}

	
	public static T Load<T>(string fileName)
	{
		return Deserialize<T>(fileName);
	}
	
	/// <summary>
	///     并行化一个文件为一个内存对象
	/// </summary>
	/// <typeparam name="T">对象类型</typeparam>
	/// <param name="fileName">读取的文件名</param>
	/// <returns></returns>
	public static T Deserialize<T>(string fileName)
	{
		if (!File.Exists(fileName))
		{
			return CreateDefault<T>();
		}
		var fs = new FileStream(fileName, FileMode.Open);
		return Deserialize<T>(fs);
	}
	
	public static T Deserialize<T>(Stream fs)
	{
		try
		{
			fs.Position = 0; //这里不能省略，可能导致无数据
			
			var xs = new XmlSerializer(typeof(T));
			var newSetting = (T)xs.Deserialize(fs);
			fs.Close();
			return newSetting;
		}
		catch (Exception ex)
		{
			//Log.Error(ex.Message);
			fs.Close();
			return CreateDefault<T>();
			//return default(T);
		}
	}
	
	public static T Deserialize<T>(byte[] buffer)
	{
		MemoryStream stream = new MemoryStream();
		stream.Write(buffer, 0, buffer.Length);
		return Deserialize<T>(stream);
	}
	
	/// <summary>
	/// 从字符串创建对象
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="text"></param>
	/// <param name="encoding"></param>
	/// <returns></returns>
	public static T LoadFromText<T>(string text,Encoding encoding)
	{
        text = text.Replace("gb2312", "utf-8");
        text = text.Replace("GB2312", "utf-8");
        //Log.Info("LoadFromText", "encoding:"+encoding+"|"+"text:"+text.Substring(0,100));
		if (encoding == null)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(text);
			encoding = Encoding.GetEncoding(((XmlDeclaration)doc.FirstChild).Encoding);
		}
		byte[] buffer = encoding.GetBytes(text);
		return Deserialize<T>(buffer);
	}

    public static T LoadFromText<T>(string text)
    {
        return LoadFromText<T>(text, Encoding.UTF8);
    }

    public static T LoadFromBase64Text<T>(string txt, Encoding encoding)
    {
        byte[] bytes = Convert.FromBase64String(txt);
        string xml = encoding.GetString(bytes);
        return LoadFromText<T>(xml, encoding);
    }
	
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
		catch (Exception ex)
		{
			//Log.Error(ex.Message);
			return default(T);
		}
	}
}


