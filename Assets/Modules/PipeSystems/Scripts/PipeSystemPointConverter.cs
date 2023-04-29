using Base.Common.Extensions;
using DbModel.Location.Pipes;
using Mathd;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeSystemPointConverter  : SingletonBehaviour<PipeSystemPointConverter>
{
    public Vector3 CADOffset;

    public Vector3 UnityOffset;

    public Vector3 GetUnityOffset()
    {
        return UnityOffset+OffsetPos;
    }

    public Vector3 OffsetPos;

    public bool isXZY=true;

    /// <summary>
    /// ʵ�ʳߴ�/��ά�ߴ�
    /// </summary>
    public Vector3 scale;
    /// <summary>
    /// ����ԭ��
    /// </summary>
    public Vector3 axisZero;

    public Vector3 GetOffsetPos()
    {
        //return axisZero;
        return OffsetPos;
    }

    public void SetOffsetPos(Vector3 pos)
    {
        //axisZero=pos;
        OffsetPos=pos;
    }

    /// <summary>
    /// ����У׼
    /// </summary>
    public Vector3 direction = Vector3.one;

    /// <summary>
    /// ����ԭ������
    /// </summary>
    public Transform axisZeroObj;

    public LocationManager locationManager;

    //public LocationSimulatorSetting LocationSimulator;

    /// <summary>
    /// ��׼���б�
    /// </summary>
    public List<PipePointCalculaterItem> list;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    [ContextMenu("GetListPos")]
    private void GetListPos()
    {
        for (int i = 0; i < list.Count; i++)
        {
            PipePointCalculaterItem item = list[i];
            if (item.targetObj == null) continue;
            var p=item.targetObj.transform.position;
            item.Pos=new Vector3(p.x,p.z,p.y);
            if(i>0){
                PipePointCalculaterItem item0 = list[i-1];
                //item.CADOff=new Vector3(item.A-item0.A,item.B-item0.B,item.C-item0.C);
                item.CADOff=new Vector3(item.B-item0.B,item.A-item0.A,item.C-item0.C);
                item.PosOff=item.Pos-item0.Pos;
                item.ScaleOff=new Vector3(item.CADOff.x/item.PosOff.x,item.CADOff.y/item.PosOff.y,item.CADOff.z/item.PosOff.z);
            }
        }
    }

    [ContextMenu("SwitchAB")]
    public void SwitchAB()
    {
        foreach (PipePointCalculaterItem item in list)
        {
            if (item.targetObj == null) continue;
            string n= item.targetObj.name;
            float t=item.A;
            item.A=item.B;
            item.B=t;
        }

        // float t1=CADOffset.x;
        // CADOffset.x=CADOffset.y;
        // CADOffset.y=t1;
    }

    [ContextMenu("SetABC")]
    public void SetABC()
    {
        foreach (PipePointCalculaterItem item in list)
        {
            if (item.targetObj == null) continue;
            string n= item.targetObj.name;
            if (n.ToLower().StartsWith("point"))
            {
                var parts = n.Split('_');
                item.A = parts[2].ToFloat();
                item.B = parts[1].ToFloat();
                item.C = 0;
            }
        }
    }

    /// <summary>
    /// ʵ�ʳߴ�/��ά�ߴ�ļ���,�Լ�ԭ��
    /// </summary>
    [ContextMenu("Calculate")]
    public void Calculate()
    {
        direction = new Vector3(direction.x >= 0 ? 1 : -1, direction.y >= 0 ? 1 : -1, direction.z >= 0 ? 1 : -1);

        List<PipePointCalculaterItem> listT = new List<PipePointCalculaterItem>();
        foreach (PipePointCalculaterItem item in list)
        {
            //item.Calculate(CADOffset,direction);
            item.Calculate(direction);
            if (item.ISSuccess)
            {
                listT.Add(item);
            }
        }

        if (listT.Count < 2)
        {
            Debug.LogError("��Ч��׼������С��2");
            return;
        }

        scale = CalculateScale(listT);
        axisZero = CalculateAxisZero(listT);

        if (locationManager != null)
        {
            locationManager.LocationOffsetScale = scale;
            locationManager.axisZero = axisZero;
            locationManager.direction = direction;
        }

        // if (LocationSimulator != null)
        // {
        //     LocationSimulator.LocationOffsetScale = scale;
        //     LocationSimulator.axisZero = axisZero;
        //     LocationSimulator.direction = direction;
        // }

        axisZeroObj.position = axisZero;
    }


    /// <summary>
    /// ����ԭ��ļ���
    /// </summary>
    public Vector3 CalculateAxisZero(List<PipePointCalculaterItem> listT)
    {
        int n = 0;
        Vector3 vNum = Vector3.zero;
        List<Vector3> posList=new List<Vector3>();
        for (int i = 0; i < listT.Count; i++)
        {
            PipePointCalculaterItem item = listT[i];
            Vector3 trans = new Vector3(item._B / scale.x, 0, item._A / scale.z);
            Vector3 r = item.targetObj.transform.position - trans;
            posList.Add(r);
            vNum += r;
            n++;
            //Debug.LogError("��" + (n) + "����ԭ�㣺" + r);
        }

        Vector3 result = Vector3.zero;
        if (n > 0)
        {
            result = vNum / n;
        }
        else
        {
            result = Vector3.zero;
        }
        for (int i = 0; i < posList.Count; i++)
        {
            Vector3 pos = posList[i];

            Debug.Log($"��{(i+1)}����ԭ�㣺{pos} offset:{(result-pos)} dis:{Vector3.Distance(result,pos)}" );
        }
        return result;
    }

    /// <summary>
    /// ʵ�ʳߴ�/��ά�ߴ�ļ���
    /// </summary>
    public Vector3 CalculateScale(List<PipePointCalculaterItem> listT)
    {

        int n = 0;
        Vector3 scaleNum = Vector3.zero;
        Vector3 result = Vector3.zero;
        //foreach (LocationCalculaterItem item in listT)
        //{
        //    if (item.ISSuccess)
        //    {
        //        n++;
        //        scaleT += item.scale;
        //    }
        //}

        // for (int i = 0; i < listT.Count; i++)
        // {
        //     if (i + 1 < listT.Count)
        //     {
        //         if (listT[i].A == listT[i + 1].A && listT[i].B == listT[i + 1].B) continue;//��ͬ�㲻�������

        //         Vector3 r = CalculateTwoItem(listT[i], listT[i + 1]);
        //         scaleNum += r;
        //         float p1=r.x/r.z;
        //         Debug.LogError($"{listT[i].GetName()} -> {listT[i + 1].GetName()} ����������1:{r} p1:{p1}" );
        //         n++;
        //         continue;
        //     }

        //     if (listT.Count >= 2 && i == listT.Count - 1)//���һ����׼�����һ������
        //     {
        //         if (listT[i].A == listT[0].A && listT[i].B == listT[0].B) continue;//��ͬ�㲻�������

        //         Vector3 r= CalculateTwoItem(listT[0], listT[i]);
        //         scaleNum += r;
        //         float p1=r.x/r.z;
        //          Debug.LogError($"{listT[0].GetName()} -> {listT[i].GetName()} ����������1:{r} p1:{p1}" );
        //         n++;
        //         continue;
        //     }
        // }


        for (int i = 0; i < listT.Count; i++)
        {
            for (int j = i+1; j < listT.Count; j++)
            {
                if (listT[i].A == listT[j].A && listT[i].B == listT[j].B) continue;//��ͬ�㲻�������
                
                Vector3 r = CalculateTwoItem(listT[i], listT[j]);
                float p1=r.x/r.z;

                Debug.LogError($"CalculateTwoItem1[{i}|{j}] {listT[i].GetName()} -> {listT[j].GetName()} result:({r.x:F5},{r.y:F5},{r.z:F5}) p1:{p1} " );

                if(p1>2||p1<0.5f)continue;

                scaleNum += r;
                n++;
                
                Debug.Log($"CalculateTwoItem2[{n},{i}|{j}] {listT[i].GetName()} -> {listT[j].GetName()} result:({r.x:F5},{r.y:F5},{r.z:F5}) p1:{p1}" );
                
            }
        }

        if (n > 0)
        {
            result = scaleNum / n;
        }
        else
        {
            result = Vector3.zero;
        }

        if (result.y == 0)
        {
            result.y = (result.x + result.z) / 2f;//һ����˵��x\y\z����ͬ��
        }
        return result;
    }

    private Vector3 CalculateTwoItem(PipePointCalculaterItem item1, PipePointCalculaterItem item2)
    {
        float BDIs = item1._B - item2._B;//��ӦX
        float CDis = 0;//��ӦY
        float ADis = item1._A - item2._A; //��ӦZ

        bool isC = true;
        if (item1._C != 0 && item2._C != 0)
        {
            CDis = item1._C - item2._C;
            isC = true;
        }
        else
        {
            CDis = 0;
            isC = false;
        }
        BDIs = Math.Abs(BDIs);
        CDis = Math.Abs(CDis);
        ADis = Math.Abs(ADis);


        float XDis_3d = item1.targetObj.transform.position.x - item2.targetObj.transform.position.x;
        float YDIs_3d = 0;
        float ZDis_3d = item1.targetObj.transform.position.z - item2.targetObj.transform.position.z;
        if (isC)
        {
            YDIs_3d = item1.targetObj.transform.position.y - item2.targetObj.transform.position.y;
        }

        XDis_3d = Math.Abs(XDis_3d);
        YDIs_3d = Math.Abs(YDIs_3d);
        ZDis_3d = Math.Abs(ZDis_3d);

        Vector3 result = Vector3.zero;

        float xtemp = BDIs / XDis_3d;
        float ztemp = ADis / ZDis_3d;

        xtemp = xtemp == 0 ? ztemp : xtemp;
        ztemp = ztemp == 0 ? xtemp : ztemp;

        if (isC)
        {
            result = new Vector3(xtemp, CDis / YDIs_3d, ztemp);
        }
        else
        {
            result = new Vector3(xtemp, 0, ztemp);
        }

        return result;
    }

    public Vector3 TestUnityPos ;

    public Vector3 TestCADPos ;

    int count=0;

    public Vector3 CadToUnityPosEx(PipePoint p)
    {
        return CadToUnityPosEx(p,GetUnityOffset());
    }

    public Vector3 CadToUnityPosEx(PipePoint p,Vector3 off)
    {
        // count++;
        // //4964330.302,437856.59,585.051
        // Vector3 cadPos1=new Vector3(p.X,p.Y,p.Z)-CADOffset;
        // Vector3 cadPos2;
        // if(isXZY)
        // {
        //     cadPos2=new Vector3(cadPos1.x,cadPos1.z,cadPos1.y);
        // }
        // else{//YZX
        //     cadPos2=new Vector3(cadPos1.y,cadPos1.z,cadPos1.x);
        // }
        // Vector3 unityPos=CadToUnityPos(cadPos2,false);
        // //Vector3 unityPos1=unityPos;
        // Vector3 unityPos1=new Vector3(unityPos.z,unityPos.y,unityPos.x);
        // Vector3 unityPos2=unityPos1+UnityOffset;

        // Debug.Log($"CadToUnityPosEx[{count}] {p} > {cadPos1} > {cadPos2} > {unityPos} > {unityPos2}");
        // return unityPos2;
        return CadToUnityPosEx(new Vector3d(p.X,p.Y,p.Z),off);
    }

    public bool IsSwithXY=false;

    public Vector3 CadToUnityPosEx(Vector3d p,Vector3 off)
    {
        count++;
        //4964330.302,437856.59,585.051
        if(IsSwithXY || p.x < 4064000){
            double t=p.x;
            p.x=p.y;
            p.y=t;
        }
        Vector3d cadPos1=p-CADOffset;
        Vector3d cadPos2;
        if(isXZY)
        {
            cadPos2=new Vector3d(cadPos1.x,cadPos1.z,cadPos1.y);
        }
        else{//YZX
            cadPos2=new Vector3d(cadPos1.y,cadPos1.z,cadPos1.x);
        }
        Vector3d unityPos=CadToUnityPos(cadPos2,false);
        //Vector3 unityPos1=unityPos;
        Vector3d unityPos1=new Vector3d(unityPos.z,unityPos.y,unityPos.x);
        Vector3d unityPos2=unityPos1+off;

        //Debug.Log($"CadToUnityPosEx[{count}] {p} > {cadPos1} > {cadPos2} > {unityPos} > {unityPos2} | ({p.x:F5},{p.y:F5},{p.z:F5})");

        //Vector3d p0=UnityPosToCadEx(unityPos2);
        //Debug.Log($"CadToUnityPosEx[{count}] {p} > {cadPos1} > {cadPos2} > {unityPos} > {unityPos2} | ({p.x:F4},{p.y:F4},{p.z:F4}) > ({p0.x:F4},{p0.y:F4},{p0.z:F4}) ");
        return unityPos2;
    }

    public Vector3d UnityPosToCadEx(Vector3 p)
    {
        return UnityPosToCadEx(p,GetUnityOffset());
    }

    public Vector3d UnityPosToCadEx(Vector3 unityPos2,Vector3 off)
    {
        Vector3 unityPos1=unityPos2-off;
        Vector3d unityPos=new Vector3d(unityPos1.z,unityPos1.y,unityPos1.x);
        Vector3d cadPos2=UnityToCadPos(unityPos,false);
        Vector3d cadPos1=Vector3.zero;
        if(isXZY)
        {
            cadPos1=new Vector3d(cadPos2.x,cadPos2.z,cadPos2.y);
        }
        else{//YZX
            cadPos1=new Vector3d(cadPos2.z,cadPos2.x,cadPos2.y);
        }

        Vector3d p=cadPos1+CADOffset;
        count++;
        //4964330.302,437856.59,585.051
        if(IsSwithXY || p.x < 4064000){
            double t=p.x;
            p.x=p.y;
            p.y=t;
        }
        //Debug.Log($"UnityPosToCadEx[{count}] {unityPos2} > {unityPos1} > {unityPos} > {cadPos2} > {cadPos1} | ({p.x:F5},{p.y:F5},{p.z:F5})");
        return p;
    }

    [ContextMenu("TestUnityAndCADPos")]
    private void TestUnityAndCADPos()
    {
        var cadPos1=UnityToCadPos(TestUnityPos,true);
        var cadPos2=UnityToCadPos(TestUnityPos,false);
        var unityPos1=CadToUnityPos(TestCADPos,true);
        var unityPos2=CadToUnityPos(TestCADPos,false);

        Debug.Log($"TestUnityAndCADPos \nUnityToCadPos1:{TestUnityPos} > {cadPos1}\nUnityToCadPos2:{TestUnityPos} > {cadPos2}\nCadToUnityPos1:{TestCADPos} > {unityPos1}\nCadToUnityPos2:{TestCADPos} > {unityPos2}");
    }

    public string CSVFile=@"D:\WorkSpace\GitHub3\SceneRebuilder\Assets\Modules\PipeSystems\Files\��ҵ�㳡���¹���.csv";

    [ContextMenu("TestPipePoints")]
    private void TestPipePoints()
    {
        GameObject root=new GameObject("TestPipePoints");
        root.transform.position=Vector3.zero;
        
        Debug.Log($"TestPipePoints list:{list.Count}");
        Vector3 sum=Vector3.zero;
        foreach(var item in list){
            Vector3 p=new Vector3(item.A,item.B,item.C);
            p=p+CADOffset;
            //var p2=CadToUnityPosEx(p,UnityOffset);
            var p2=CadToUnityPosEx(p,Vector3.zero);
            GameObject go=GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name=item.GetName();
            go.transform.position=p2;
            go.transform.SetParent(root.transform);
            Vector3 v=p2-item.targetObj.transform.position;
            sum+=v;
            go.name+=$"_{v.x:F5},{v.y:F5},{v.z:F5}";
        }
        UnityOffset=-sum/list.Count;

        var points = PipeSystemUtils.ReadCSVFile(CSVFile);
        Debug.Log($"TestPipePoints points:{points.Count}");
        for(int i=0;i<points.Count;i++){
            var p=points[i];
            var p2=CadToUnityPosEx(p,GetUnityOffset());
            GameObject go=GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name=p.ToString();
            go.transform.position=p2;
            go.transform.SetParent(root.transform);
        }
    }

    //#region CAD��Unityλ��ת��
    /// <summary>
    /// CADλ��תUnityλ��
    /// </summary>
    /// <param name="cadPos"></param>
    /// <param name="isLocalPos"></param>
    /// <returns></returns>
    public Vector3d CadToUnityPos(Vector3d cadPos, bool isLocalPos)
    {
        Vector3d pos;
        if (!isLocalPos)
        {
            pos = GetRealVector(cadPos);
        }
        else
        {
            pos = CadToUnityLocalPos(cadPos);
        }
        return pos;
    }
    private Vector3d CadToUnityLocalPos(Vector3d p)
    {
        //����������ʵ��������άģ�͵ĽǶȲ�ͬ YZL20201224
        p = new Vector3d(this.direction.x * p.x, p.y, this.direction.z * p.z);

        Vector3d pos;
        Vector3d offsetScale = scale;
        if (offsetScale.y == 0)
        {
            pos = new Vector3d(p.x / offsetScale.x, p.y / offsetScale.x, p.z / offsetScale.z);
        }
        else
        {
            pos = new Vector3d(p.x / offsetScale.x, p.y / offsetScale.y, p.z / offsetScale.z);
        }
        return pos;
    }

    public Vector3d GetRealVector(Vector3d p)
    {
        Vector3d pos = GetRealSizeVector(p);
        pos = pos + axisZero;
        return pos;
    }

    public Vector3d GetRealSizeVector(Vector3d p)
    {
        //����������ʵ��������άģ�͵ĽǶȲ�ͬ
        //p = new Vector3(-p.x, p.y, -p.z);
        p = new Vector3d(this.direction.x * p.x, p.y, this.direction.z * p.z);

        return GetRealSizeVs(p);
    }

    /// <summary>
    /// Unityλ��תCADλ��
    /// </summary>
    public Vector3d UnityToCadPos(Vector3d unityPos, bool isLocalPos)
    {
        Vector3d pos;
        if (!isLocalPos)
        {
            pos = GetCadVector(unityPos);
        }
        else
        {
            pos = UnityLocalPosToCad(unityPos);
        }
        return pos;
    }

        /// <summary>
    /// Postionת����CADλ��
    /// </summary>
    /// <returns></returns>
    public Vector3d GetCadVector(Vector3d position)
    {
        position -= axisZero;
        Vector3d tempPos;
        if (this.scale.y == 0)
        {
            tempPos = new Vector3d(position.x * this.scale.x, position.y * this.scale.x, position.z * this.scale.z);
        }
        else
        {
            tempPos = new Vector3d(position.x * this.scale.x, position.y * this.scale.y, position.z * this.scale.z);
        }
        tempPos = new Vector3d(tempPos.x / this.direction.x, tempPos.y, tempPos.z / this.direction.z);
        return tempPos;
    }

    /// <summary>
    /// UnityLocalPosתCADPos
    /// </summary>
    /// <param name="localPos"></param>
    /// <returns></returns>
    public Vector3d UnityLocalPosToCad(Vector3d localPos)
    {
        Vector3d tempPos;
        Vector3d offsetScale = this.scale;
        if (offsetScale.y == 0)
        {
            tempPos = new Vector3d(localPos.x * offsetScale.x, localPos.y * offsetScale.x, localPos.z * offsetScale.z);
        }
        else
        {
            tempPos = new Vector3d(localPos.x * offsetScale.x, localPos.y * offsetScale.y, localPos.z * offsetScale.z);
        }
        //����������ʵ��������άģ�͵ĽǶȲ�ͬ YZL20201224
        tempPos = new Vector3d(this.direction.x * tempPos.x, tempPos.y, this.direction.z * tempPos.z);
        return tempPos;
    }

    public Vector3d GetRealSizeVs(Vector3d p)
    {
        Vector3d pos;
        if (scale.y == 0)
        {
            pos = new Vector3d(p.x / scale.x, p.y / scale.x, p.z / scale.z);
        }
        else
        {
            pos = new Vector3d(p.x / scale.x, p.y / scale.y, p.z / scale.z);
        }

        return pos;
    }

    /// <summary>
    /// ����ʵ�ʱ�������������ʵ�����λ��
    /// </summary>
    /// <param name="p"></param>
    public Vector3 GetDisRealVector(Vector3 p)
    {
        p = p - axisZero;
        Vector3 pos = GetDisRealSizeVector(p);

        return pos;
    }

    /// <summary>
    /// ����ʵ�ʱ�����������������ʵ����ĳߴ�
    /// </summary>
    /// <param name="p"></param>
    public Vector3d GetDisRealSizeVector(Vector3d p)
    {
        Vector3d pos= GetDisRealSizeVs(p);
        //����������ʵ��������άģ�͵ĽǶȲ�ͬ
        pos = new Vector3d(this.direction.x * pos.x, pos.y, this.direction.z * pos.z);
        return pos;
    }

    public Vector3d GetDisRealSizeVs(Vector3d p)
    {
        Vector3d pos;
        if (scale.y == 0)
        {
            pos = new Vector3d(p.x * scale.x, p.y *scale.x, p.z * scale.z);
        }
        else
        {
            pos = new Vector3d(p.x * scale.x, p.y * scale.y, p.z * scale.z);
        }

        return pos;
    }
}

