using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class PipeModelSaveData
{
    [XmlAttribute]
    public string Name;

    [XmlAttribute]
    public string Id;

    public List<string> Path;

    public TransformInfo Transform;

    public override string ToString()
    {
        return $"Name:{Name} Id:{Id}";
    }
}
