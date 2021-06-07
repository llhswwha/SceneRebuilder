using System.IO;
using System;
public class FileHelper
{
    /// <summary>
    /// 写文件
    /// </summary>
    /// <param name="fileInfo"></param>
    /// <param name="content"></param>
    /// <returns></returns>
	public static bool WriteFile (FileInfo fileInfo,string content)
	{
		StreamWriter sw = fileInfo.CreateText ();
		sw.Write (content);
		sw.Close ();
		sw.Dispose ();
		return true;
	}
	
    /// <summary>
    /// 读文件
    /// </summary>
    /// <param name="fileInfo"></param>
    /// <returns></returns>
	public static string ReadFile (FileInfo fileInfo)
	{
		string content="";
		StreamReader sr;
		try
		{
			sr=File.OpenText(fileInfo.FullName);
			content=sr.ReadToEnd();
		}
		catch(Exception ex)
		{
			LogInfo.Error(ex);
		}
		return content;
	}
}


