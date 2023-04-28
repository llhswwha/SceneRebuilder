//using Location.WCFServiceReferences.LocationServices;
//using Newtonsoft.Json;

using System;
using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Collections.Generic;
using Location.WCFServiceReferences.LocationServices;
using LitJson;
using System.Security.Cryptography.X509Certificates;
using BestHTTP;

/// <summary>
/// https请求时要忽略证书验证，在闵行数据接口中存在证书验证
/// </summary>
public class CertHandler : CertificateHandler
{
    /// <summary>
    /// https请求时要忽略证书验证
    /// </summary>
    /// <param name="certificateData"></param>
    /// <returns></returns>
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
    }
}

public enum JsonType
{
    LitJson,
    NewtsonJson,
}

public class WebApiClient : MonoBehaviour//,ICommunicationClient
{
    public bool Offline=false;

    // Use this for initialization
    void Start()
    {
        //GetTags((txt) =>
        //{
        //    Debug.Log(txt);
        //});
        //GetTag(128, (txt) =>
        // {
        //     Debug.Log(txt);
        // });
        ////LoginOut();
    }

    public bool isGetTagsBusy = false;

    public string host = "localhost";
    public string port = "8733";

    public string javahost = "localhost";//JAVA服务器IP
    public string javaport = "8094";////JAVA服务器端口号

    private static string NullString = "null";//服务端为空时，这里收到的是null,而不是空字符

    public string apiUrl = "api";

    public string GetBaseUrl()
    {
        return $"http://{host}:{port}/{apiUrl}/";
    }

    /// <summary>
    /// 闵行java服务器数据获取路径
    /// </summary>
    /// <returns></returns>
    public string GetBaseUrl_JavaServer()
    {
        return string.Format("https://{0}:{1}/", javahost, javaport);
    }

