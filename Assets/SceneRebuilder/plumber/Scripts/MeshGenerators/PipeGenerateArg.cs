using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PipeGenerateArg 
{
    public Material pipeMaterial;

    public Material weldMaterial;

    public int pipeSegments = 16;

    public int elbowSegments = 6;

    public int weldPipeSegments = 12;//焊缝的管道面数

    public int weldElbowSegments = 6;//焊缝的弯管段数 n*4

    public float weldRadius = 0.005f;

    public bool generateWeld = false;

    public bool makeDoubleSided = false;

    public bool generateEndCaps = false;

    public bool IsGenerateEndWeld = true;

    public override string ToString()
    {
        return $"{pipeSegments}_{elbowSegments}_{weldPipeSegments}_{weldElbowSegments}";
    }


    public Vector3 Offset = Vector3.zero;

    public void SetArg(PipeMeshGenerator pipe)
    {
        if (this.pipeMaterial == null)
        {
            this.pipeMaterial = PipeFactory.Instance.pipeMaterial;
        }
        if (this.weldMaterial == null)
        {
            this.weldMaterial = PipeFactory.Instance.weldMaterial;
        }

        pipe.pipeSegments = this.pipeSegments;
        pipe.pipeMaterial = this.pipeMaterial;
        pipe.weldMaterial = this.weldMaterial;
        pipe.weldRadius = this.weldRadius;
        pipe.generateWeld = this.generateWeld;
        pipe.elbowSegments = this.elbowSegments;
        pipe.makeDoubleSided = this.makeDoubleSided;
        pipe.generateEndCaps = this.generateEndCaps;
        pipe.IsGenerateEndWeld = this.IsGenerateEndWeld;
        pipe.weldPipeSegments = this.weldPipeSegments;
        pipe.weldElbowSegments = this.weldElbowSegments;
    }

    public void SetArg(PipeMeshGeneratorEx pipe)
    {
        if (this.pipeMaterial == null)
        {
            this.pipeMaterial = PipeFactory.Instance.pipeMaterial;
        }
        if (this.weldMaterial == null)
        {
            this.weldMaterial = PipeFactory.Instance.weldMaterial;
        }

        pipe.pipeSegments = this.pipeSegments;
        pipe.pipeMaterial = this.pipeMaterial;
        pipe.weldMaterial = this.weldMaterial;
        pipe.weldRadius = this.weldRadius;
        pipe.generateWeld = this.generateWeld;
        pipe.elbowSegments = this.elbowSegments;
        pipe.makeDoubleSided = this.makeDoubleSided;
        pipe.generateEndCaps = this.generateEndCaps;
        pipe.weldPipeSegments = this.weldPipeSegments;
        pipe.weldElbowSegments = this.weldElbowSegments;
    }

    public PipeGenerateArg Clone()
    {
        PipeGenerateArg pipe = new PipeGenerateArg();
        pipe.pipeSegments = this.pipeSegments;
        pipe.pipeMaterial = this.pipeMaterial;
        pipe.weldMaterial = this.weldMaterial;
        pipe.weldRadius = this.weldRadius;
        pipe.generateWeld = this.generateWeld;
        pipe.elbowSegments = this.elbowSegments;
        pipe.makeDoubleSided = this.makeDoubleSided;
        pipe.generateEndCaps = this.generateEndCaps;
        pipe.IsGenerateEndWeld = this.IsGenerateEndWeld;
        pipe.weldPipeSegments = this.weldPipeSegments;
        pipe.weldElbowSegments = this.weldElbowSegments;
        return pipe;
    }



    
}
