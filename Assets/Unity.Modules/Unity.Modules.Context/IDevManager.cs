using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Unity.Modules.Context
{
    public interface IDevManager
    {
        /// <summary>
        /// 聚焦设备(去掉int类型，统一用string,方法内部区分int还是Guid的devId)
        /// </summary>
        /// <param name="devId"></param>
        /// <param name="depId"></param>
        /// <param name="onFocusComplete"></param>
        void FocusDev(string devId, int depId, Action<bool> onFocusComplete = null);
    }
}