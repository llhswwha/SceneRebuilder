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

    public bool IsTryRT = true;

    public bool IsTryICP = true;

    public bool IsCheckResult=false;

    public bool IsSetParent=false;

    public float MaxVertexCount=10;

    [ContextMenu("SetAcRTAlignJobSetting")]
    public void SetAcRTAlignJobSetting()
    {
         AcRTAlignJob.IsTryAngles=this.IsTryAngles;
         AcRTAlignJob.IsTryAngles_Scale=this.IsTryAngles_Scale;
        AcRTAlignJob.IsTryRT = this.IsTryRT;
        AcRTAlignJob.IsTryICP = this.IsTryICP;
        AcRTAlignJobContainer.IsCheckResult=this.IsCheckResult;

         AcRTAlignJobContainer.IsSetParent=this.IsSetParent;

         AcRTAlignJobContainer.MaxVertexCount=(int)(this.MaxVertexCount*10000);
    }

    public float MaxModelLength=1.2f;
}