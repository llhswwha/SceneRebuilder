using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

[XmlType(TypeName = "AlarmSetting")]
public class AlarmSetting
{
    /// <summary>
    /// 进入电厂时是否显示告警推送
    /// </summary>
    [XmlAttribute]
    public bool PushAlarmShow = true ;
}

