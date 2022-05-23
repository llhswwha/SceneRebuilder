using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProgressArgEx : List<ProgressArg>, IProgressArg
{
    public ProgressArgEx()
    {

    }

    public ProgressArgEx(ProgressArg arg)
    {
        this.Add(arg);
    }

    public float GetProgress()
    {
        return this[0].GetProgress();
    }

    public string GetTitle()
    {
        return this[0].GetTitle();
    }

    public override string ToString()
    {
        if (this.Count > 0)
        {
            return this[0].ToString();
        }
        else
        {
            return base.ToString();
        }
    }

    internal void AddSubProgress(ProgressArg subProgress)
    {
        foreach (var item in this)
        {
            if (item == subProgress)
            {
                Debug.LogError("ExistProgress:" + subProgress);
                return;
            }
        }
        this.Last().AddSubProgress(subProgress);
        this.Add(subProgress);
    }

    public IProgressArg Clone()
    {
        ProgressArgEx list = new ProgressArgEx();
        list.AddRange(this);
        return list;
    }
}

//public interface IProgressArg
//{
//    string GetTitle();

//    float GetProgress();

//    IProgressArg Clone();
//}

public class ProgressArg : IProgressArg
{
    public IProgressArg Clone()
    {
        ProgressArg arg = new ProgressArg();
        arg.title = this.title;
        //arg.progress = this.progress;
        arg.i = this.i;
        arg.count = this.count;
        arg.tag = this.tag;
        if (this.subP != null)
        {
            //arg.subP = this.subP.Clone();
            arg.subP = this.subP;
        }
        return arg;
    }

    public string title = "";
    public float progress;

    public float GetProgress()
    {
        //float progress = i / count;
        if (subP == null)
        {
            return progress;
        }
        else
        {
            return (i + subP.GetProgress()) / count;
        }
        //return progress;
    }
    public int i;
    public int count;
    public object tag;

    //public ProgressArg(float p,object t=null)
    //{
    //    this.progress = p;
    //    this.tag = t;
    //}

    //public ProgressArg(string title, float p, object t = null)
    //{
    //    this.title = title;
    //    this.progress = p;
    //    this.tag = t;
    //}

    //public ProgressArg(int i,int count, object t = null)
    //{
    //    this.i = i;
    //    this.count = count;
    //    this.progress = (float)i / count;
    //    this.tag = t;
    //}

    public static ProgressArgEx New(string title, int i, int count, object t = null, IProgressArg pp = null)
    {
        ProgressArg subProgress = new ProgressArg(title, i, count, t);

        if (pp == null)
        {
            ProgressArgEx list = new ProgressArgEx(subProgress);
            return list;
        }
        else
        {
            IProgressArg cloneP = pp.Clone();
            if (cloneP is ProgressArgEx)
            {
                ProgressArgEx list = cloneP as ProgressArgEx;
                //list.Last().AddSubProgress(subProgress);
                //list.Add(subProgress);
                list.AddSubProgress(subProgress);
                return list;
            }
            else
            {
                //ProgressArg p0 = (pp as ProgressArg).Clone();
                ProgressArg p0 = (cloneP as ProgressArg);
                ProgressArgEx list = new ProgressArgEx();
                p0.AddSubProgress(subProgress);
                //return p0;
                list.Add(p0);
                list.Add(subProgress);
                return list;
            }
        }

    }

    public ProgressArg()
    {

    }

    public ProgressArg(string title, int i, int count, object t = null)
    {
        this.title = title;
        this.i = i;
        this.count = count;
        this.progress = (float)i / count;
        this.tag = t;

        if (count == 0)
        {
            Debug.LogError($"ProgressArg count==0 title:{title} i:{i} t:{t}");
        }

        if (float.IsNaN(progress))
        {
            Debug.LogError($"ProgressArg IsNaN title:{title} i:{i} t:{t}");
        }
    }

    public bool IsFinished()
    {
        return this.progress >= 1;
    }

    private string GetProgressText()
    {
        return $"{i}/{count} {GetProgress():P2}";
    }

    public override string ToString()
    {
        int count = 1;
        ProgressArg sub = this.subP;
        //string totalProgress = $"[{this.GetProgressText()}]";
        string totalProgress = "";
        string totalTag = this.tag + "";
        while (sub != null)
        {
            count++;
            totalProgress += $">[{sub.GetProgressText()}]";
            totalTag += ">" + sub.tag;
            sub = sub.subP;
            if (count > 5)
            {
                break;
            }
        }
        return $"[T{count}]{totalProgress}({totalTag})";

    }

    public string GetTitle()
    {
        int count = 1;
        ProgressArg sub = this.subP;
        string totalTitle = $"[{title}]";
        while (sub != null)
        {
            count++;
            totalTitle += $">[{sub.title}]";
            sub = sub.subP;
            if (count > 5)
            {
                break;
            }
        }
        return $"[T{count}][{GetProgressText()}]{totalTitle}";

        //if (subP == null)
        //{
        //    return $"[T1][{title}]";
        //}
        //else
        //{
        //    if (subP.subP == null)
        //    {
        //        return $"[T2][{title}]>[{subP.title}]";
        //    }
        //    else
        //    {

        //        if (subP.subP.subP == null)
        //        {
        //            return $"[T3][{title}]>[{subP.title}]>[{subP.subP.title}]";
        //        }
        //        else
        //        {

        //            return $"[T4][{title}]>[{subP.title}]>[{subP.subP.title}]>[{subP.subP.subP.title}]";
        //        }
        //    }
        //}
    }

    public ProgressArg subP;

    public ProgressArg parent;

    public void AddSubProgress(ProgressArg p)
    {
        if (p == this)
        {
            Debug.LogError("AddSubProgress subProgress==this:" + this);
            return;
        }
        p.parent = this;
        subP = p;

        //this.progress = (i + p.progress) / count;
    }
}
