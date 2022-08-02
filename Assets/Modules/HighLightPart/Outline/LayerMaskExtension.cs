using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LayerMaskExtension
{
    public static bool IsInLayer(int _gameObjectLayer, LayerMask _mask)
    {
        return ((1 << _gameObjectLayer) & _mask) != 0;
    }
 
    /// <summary>
    /// Returns a value between [0;31].
    /// Important: This will only work properly if the LayerMask is one created in the inspector and not a LayerMask
    /// with multiple values.
    /// </summary>
    public static int GetLayerNumber(this LayerMask _mask)
    {
        var bitmask = _mask.value;
        int result = bitmask > 0 ? 0 : 31;
        while (bitmask > 1)
        {
            bitmask = bitmask >> 1;
            result++;
        }
        return result;
    }
 
    /// <summary>
    /// Returns a list of values between [0;31].
    /// </summary>
    public static List<int> GetAllLayerNumbers(this LayerMask _mask)
    {
         List<int> layers = new List<int>();
        var bitmask = _mask.value;
        for (int i = 0; i < 32; i++) {
            if (((1 << i) & bitmask) != 0) {
                layers.Add(i);
            }
        }
        return layers;
    }

    /// <summary>
    /// Returns a list of values between [0;31].
    /// </summary>
    public static List<string> GetAllLayerNames(this LayerMask _mask)
    {
         List<string> layers = new List<string>();
        var bitmask = _mask.value;
        for (int i = 0; i < 32; i++) {
            if (((1 << i) & bitmask) != 0) {
                layers.Add(LayerMask.LayerToName(i));
            }
        }
        return layers;
    }
}