    public IEnumerator GetArray<T>(string url, Action<T[]> callback,Action<string> errorCallback=null)
    {
        //Debug.Log("GetArray:" + url);
        yield return GetString(url, json =>
        {
            try
            {
                T[] array = null;
                string urlT = url;
                string jsonT = json;
                if (json == null || json == ""||json== NullString)
                {
                    array = null;
                }
                else
                {
                    array = JsonMapper.ToObject<T[]>(json);
                    //array = JsonConvert.DeserializeObject<T[]>(json);
                }
                if (callback != null)
                {
                    callback(array);
                }

                //ThreadManager.Run(() => //将json解析过程放到子线程中处理
                //{
                //    T[] array = JsonConvert.DeserializeObject<T[]>(json);
                //    return array;
                //}, (array) =>
                //{
                //    if (callback != null)
                //    {
                //        callback(array);
                //    }
                //}, "DeserializeObject");
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
        }, errorCallback);
    }

    public void GetArrayByThread<T>(string url, Action<T[]> callback, Action<string> errorCallback = null)
    {
        ThreadManager.Run(() => 
        {
            GetStringByThread(url, json =>
            {
                T[] array = null;
                ThreadManager.Run(() =>
                {
                    string urlT = url;
                    string jsonT = json;
                    if (json == null || json == "" || json == NullString)
                    {
                        array = null;
                    }
                    else
                    {
                        array = JsonMapper.ToObject<T[]>(json);
                        //array = JsonConvert.DeserializeObject<T[]>(json);
                    }

                }, () =>
                {
                    if (callback != null) callback(array);
                }, "");
            }, errorCallback);
        }, () => { }, "");        
    }

    public void GetStringByThread(string url, Action<string> callback = null, Action<string> errorCallback = null)
    {
        HTTPRequest request = new HTTPRequest(new System.Uri(url), HTTPMethods.Get, (req, resp) => {
            try
            {
                if (resp != null)
                {
                    if (resp.IsSuccess)
                    {
                        string info = resp.DataAsText;
                        if (callback != null) callback(info);
                    }
                    else
                    {
                        string status = string.Format("Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}",
                                                        resp.StatusCode,
                                                        resp.Message,
                                                        resp.DataAsText);
                        Debug.LogError(status);
                        if (errorCallback != null) errorCallback(status); ;
                    }
                }
                else
                {
                    if (errorCallback != null) errorCallback("resp == null"); ;
                    Debug.LogError("GetAreaDevInfo.Resp is empty...");
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                if (errorCallback != null) errorCallback(e.ToString()); ;
            }
        });
        //request.UseStreaming = true;
        //request.StreamFragmentSize = 1 * 1024 * 1024;// 1 megabyte 
        //request.DisableCache = true;// already saving to a file, so turn off caching 
        //request.OnProgress = OnLoadProgress;
        //request.EnableTimoutForStreaming = true;
        request.Send();
    }

    //IEnumerator GetList<T>(string url, Action<List<T>> callback, Action<string> errorCallback)
    //{
    //    //Debug.Log("GetArray:" + url);
    //    yield return GetString(url, json =>
    //    {
    //        try
    //        {
    //            //var jo = JArray.Parse(json);
    //            //T[] array = jo.ToObject<T[]>();//此处Data类和Java中的结构完全一样
    //            List<T> list = JsonConvert.DeserializeObject<List<T>>(json);
    //            if (callback != null)
    //            {
    //                callback(list);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Debug.LogError(ex.ToString());
    //        }
    //    }, errorCallback);
    //}

    public  IEnumerator GetObject<T>(string url, Action<T> callback, Action<string> errorCallback=null,JsonType jsonType = JsonType.LitJson)
    {
        //Debug.Log("GetObject:" + url);
        yield return GetString(url, json =>
        {
            try
            {
                ParseAndCallback(json, callback, errorCallback, jsonType);
            }
            catch (Exception ex)
            {
                Debug.LogError("WebApiClient.GetObject:"+url+"\n"+ex.ToString());
            }
        }, (error)=>
        {
            Debug.LogError($"WebApiClient.GetObject error:{error} errorCallback:{errorCallback} callback:{callback}");
            if (errorCallback != null)
            {
                errorCallback(error);
            }
            else
            {
                if (callback != null)
                {
                    callback(default(T));
                }
            }
        });
    }

   public  IEnumerator DeleteString(string url, Action<string> callback, Action<string> errorCallback)
    {
        using (UnityWebRequest www = new UnityWebRequest())
        {
            www.url = url;
            www.method = UnityWebRequest.kHttpVerbDELETE;
            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error + "|" + url);
                if (errorCallback != null)
                {
                    errorCallback(www.error);
                }
                else
                {
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
            }
            else
            {
                string text = www.downloadHandler.text;
                //Debug.Log(text);
                if (callback != null)
                {
                    callback(text);
                }
            }
        }
    }


   public  IEnumerator DeleteObject<T>(string url, Action<T> callback, Action<string> errorCallback = null)
    {
        yield return DeleteString(url, json =>
        {
            try
            {
                ParseAndCallback(json, callback, errorCallback);
            }
            catch (Exception ex)
            {
                Debug.LogError("WebApiClient.DeleteObject:" + url + "\n" + ex.ToString());
            }
        }, errorCallback);
    }
    /// <summary>
    /// 上面的DeleteObject,再url后加list<int>时出错。所以增加了一个object,用于传递参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="url"></param>
    /// <param name="entity"></param>
    /// <param name="callback"></param>
    /// <param name="errorCallback"></param>
    /// <returns></returns>
    public IEnumerator DeleteObject<T>(string url,object entity,Action<T>callback,Action<string>errorCallback=null)
    {
        using (UnityWebRequest www = new UnityWebRequest())
        {
            www.url = url;
            //Debug.Log(url);
            www.method = UnityWebRequest.kHttpVerbDELETE;
            //string  json = JsonConvert.SerializeObject(entity);
            string json="";
            if(entity!=null)
                json = JsonMapper.ToJson(entity);
            //Debug.Log("发送json:" + json.ToString());
            if (json != null && json != "")
            {
                //Debug.Log("转化为byte类型");
                byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
                //Debug.Log("bodyRaw:byte");
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                //Debug.Log(www.uploadHandler.ToString());
                www.SetRequestHeader("Content-Type", "application/json");
            }
            else
            {
                Debug.Log("未生成");
            }

            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log("错误:www.error =>" + www.error);
                if (errorCallback != null)
                {
                    errorCallback(www.error);
                }
            }
            else
            {
                // Show results as text
                string text = www.downloadHandler.text;
                //Debug.Log("返回Json:" + text);
                ParseAndCallback(text, callback, errorCallback);
            }
        }
    }

    public static List<string> urls = new List<string>();

