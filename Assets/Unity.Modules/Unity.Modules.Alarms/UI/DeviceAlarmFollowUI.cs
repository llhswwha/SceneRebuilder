using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// DeviceAlarm的告警信息展示(边界，后来又有门禁）
/// </summary>
public class DeviceAlarmFollowUI : MonoBehaviour {
    /// <summary>
    /// 页面Toggle
    /// </summary>
    public Toggle devToggle;

    /// <summary>
    /// 信息窗体
    /// </summary>
    public GameObject InfoWindow;
    /// <summary>
    /// 告警内容
    /// </summary>
    public Text alarmInfoText;

    public List<Sprite> redAlarmSprite;//告警UI
    public List<Sprite> yellowAlarmSprite;//分组内设备告警UI

    // Use this for initialization
    void Start () {
        InitToggle();
    }

    protected void InitToggle()
    {
        if (devToggle == null)
        {
            Log.Error("DeviceAlarmFollowUI.InitToggle", "devToggle == null");
        }
        else
        {
            devToggle.onValueChanged.AddListener(OnToggleValueChanged);
        }        
    }

    protected List<DeviceAlarm> alarmInfos = new List<DeviceAlarm>();

    /// <summary>
    /// 初始化信息
    /// </summary>
    /// <param name="alarmInfo"></param>
    public void InitInfo(DeviceAlarm alarmInfo)
    {
        AddAlarmInfo(alarmInfo);
        SetText();
    }
    /// <summary>
    /// 添加告警信息
    /// </summary>
    /// <param name="alarmInfo"></param>
    private void AddAlarmInfo(DeviceAlarm alarmInfo)
    {
        //去除内容重复的告警
        DeviceAlarm alarm = alarmInfos.Find(i=>i!=null&&alarmInfo!=null&&i.Message==alarmInfo.Message);
        if (alarm != null) return;
        else alarmInfos.Add(alarmInfo);
    }
    /// <summary>
    /// 更换UI状态
    /// </summary>
    /// <param name="alarm"></param>
    public void ChangeAlarmSprite(bool isGroupAlarm = false)
    {
        if (isGroupAlarm)
        {
            if (yellowAlarmSprite == null || yellowAlarmSprite.Count < 3) return;
            ChangeToggleSprite(devToggle, yellowAlarmSprite[0], yellowAlarmSprite[1], yellowAlarmSprite[2]);
        }
        else
        {
            if (redAlarmSprite == null || redAlarmSprite.Count < 3) return;
            ChangeToggleSprite(devToggle, redAlarmSprite[0], redAlarmSprite[1], redAlarmSprite[2]);
        }
    }
    /// <summary>
    /// 更换Toggle Sprite图片
    /// </summary>
    /// <param name="normal"></param>
    /// <param name="hover"></param>
    /// <param name="checkedSprite"></param>
    private void ChangeToggleSprite(Toggle devToggle, Sprite normal, Sprite hover, Sprite checkedSprite)
    {
        if (devToggle == null || devToggle.targetGraphic == null || devToggle.graphic == null) return;
        if (hover != null)
        {
            SpriteState state = new SpriteState();
            state.highlightedSprite = hover;
            devToggle.spriteState = state;
        }

        Image img = devToggle.targetGraphic.GetComponent<Image>();
        if (img && normal) img.sprite = normal;

        Image check = devToggle.graphic.GetComponent<Image>();
        if (img && checkedSprite) check.sprite = checkedSprite;
    }
    protected virtual string GetText()
    {
        string txt = "";
        foreach (var item in alarmInfos)
        {
            txt += string.Format("{0}:{1}\n", item.Title, item.Message);
        }
        txt = txt.Trim();
        return txt;
    }

    private void SetText()
    {
        alarmInfoText.text = "";
        if(alarmInfos!=null&&alarmInfos.Count>0)alarmInfoText.text += string.Format("设备名称：{0}\n",alarmInfos[0].DevName);
        alarmInfoText.text += GetText();
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            SetText();
            InfoWindow.SetActive(true);
        }
        else
        {
            InfoWindow.SetActive(false);
        }
    }

}
