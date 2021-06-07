using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Unity.ComnLib.Utils
{
    public class GameObjectList:List<GameObject>
    {
        /// <summary>
        /// 能否重复
        /// </summary>
        public bool CanRepeat { get; set; }

        public GameObjectList(bool canRepeat=true)
        {
            CanRepeat = canRepeat;
        }

        public GameObjectList(Dictionary<string, GameObject> dict)
        {
            CanRepeat = true;
            AddDict(dict);
        }

        public GameObjectList(IEnumerable<GameObject> list)
        {
            CanRepeat = true;
            base.AddRange(list);
        }

        public new void Add(GameObject obj)
        {
            if (!CanRepeat)
            {
                if (!Contains(obj))
                {
                    base.Add(obj);
                }
            }
            else
            {
                base.Add(obj);
            }
        }

        public void AddDict(Dictionary<string, GameObject> dict)
        {
            if (dict == null) return;
            foreach (GameObject o in dict.Values)
            {
                this.Add(o);
            }
        }

        public new void Remove(GameObject obj)
        {
            if (!Contains(obj))
            {
                base.Remove(obj);
            }
        }

        public GameObjectList GetListByTag(string tagName)
        {
            GameObjectList result = new GameObjectList();
            foreach (GameObject item in this)
            {
                if (item.tag == tagName)
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public GameObjectList GetListByNoTag(string tagName)
        {
            GameObjectList result = new GameObjectList();
            foreach (GameObject item in this)
            {
                if (item.tag != tagName)
                {
                    result.Add(item);
                }
            }
            return result;
        }
    }
}
