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

    public bool IsTryAngles_Scale=true;

    public bool IsCheckResult=false;

    public bool IsSetParent=false;

    public int MaxVertexCount=2400;

    [ContextMenu("SetAcRTAlignJobSetting")]
    public void SetAcRTAlignJobSetting()
    {
         AcRTAlignJob.IsTryAngles=this.IsTryAngles;
         AcRTAlignJob.IsTryAngles_Scale=this.IsTryAngles_Scale;
         AcRTAlignJobContainer.IsCheckResult=this.IsCheckResult;

         AcRTAlignJobContainer.IsSetParent=this.IsSetParent;

         AcRTAlignJobContainer.MaxVertexCount=this.MaxVertexCount;
    }

    public float MaxModelLength=1.2f;
}