    public IEnumerator GetString(string url, Action<string> callback,Action<string> errorCallback)
    {
#if UNITY_EDITOR
        //if (!urls.Contains(url))
        //{
        //    //Debug.Log($"GetString !urls.Contains(url) First url:" + url);
        //    urls.Add(url);
        //}
#endif
        if (Offline){
            yield return null;
            string txt=LocalDb.Load(url);
            if (callback != null)
            {
                callback(txt);
            }
        }
        else{
#if UNITY_EDITOR
            //Debug.Log("GetString:" + url);
#endif
            //Log.Info("GetString", "url:" + url);
            //Debug.LogError("");
            using (UnityWebRequest www = new UnityWebRequest())
            {
                www.url = url;
                www.method = UnityWebRequest.kHttpVerbGET;
                www.downloadHandler = new DownloadHandlerBuffer();
                
#if UNITY_EDITOR
            Debug.Log($"GetString url:{url} timeout:{www.timeout}");
#endif
                //https请求忽略证书验证YZL20210815
                if (url.ToLower().StartsWith("https:"))
                {
                    www.certificateHandler = new CertHandler();
                }
                //DateTime r = DateTime.Now;
                yield return www.SendWebRequest();//网络通讯，下面的代码是异步方式的。
                if (www.isNetworkError || www.isHttpError)
                {
                    LocalDb.Save(url,www.error,DbDataItemType.Error);
                    Debug.LogError($"GetString Error:{www.error} url:{url} errorCallback:{errorCallback} callback:{callback}");
                    if (errorCallback != null)
                    {
                        errorCallback(www.error);
                    }
                    else
                    {
                        if (callback != null)
                        {
                            callback(null);
                        }
                    }
                }
                else
                {
                    string text = www.downloadHandler.text;
                    //Debug.LogFormat("获取数据耗时：{0}ms",(DateTime.Now-r).TotalMilliseconds);
                    //LocalDb.Save(url,text,DbDataItemType.Data);
                    if (callback != null)
                    {
                        callback(text);
                    }
                }
            }
        }
        
    }

    public void GetDataInfo(string url, Action<string> callback = null)
    {

        HTTPRequest request = new HTTPRequest(new System.Uri(url), HTTPMethods.Get, (req, resp) => {
            try
            {
                //Debug.Log(req.State);
                if (req.State == HTTPRequestStates.Error)
                {
                    Debug.Log(req.Exception);
                }
                //当流传输未完全结束，也会执行回调.(满足两个条件，才算完成)
                if (resp.IsSuccess)
                {
                    string info = resp.DataAsText;
                    if (callback != null) callback(info);
                }
                else
                {
                    string status = string.Format("Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}",
                                                    resp.StatusCode,
                                                    resp.Message,
                                                    resp.DataAsText);
                    Debug.LogError(status);
                    if (callback != null) callback("");
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                if (callback != null) callback("");
            }
        });
        //request.UseStreaming = true;
        //request.StreamFragmentSize = 1 * 1024 * 1024;// 1 megabyte 
        //request.DisableCache = true;// already saving to a file, so turn off caching 
        //request.OnProgress = OnLoadProgress;
        //request.EnableTimoutForStreaming = true;
        request.Send();
    }

    public  IEnumerator PostString(string url, Action<string> callback, Action<string> errorCallback)
    {
        //Debug.Log("GetString:" + url);
        using (UnityWebRequest www = new UnityWebRequest())
        {
            www.url = url;
            www.method = UnityWebRequest.kHttpVerbPOST;
            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error + "|" + url);
                if (errorCallback != null)
                {
                    errorCallback(www.error);
                }
                else
                {
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
            }
            else
            {
                string text = www.downloadHandler.text;
                //Debug.Log(text);
                if (callback != null)
                {
                    callback(text);
                }
            }
        }
    }

