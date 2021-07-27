using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsBoxManagerUI : MonoBehaviour
{
    public void ShowBoxes()
    {
        BoundsBoxManager.Instance.ShowBoxes();
    }

    public void HideBoxes()
    {
        BoundsBoxManager.Instance.HideBoxes();
    }
}
