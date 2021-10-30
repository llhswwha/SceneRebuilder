using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NavisPlugins.Infos
{
    [Serializable]
    public class BIMDevDetail
    {
        [XmlAttribute]
        /// <summary>
        /// 管道/阀门/设备
        /// </summary>
        public string Type { get; set; }
        [XmlAttribute]
        /// <summary>
        /// 序号
        /// </summary>
        public string Id { get; set; }

        [XmlAttribute]
        public string KKS { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string uId { get; set; }

        [XmlAttribute]
        public string SystemPath { get; set; }

        [XmlAttribute]
        /// <summary>
        /// 专业
        /// </summary>
        public string Major { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [XmlAttribute]
        public string Remarks { get; set; }


        /// <summary>
        /// 流程图
        /// </summary>
        [XmlAttribute]
        public string FlowDocs { get; set; }

        /// <summary>
        /// 安装图
        /// </summary>
        [XmlAttribute]
        public string InstallDocs { get; set; }

        [XmlAttribute]
        public string SPID { get; set; }
        [XmlAttribute]
        public string Doc1 { get; set; }

        [XmlAttribute]
        public string Doc2 { get; set; }

        [XmlIgnore]
        public ModelItemInfo Info { get; set; }

        public void SetInfo(ModelItemInfo info)
        {
            Info = info;
            Info.SetVisible(1);
            //Remarks = Info.Name;

            //Info.Description = this.Name;
            //if(!string.IsNullOrEmpty(this.KKS))
            //    Info.KKS = this.KKS;

            info.Detail = this;
        }

        public BIMDevDetail()
        {

        }

        public virtual void Parse(string line)
        {
            string[] parts = line.Split('\t');
        }

        public virtual void Parse(object[] values)
        {
            
        }


        /// <summary>
        /// 标准
        /// </summary>
        [XmlAttribute]
        public string Standard { get; set; }

        public void ParsePipe(object[] values)
        {
            Type = "管线";
            Id = values[0].ToString();
            KKS = values[1].ToString();
            Name = values[2].ToString();
            Standard = values[13].ToString();
            Remarks = values[14].ToString();
            SPID = values[25].ToString();
            Doc1 = values[26].ToString();
            Doc2 = values[27].ToString();
            uId = values[28].ToString();
            uId = CleanUID(uId);
            SystemPath = values[29].ToString();
            FlowDocs = values[30].ToString();
            InstallDocs = values[31].ToString();
            Major = values[32].ToString();

            
        }

        /// <summary>
        /// 型号
        /// </summary>
        [XmlAttribute]
        public string Model { get; set; }


        [XmlAttribute]
        public string PlantItemType { get; set; }

        public void ParseValve(object[] values)
        {
            Type = "阀门";
            Id = values[0].ToString();
            KKS = values[1].ToString();
            Name = values[2].ToString();
            Model = values[3].ToString();
            Remarks = values[19].ToString();
            SPID = values[25].ToString();
            PlantItemType = values[26].ToString();
            Doc1 = values[27].ToString();
            Doc2 = values[28].ToString();
            uId = values[29].ToString();
            uId = CleanUID(uId);
            SystemPath = values[30].ToString();
            FlowDocs = values[31].ToString();
            InstallDocs = values[32].ToString();
            Major = values[33].ToString();
        }

        public override string ToString()
        {
            return $"{Id}\t{KKS}\t{Name}\t{uId}\t{Major}\t[{FlowDocs}]\t:[{InstallDocs}]";
        }

        public static string CleanUID(string id)
        {
            //
            id = id.Replace("SPRF:M: ", "");
            id = id.Replace("@a=", "");
            id = id.Replace("!!", "-");
            id = id.Replace("##", "-");

            if (id.Contains("%"))
            {
                id = id.Split('%')[0];
            }
            return id;
        }
    }



    //[Serializable]
    //public class BIMDevDetail_Valve : BIMDevDetail
    //{
    //    /// <summary>
    //    /// 型号
    //    /// </summary>
    //    [XmlAttribute]
    //    public string Model { get; set; }


    //    [XmlAttribute]
    //    public string PlantItemType { get; set; }

    //    public BIMDevDetail_Valve():base()
    //    {

    //    }


    //    public BIMDevDetail_Valve(string line)
    //    {
    //        this.Parse(line);
    //    }

    //    public override void Parse(string line)
    //    {
    //        string[] parts = line.Split('\t');
    //        //8	30LCP22AA002	手动截止阀	J41W-25P	2.5	60	2.5	200	法兰	手动	不锈钢	OD	219	8	S30408	OD	219	8	S30408		SPRF:M: @a=0027!!80005##297061187689253566		F107201S-JQ-3-0LCA-01-XL-00002^#3机凝结水系统流程图(二).PDF;	"F107201S-JQ-3-0LCP-11-BR-21002^30LCP21BR002管道安装图(汽机专业).pdf;
    //        //F107201S - JQ - 3 - 0LCP - 11 - BR - 22002 ^ 30LCP22BR002管道安装图(汽机专业).pdf; "	汽机专业;	

    //    }

    //    public BIMDevDetail_Valve(object[] values)
    //    {
    //        this.Parse(values);
    //    }

    //    public override void Parse(object[] values)
    //    {
    //        Type = "阀门";
    //        Id = values[0].ToString();
    //        KKS = values[1].ToString();
    //        Name = values[2].ToString();
    //        Model = values[3].ToString();
    //        Remarks = values[19].ToString();
    //        SPID = values[25].ToString();
    //        PlantItemType = values[26].ToString();
    //        Doc1 = values[27].ToString();
    //        Doc2 = values[28].ToString();
    //        UID = values[29].ToString();
    //        SystemPath = values[30].ToString();
    //        FlowDocs = values[31].ToString();
    //        InstallDocs = values[32].ToString();
    //        Major = values[33].ToString();
    //    }
    //}

    //[Serializable]
    //public class BIMDevDetail_Pipe : BIMDevDetail
    //{
    //    /// <summary>
    //    /// 标准
    //    /// </summary>
    //    [XmlAttribute]
    //    public string Standard{ get; set; }



    //    public BIMDevDetail_Pipe() : base()
    //    {

    //    }


    //    public BIMDevDetail_Pipe(string line)
    //    {
    //        this.Parse(line);
    //    }

    //    public override void Parse(string line)
    //    {
    //        string[] parts = line.Split('\t');
    //    }

    //    public BIMDevDetail_Pipe(object[] values)
    //    {
    //        this.Parse(values);
    //    }

    //    public override void Parse(object[] values)
    //    {
    //        Type = "管道";
    //        Id = values[0].ToString();
    //        KKS = values[1].ToString();
    //        Name = values[2].ToString();
    //        Standard = values[13].ToString();
    //        Remarks = values[14].ToString();
    //        SPID = values[25].ToString();
    //        Doc1 = values[26].ToString();
    //        Doc2 = values[27].ToString();
    //        UID = values[28].ToString();
    //        SystemPath = values[29].ToString();
    //        FlowDocs = values[30].ToString();
    //        InstallDocs = values[31].ToString();
    //        Major = values[32].ToString();
    //    }
    //}

    //[Serializable]
    //public class BIMDevDetail_Dev : BIMDevDetail
    //{
    //    public BIMDevDetail_Dev(string line)
    //    {
    //        this.Parse(line);
    //    }

    //    public override void Parse(string line)
    //    {
    //        string[] parts = line.Split('\t');
    //    }

    //    public BIMDevDetail_Dev(object[] values)
    //    {
    //        this.Parse(values);
    //    }

    //    public override void Parse(object[] values)
    //    {
    //        Type = "设备";
    //        Id = values[0].ToString();
    //        KKS = values[1].ToString();
    //        Name = values[2].ToString();
    //        //Standard = values[13].ToString();
    //        //Remarks = values[14].ToString();
    //        SPID = values[25].ToString();
    //        Doc1 = values[26].ToString();
    //        Doc2 = values[27].ToString();
    //        UID = values[28].ToString();
    //        SystemPath = values[29].ToString();
    //        FlowDocs = values[30].ToString();
    //        InstallDocs = values[31].ToString();
    //        Major = values[32].ToString();
    //    }
    //}

    public class BIMDevDetailList : List<BIMDevDetail>
    {
        //public void LoadValvesFromText(string txt)
        //{
        //    string[] lines = txt.Split('\n');
        //    for (int i = 0; i < lines.Length; i++)
        //    {
        //        string line = lines[i].Trim();
        //        BIMDevDetail detail = new BIMDevDetail_Valve(line);
        //        this.Add(detail);
        //        break;
        //    }
        //}

        public void AddPipe(object[] values)
        {
            //throw new NotImplementedException();
            BIMDevDetail detail = new BIMDevDetail();
            detail.ParsePipe(values);
            this.Add(detail);
        }

        //public void LoadPipeFromText(string txt)
        //{
        //    string[] lines = txt.Split('\n');
        //    for (int i = 0; i < lines.Length; i++)
        //    {
        //        string line = lines[i].Trim();
        //        BIMDevDetail detail = new BIMDevDetail_Valve(line);
        //        this.Add(detail);
        //        break;
        //    }
        //}

        public void AddValve(object[] values)
        {
            BIMDevDetail detail = new BIMDevDetail();
            detail.ParseValve(values);
            this.Add(detail);
        }

        public Dictionary<string, ModelItemInfo> idDict = new Dictionary<string, ModelItemInfo>();

        public Dictionary<string, List<ModelItemInfo>> nameDict = new Dictionary<string, List<ModelItemInfo>>();

        public BIMDevDetailList listFound1;
        public BIMDevDetailList listFound2;
        public BIMDevDetailList listNotFound ;

        public void Compare(NavisFileInfo navisFileInfo,Action finished)
        {
            Thread thread = new Thread(() =>
            {
                var models = navisFileInfo.GetAllItems();
                idDict.Clear();
                nameDict.Clear();
                foreach (var model in models)
                {
                    model.Visible = -1;
                    if (!string.IsNullOrEmpty(model.UId))
                    {
                        idDict.Add(model.UId, model);
                    }

                    string mName = model.Name;
                    if(!string.IsNullOrEmpty(mName))
                    {
                        if (!nameDict.ContainsKey(mName))
                        {
                            nameDict.Add(mName, new List<ModelItemInfo>());
                        }
                        nameDict[mName].Add(model);
                    }
                }

                BIMDevDetailList tmp = new BIMDevDetailList();
                tmp.AddRange(this);

                listFound1 = new BIMDevDetailList();
                listFound2 = new BIMDevDetailList();
                listNotFound = new BIMDevDetailList();

                for (int i = 0; i < tmp.Count; i++)
                {
                    BIMDevDetail item = tmp[i];
                    string uid = item.uId;
                    string kks = item.KKS;
                    string name = item.Name;
                    string path = item.SystemPath;
                    //ProgressHelper.DoProgressChanged1(new ProgressArg(i, tmp.Count, name, "Compare"));
                    if (!string.IsNullOrEmpty(uid) && idDict.ContainsKey(uid))
                    {
                        //item.Info = idDict[uid];
                        //item.Info.SetVisible(1);
                        //item.Remarks = item.Info.Name;

                        item.SetInfo(idDict[uid]);
                        listFound1.Add(item);
                        continue;
                    }

                    //if (!string.IsNullOrEmpty(name) && nameDict.ContainsKey(name))
                    //{
                    //    var list= nameDict[name];
                    //    if (list.Count == 1)
                    //    {
                    //        item.Info = list[0];
                    //        listFound.Add(item);

                    //        continue;
                    //    }
                    //    else
                    //    {

                    //    }
                    //}

                    if (!string.IsNullOrEmpty(kks) && nameDict.ContainsKey(kks))
                    {
                        var list = nameDict[kks];
                        if (list.Count == 1)
                        {
                            //item.Info = list[0];
                            //item.Info.SetVisible(2);
                            //item.Remarks = item.Info.Name;

                            item.SetInfo(list[0]);
                            listFound1.Add(item);

                            continue;
                        }
                        else
                        {
                            listFound2.Add(item);
                            continue;
                        }
                    }

                    listNotFound.Add(item);
                }
                //ProgressHelper.Clear();

                if (finished != null)
                {
                    finished();
                }
            });
            thread.Start();
            
        }
    }
}