   public IEnumerator PostObject<T>(string url,object entity,Action<T> callback,Action<string> errorCallback)
    {
        using (UnityWebRequest www = new UnityWebRequest())
        {
            www.url = url;
            //Debug.Log(url);
            www.method = UnityWebRequest.kHttpVerbPOST;
            //string  json = JsonConvert.SerializeObject(entity);
            string json = JsonMapper.ToJson(entity);
            //Debug.Log("111发送json:" + json.ToString());
            if (json != null && json != "")
            {
                //Debug.Log("转化为byte类型");
                byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
                //Debug.Log("bodyRaw:byte");
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                //Debug.Log(www.uploadHandler.ToString());
                www.SetRequestHeader("Content-Type", "application/json");
            }
            else
            {
                Debug.Log("未生成:"+ url);
            }
            //https请求忽略证书验证YZL20210815
            if (url.ToLower().StartsWith("https:"))
            {
                www.certificateHandler = new CertHandler();
            }
            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                //Debug.Log("111错误:www.error =>"+www.error);
                if (errorCallback != null)
                {
                    errorCallback(www.error);
                }
            }
            else
            {
                // Show results as text
                string text = www.downloadHandler.text;
                //Debug.Log("返回Json:"+text);
                ParseAndCallback(text, callback, errorCallback);
                // Or retrieve results as binary data
                //byte[] results = www.downloadHandler.data;
            }
        }
    }

    public IEnumerator PostArray<T>(string url,object entity,Action<T[]> callback,Action<string> errorCallback)
    {
       // PostObject<T[]>(url, entity, callback, errorCallback);

        using (UnityWebRequest www = new UnityWebRequest())
        {
            www.url = url;
            //Debug.Log(url);
            www.method = UnityWebRequest.kHttpVerbPOST;
            //string json = JsonConvert.SerializeObject(entity);
            string json = JsonMapper.ToJson(entity);
            //Debug.Log("获取json:" + json.ToString());
            if (json != null && json != "")
            {
                //Debug.Log("转化为byte类型");
                byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
                //Debug.Log("bodyRaw:byte");
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                //Debug.Log(www.uploadHandler.ToString());
                www.SetRequestHeader("Content-Type", "application/json");
            }
            else
            {
                Debug.Log("未生成");
            }

            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log("错误:www.error =>" + www.error);
                if (errorCallback != null)
                {
                    errorCallback(www.error);
                }
            }
            else
            {
                // Show results as text
                string text = www.downloadHandler.text;
                //Debug.Log("返回Json:" + text);

                try
                {
                    T[] array = null;
                    if (text == null || text == ""|| text == NullString)
                    {
                        array = null;
                    }
                    else
                    {
                        //array = JsonConvert.DeserializeObject<T[]>(text);
                        array = JsonMapper.ToObject<T[]>(text);
                    }
                    if (callback != null)
                    {
                        callback(array);
                    }

                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.ToString());
                }

            }
        }
    }





    private static void ParseAndCallback<T>(string text, Action<T> callback, Action<string> errorCallback, JsonType jsonType=JsonType.LitJson)
    {
        var type = typeof(T);
        //Debug.Log($"WebApiClient.ParseAndCallback type:{type} callback:{callback} errorCallback:{errorCallback} text:{text} ");
        if (text != null && text != "")
        {
            if (type == typeof(string)) //string
            {
                T result = (T)Convert.ChangeType(text, typeof(T));
                if (callback != null)
                {
                    callback(result);
                }
            }
            else if (type.IsPrimitive)//基本类型 int bool
            {
                T result = (T)Convert.ChangeType(text, typeof(T));
                if (callback != null)
                {
                    callback(result);
                }
            }
            else //类类型
            {
                try
                {
                    DateTime r = DateTime.Now;
                    T t = default(T);
                    //t = JsonConvert.DeserializeObject<T>(text);
                    if(jsonType==JsonType.LitJson)
                    {
                        try
                        {
                            if (text != "" && text != NullString) t = JsonMapper.ToObject<T>(text);
                            //Debug.LogFormat("LitJson.解析Json耗时：{0}ms", (DateTime.Now - r).TotalMilliseconds);
                        }
                        catch (System.Exception ex2)
                        {
                            Debug.LogWarning($"WebApiClient.PostObject: \n{ex2.ToString()}\nText: \n:{text}");
                            t = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(text);
                        }
                    }
                    else if(jsonType==JsonType.NewtsonJson)
                    {
                        t = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(text);
                        //Debug.LogFormat("Newtsonsoft.解析Json耗时：{0}ms", (DateTime.Now - r).TotalMilliseconds);
                    }
                    callback(t);//数据为空，也要触发回调，不触发会导致回调后功能实现不了
                }
                catch (Exception ex)
                {
                    Debug.LogError("WebApiClient.PostObject: \n" + ex.ToString());
                    Debug.LogError("Text: \n" + text);
                    if (errorCallback != null)
                    {
                        errorCallback(ex.ToString());
                    }
                }
            }
        }
        else
        {
            Debug.LogError($"WebApiClient.ParseAndCallback null or empty :{text}");
        }
    }


 public   IEnumerator PutObject<T>(string url, object entity, Action<T> callback, Action<string> errorCallback)
    {
        using (UnityWebRequest www = new UnityWebRequest())
        {
            www.url = url;
            //Debug.Log(url);
            www.method = UnityWebRequest.kHttpVerbPUT;
            //string json = JsonConvert.SerializeObject(entity);
            string json = JsonMapper.ToJson(entity);
            //Debug.Log("获取json:" + json.ToString());
            if (json != null && json != "")
            {
                //Debug.Log("转化为byte类型");
                byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
                //Debug.Log("bodyRaw:byte");
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                //Debug.Log(www.uploadHandler.ToString());
                www.SetRequestHeader("Content-Type", "application/json");
            }
            else
            {
                Debug.Log("未生成");
            }

            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log("错误:www.error =>" + www.error);
                if (errorCallback != null)
                {
                    errorCallback(www.error);
                }
            }
            else
            {
                // Show results as text
                string results = www.downloadHandler.text;
                //Debug.Log(results);
                ParseAndCallback(results, callback, errorCallback);
            }
        }
    }

    public void GetObjectByThread<T>(string url, Action<T> callback, Action<string> errorCallback = null)
    {
        ThreadManager.Run(() =>
        {
            GetStringByThread(url, json =>
            {
                T obj = default(T);
                ThreadManager.Run(() =>
                {
                    string urlT = url;
                    string jsonT = json;
                    if (json == null || json == "" || json == NullString)
                    {
                        obj = default(T);
                    }
                    else
                    {
                        obj = JsonMapper.ToObject<T>(json);
                        //array = JsonConvert.DeserializeObject<T[]>(json);
                    }

                }, () =>
                {
                    if (callback != null) callback(obj);
                }, "");
            }, errorCallback);
        }, () => { }, "");
    }

    


    //private class AcceptAllCertificatesSignedWithASpecificKeyPublicKey : CertificateHandler
    //{
    //    // Encoded RSAPublicKey
    //    private static string PUB_KEY = "30818902818100C4A06B7B52F8D17DC1CCB47362" +
    //        "C64AB799AAE19E245A7559E9CEEC7D8AA4DF07CB0B21FDFD763C63A313A668FE9D764E" +
    //        "D913C51A676788DB62AF624F422C2F112C1316922AA5D37823CD9F43D1FC54513D14B2" +
    //        "9E36991F08A042C42EAAEEE5FE8E2CB10167174A359CEBF6FACC2C9CA933AD403137EE" +
    //        "2C3F4CBED9460129C72B0203010001";

    //    protected override bool ValidateCertificate(byte[] certificateData)
    //    {
    //        X509Certificate2 certificate = new X509Certificate2(certificateData);
    //        string pk = certificate.GetPublicKeyString();

    //        return true;

    //        // Bad dog

    //    }
    //}


    //public LoginInfo getOgjByJson(string json)
    //{
    //    try
    //    {
    //        LoginInfo obj = Activator.CreateInstance<LoginInfo>();
    //        using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
    //        {

    //            //DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());

    //            return null;
    //          //  return (LoginInfo)serializer.ReadObject(ms);

    //        }

    //        LoginInfo info = new LoginInfo();
    //        return info;
    //    }
    //    catch(Exception ex)
    //    {
    //        Debug.LogError(ex.ToString());
    //        return null;
    //    }
    //}




    //string getTagsUrl;

    //private Tag[] tagsBuffer;

    //public void GetTags(Action<Tag[]> callback)
    //{
    //    getTagsUrl = GetBaseUrl() + "tags/detail";
    //    if (isGetTagsBusy == true && tagsBuffer!=null)//必须有这个，不然和InvokeRepeating结合会导致一直发送没有接收。奇怪，又没有了
    //    {
    //        //Log.Alarm("WebApi.GetTags", "isBusy == true");
    //        if (callback != null)
    //        {
    //            callback(tagsBuffer);//有缓存数据把缓存给他。
    //        }
    //        return;
    //    }
    //    isGetTagsBusy = true;//奇怪，又没有了
    //    StartCoroutine(GetArray<Tag>(getTagsUrl, (tags) =>
    //    {
    //        tagsBuffer = tags;
    //        if (callback != null)
    //        {
    //            callback(tags);
    //        }
    //        isGetTagsBusy = false;
    //    },error=>
    //    {
    //        if (callback != null)
    //        {
    //            callback(null);//这个不能忘记
    //        }
    //        isGetTagsBusy = false;//断线重连的情况
    //    }));
    //}

    //public void GetRealPositions(Action<TagPosition> callback)
    //{
    //    string url = GetBaseUrl() + "pos";
    //    StartCoroutine(GetObject(url, callback));
    //}

    //public void GetTag(int id, Action<Tag> callback)
    //{
    //    string url = GetBaseUrl() + "tags/" + id;
    //    StartCoroutine(GetObject(url, callback));
    //}
    //public void GetPersonTree(Action<AreaNode> callback)
    //{
    //    string url = GetBaseUrl() + "areas/tree?view=2";
    //    StartCoroutine(GetObject(url, callback));
    //}

    //public void GetDepartmentTree(Action<Department> callback)
    //{
    //    string url = GetBaseUrl() + "deps/tree?view=2";
    //    StartCoroutine(GetObject(url, callback));
    //}


    //public void GetTopoTree(Action<PhysicalTopology> callback)
    //{
    //    string url = GetBaseUrl() + "areas/tree/detail?view=0";
    //    StartCoroutine(GetObject(url, callback));
    //}

    //public void GetAreaStatistics(int id, Action<AreaStatistics> callback)
    //{
    //    string url = GetBaseUrl() + "areas/statistics?id=" + id;
    //    StartCoroutine(GetObject(url, callback));
    //}

    //public void GetPointsByPid(int areaId, Action<AreaPoints[]> callback)
    //{
    //    string url = GetBaseUrl() + "areas/getPointsByPid?pid=" + areaId;
    //    StartCoroutine(GetArray(url, callback));
    //}

    //public void HeartBeat(string info, Action<string> callback, Action<string> errorCallback)
    //{
    //    string url = GetBaseUrl() + "users/HeartBeat/" + info;
    //    StartCoroutine(GetString(url, callback,errorCallback));
    //}
    ///// <summary>
    ///// 登录
    ///// </summary>
    ///// <param name="info"></param>
    ///// <param name="callback"></param>
    ///// <param name="errorCallback"></param>
    //public void Login(LoginInfo info, Action<LoginInfo> callback,Action<string> errorCallback)
    //{
    // string url = GetBaseUrl() + "users/LoginPost";
    //    Debug.Log(url);
    //    StartCoroutine(PostObject(url,info,callback,errorCallback));
    //}

    ///// <summary>
    ///// 登出
    ///// </summary>
    ///// <param name="info"></param>
    ///// <param name="callback"></param>
    ///// <param name="errorCallback"></param>
    //public void LoginOut(LoginInfo info, Action<LoginInfo> callback, Action<string> errorCallback)
    //{
    //    string url = GetBaseUrl() + "users/LogoutPost";
    //    Debug.Log(url);
    //    StartCoroutine(PostObject(url,info,callback,errorCallback));
    //}

    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="info"></param>
    ///// <param name="callback"></param>
    ///// <param name="errorCallback"></param>
    //public void KeepLive(LoginInfo info, Action<LoginInfo> callback, Action<string> errorCallback)
    //{
    //    string url = GetBaseUrl() + "users/KeepLivePost";
    //    Debug.Log(url);
    //    StartCoroutine(PostObject(url, info, callback, errorCallback));

    //}



    ///// <summary>
    ///// 获取人员列表
    ///// </summary>
    ///// <param name="name"></param>
    ///// <param name="callback"></param>
    ///// <param name="errorCallback"></param>
    //public void getListPersonls(string name, Action<Personnel[]> callback, Action<string> errorCallback)
    //{
    //    Debug.Log(name);
    //    string url = GetBaseUrl();
    //    //if (name != null && name != "")
    //    //{
    //    //    url += "persons/list";
    //    //}
    //    //else
    //    //{
    //    //    url += "persons/search/?name="+name;
    //    //}
    //    url += "persons/list";

    //    Debug.Log(url);
    //    StartCoroutine(GetArray(url, callback, errorCallback));
    //}

    ///// <summary>
    ///// 添加人员
    ///// </summary>
    ///// <param name="person"></param>
    ///// <param name="callBack"></param>
    ///// <param name="errorBack"></param>
    //public void AddPersonnel(Personnel person, Action<Personnel> callBack, Action<string> errorBack)
    //{
    //    string url = GetBaseUrl()+ "persons";
    //    Debug.Log(url);
    //    StartCoroutine(PostObject(url,person,callBack,errorBack));
    //}
    ///// <summary>
    ///// 修改人员
    ///// </summary>
    ///// <param name="person"></param>
    ///// <param name="callBack"></param>
    ///// <param name="errorBack"></param>
    //public void UpdatePersonnel(Personnel person, Action<Personnel> callBack, Action<string> errorBack)
    //{
    //    string url = GetBaseUrl() + "persons";
    //    Debug.Log(url);
    //    StartCoroutine(PutObject(url, person, callBack, errorBack));
    //}

    ///// <summary>
    ///// 删除人员
    ///// </summary>
    ///// <param name="id"></param>
    ///// <param name="callBack"></param>
    ///// <param name="errorBack"></param>
    //public void DeletePersonnel(string id, Action<Personnel> callBack, Action<string> errorBack)
    //{
    //    string url = GetBaseUrl()+ "persons/"+id;
    //    Debug.Log(url);
    //    StartCoroutine(DeleteObject(url,callBack,errorBack));
    //}

    ///// <summary>
    ///// 根据ID查询人员信息
    ///// </summary>
    ///// <param name="id"></param>
    ///// <param name="callBack"></param>
    ///// <param name="errorBack"></param>
    //public void GetPersonnelById(string id, Action<Personnel> callBack, Action<string> errorBack)
    //{
    //    string url = GetBaseUrl() + "persons/" + id;
    //    Debug.Log(url);
    //    StartCoroutine(GetObject(url, callBack, errorBack));
    //}


    ///// <summary>
    ///// 获取设备列表
    ///// </summary>
    ///// <param name="callBack"></param>
    ///// <param name="errorBack"></param>
    //public void GetDevList(Action<DevInfo[]> callBack, Action<string> errorBack)
    //{
    //    string url = GetBaseUrl() + "devices/list";
    //    Debug.Log(url);
    //    StartCoroutine(GetArray(url,callBack,errorBack));
    //}

    ///// <summary>
    ///// 获取设备详细信息
    ///// </summary>
    ///// <param name="id"></param>
    ///// <param name="callBack"></param>
    ///// <param name="errorBack"></param>
    //public void GetDevinfoById(string id, Action<DevInfo> callBack, Action<string> errorBack)
    //{
    //    string url = GetBaseUrl() + "devices/"+id;
    //    Debug.Log(url);
    //    StartCoroutine(GetObject(url, callBack, errorBack));
    //}

    ///// <summary>
    ///// 添加设备
    ///// </summary>
    ///// <param name="devinfo"></param>
    ///// <param name="callBack"></param>
    ///// <param name="errorBack"></param>
    //public void AddDevinfo(DevInfo devinfo, Action<DevInfo> callBack, Action<string> errorBack)
    //{
    //    string url = GetBaseUrl() + "devices" ;
    //    Debug.Log(url);
    //    StartCoroutine(PostObject(url, devinfo,callBack, errorBack));
    //}


    ///// <summary>
    ///// 修改设备
    ///// </summary>
    ///// <param name="devinfo"></param>
    ///// <param name="callBack"></param>
    ///// <param name="errorBack"></param>
    //public void UpdateDevinfo(DevInfo devinfo, Action<DevInfo> callBack, Action<string> errorBack)
    //{
    //    string url = GetBaseUrl() + "devices";
    //    Debug.Log(url);
    //    StartCoroutine(PutObject(url, devinfo, callBack, errorBack));
    //}
    ///// <summary>
    ///// 删除设备
    ///// </summary>
    ///// <param name="id"></param>
    ///// <param name="callBack"></param>
    ///// <param name="errorBack"></param>
    //public void DeleteDevinfo(string id, Action<DevInfo> callBack, Action<string> errorBack)
    //{
    //    string url = GetBaseUrl() + "devices?id="+id;
    //    Debug.Log(url);
    //    StartCoroutine(DeleteObject(url, callBack, errorBack));

    //}





}