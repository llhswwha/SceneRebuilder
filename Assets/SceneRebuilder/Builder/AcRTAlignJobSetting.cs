using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MeshJobs;
using UnityEngine;
using System.IO;

public class AcRTAlignJobSetting : SingletonBehaviour<AcRTAlignJobSetting>
{
    public bool IsTryAngles=true;

    public bool IsTryAngles_Scale= false;

    public bool IsTryRT = false;

    public bool IsTryICP = false;

    public bool IsCheckResult=true;

    public bool IsSetParent=false;

    public float MaxVertexCount=10;

    public float CompareSizeMinValue = 1.5f;

    [ContextMenu("SetAcRTAlignJobSetting")]
    public void SetAcRTAlignJobSetting()
    {
         AcRTAlignJob.IsTryAngles=this.IsTryAngles;
         AcRTAlignJob.IsTryAngles_Scale=this.IsTryAngles_Scale;
        AcRTAlignJob.IsTryRT = this.IsTryRT;
        AcRTAlignJob.IsTryICP = this.IsTryICP;
        AcRTAlignJobContainer.IsCheckResult=this.IsCheckResult;
        AcRTAlignJobContainer.CompareSizeMinValue = this.CompareSizeMinValue;
         AcRTAlignJobContainer.IsSetParent=this.IsSetParent;

         AcRTAlignJobContainer.MaxVertexCount=(int)(this.MaxVertexCount*10000);
    }

    public float MaxModelLength=1.2f;
}