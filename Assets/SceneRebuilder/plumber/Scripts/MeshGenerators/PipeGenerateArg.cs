using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[Serializable]
public class PipeGenerateArg 
{
    [XmlIgnore]
    public Material pipeMaterial;

    [XmlIgnore]
    public Material weldMaterial;

    [XmlAttribute]
    public int pipeSegments = 12;

    [XmlAttribute]
    public int elbowSegments = 6;

    [XmlAttribute]
    public int weldPipeSegments = 8;//焊缝的管道面数

    [XmlAttribute]
    public int weldElbowSegments = 6;//焊缝的弯管段数 n*4

    [XmlAttribute]
    public float weldRadius = 0.005f;

    [XmlAttribute]
    public bool generateWeld = true;

    [XmlAttribute]
    public bool makeDoubleSided = false;

    [XmlAttribute]
    public bool generateEndCaps = false;

    [XmlAttribute]
    public float StartCapOffset = 0;

    [XmlAttribute]
    public float EndCapOffset = 0;

    [XmlAttribute]
    public bool IsGenerateEndWeld = true;

    [XmlAttribute]
    public int uniformRadiusP = 4;

    [XmlAttribute]
    public float weldCircleRadius = 0;

    public override string ToString()
    {
        return $"{pipeSegments}_{elbowSegments}_{weldPipeSegments}_{weldElbowSegments}";
    }


    public Vector3 Offset = Vector3.zero;

    public void SetArg(PipeMeshGenerator pipe)
    {
        if (this.pipeMaterial == null)
        {
            this.pipeMaterial = PipeFactory.Instance.GetPipeMaterial();
        }
        if (this.weldMaterial == null)
        {
            this.weldMaterial = PipeFactory.Instance.GetWeldMaterial();
        }

        pipe.pipeSegments = this.pipeSegments;
        pipe.pipeMaterial = this.pipeMaterial;
        pipe.weldMaterial = this.weldMaterial;
        pipe.weldPipeRadius = this.weldRadius;
        pipe.generateWeld = this.generateWeld;
        pipe.elbowSegments = this.elbowSegments;
        pipe.makeDoubleSided = this.makeDoubleSided;
        pipe.generateEndCaps = this.generateEndCaps;
        pipe.IsGenerateEndWeld = this.IsGenerateEndWeld;
        pipe.weldPipeSegments = this.weldPipeSegments;
        pipe.weldElbowSegments = this.weldElbowSegments;
        pipe.uniformRadiusP = this.uniformRadiusP;
        pipe.StartCapOffset = this.StartCapOffset;
        pipe.EndCapOffset = this.EndCapOffset;
        pipe.IsGenerateEndWeld = this.IsGenerateEndWeld;
        pipe.weldCircleRadius = this.weldCircleRadius;
    }

    public void SetArg(PipeMeshGeneratorEx pipe)
    {
        if (this.pipeMaterial == null)
        {
            this.pipeMaterial = PipeFactory.Instance.GetPipeMaterial();
        }
        if (this.weldMaterial == null)
        {
            this.weldMaterial = PipeFactory.Instance.GetWeldMaterial();
        }

        pipe.pipeSegments = this.pipeSegments;
        pipe.pipeMaterial = this.pipeMaterial;
        pipe.weldMaterial = this.weldMaterial;
        pipe.weldPipeRadius = this.weldRadius;
        pipe.generateWeld = this.generateWeld;
        pipe.elbowSegments = this.elbowSegments;
        pipe.makeDoubleSided = this.makeDoubleSided;
        pipe.generateEndCaps = this.generateEndCaps;
        pipe.weldPipeSegments = this.weldPipeSegments;
        pipe.weldElbowSegments = this.weldElbowSegments;
        pipe.uniformRadiusP = this.uniformRadiusP;
        pipe.StartCapOffset = this.StartCapOffset;
        pipe.EndCapOffset = this.EndCapOffset;
        pipe.IsGenerateEndWeld = this.IsGenerateEndWeld;
        pipe.weldCircleRadius = this.weldCircleRadius;
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
        pipe.uniformRadiusP = this.uniformRadiusP;
        pipe.StartCapOffset = this.StartCapOffset;
        pipe.EndCapOffset = this.EndCapOffset;
        pipe.IsGenerateEndWeld = this.IsGenerateEndWeld;
        pipe.weldCircleRadius = this.weldCircleRadius;
        return pipe;
    }



    
}