[Serializable]
public class PipePointCalculaterItem
{
    public string Name;

    public string GetName()
    {
        if(string.IsNullOrEmpty(Name)){
            if(targetObj){
                Name=targetObj.name;
            }
        }
        return Name;
    }
    public GameObject targetObj;
    /// <summary>
    /// ��ά��Z����ʵ�ʵ��ϱ�����
    /// </summary>
    public float A = 0;//��ά��Z�����ϱ�����
    /// <summary>
    /// ��ά��X����ʵ�ʵĶ�������
    /// </summary>
    public float B = 0;//��ά��X���򣬶�������
    /// <summary>
    /// ��ά��y���򣬸߶ȷ���
    /// </summary>
    public float C = 0;//��ά��y���򣬸߶ȷ���

    /// <summary>
    /// ��ά��Z����ʵ�ʵ��ϱ�����,���ڼ���
    /// </summary>
    //[HideInInspector]
    public float _A = 0;//��ά��Z�����ϱ�����
    /// <summary>
    /// ��ά��X����ʵ�ʵĶ�������,���ڼ���
    /// </summary>
    //[HideInInspector]
    public float _B = 0;//��ά��X���򣬶�������
    /// <summary>
    /// ��ά��y���򣬸߶ȷ���,���ڼ���
    /// </summary>
    //[HideInInspector]
    public float _C = 0;//��ά��y���򣬸߶ȷ���

    public Vector3 Pos;

    public Vector3 CADOff;

    public Vector3 PosOff;

    public Vector3 ScaleOff;

    ///// <summary>
    ///// ʵ�ʳߴ�/��ά�ߴ�
    ///// </summary>
    //public Vector3 scale;
    /// <summary>
    /// �Ƿ���Բ������
    /// </summary>
    public bool iSSuccess = false;

    public bool ISSuccess
    {
        get
        {
            //Calculate();
            return iSSuccess;
        }

        set
        {
            iSSuccess = value;
        }
    }

    /// <summary>
    /// ʵ�ʳߴ�/��ά�ߴ�ļ���
    /// </summary>
    public void Calculate(Vector3 dir)
    {
        //if (targetObj == null || A == 0 || B == 0)
        if (targetObj == null)
        {
            //Debug.LogError("(targetObj == null || A == 0 || B == 0)");
            Debug.LogError("(targetObj == null)");
            iSSuccess = false;
            return;
        }

        //����������ʵ��������άģ�͵ĽǶȲ�ͬ
        SetDirection(dir);
        //_A = -A; 
        // _B = -B;
        //_C = C;

        iSSuccess = true;
    }

    public void Calculate(Vector3 off,Vector3 dir)
    {
        //if (targetObj == null || A == 0 || B == 0)
        if (targetObj == null)
        {
            //Debug.LogError("(targetObj == null || A == 0 || B == 0)");
            Debug.LogError("(targetObj == null)");
            iSSuccess = false;
            return;
        }

        //����������ʵ��������άģ�͵ĽǶȲ�ͬ
        SetDirection(off,dir);
        //_A = -A; 
        // _B = -B;
        //_C = C;

        iSSuccess = true;
    }

    /// <summary>
    /// ����У׼,����������ʵ��������άģ�͵ĽǶȲ�ͬ
    /// </summary>
    /// <param name="v"></param>
    public void SetDirection(Vector3 v)
    {
        _A = v.z * A;
        _B = v.x * B;
        _C = v.y * C;
    }

    /// <summary>
    /// ����У׼,����������ʵ��������άģ�͵ĽǶȲ�ͬ
    /// </summary>
    /// <param name="v"></param>
    public void SetDirection(Vector3 off,Vector3 v)
    {
        _A = v.z * (A+off.x);
        _B = v.x * (B+off.y);
        _C = v.y * (C+off.z);
    }
}
