using Battlehub.RTCommon;
using Battlehub.UIControls.DockPanels;
using UnityEngine;

namespace Battlehub.RTEditor
{
    [DefaultExecutionOrder(-91)]
    public class AutoSaveLayout : EditorExtension
    {
        private IWindowManager m_wm;

        private string m_savedLayoutName;

        public bool IsSaveWhenExit=true;

        protected override void OnEditorCreated(object obj)
        {
            m_wm = IOC.Resolve<IWindowManager>();
            OverrideDefaultLayout();
            m_savedLayoutName = m_wm.DefaultPersistentLayoutName;
        }

        protected override void OnEditorExist()
        {
            OverrideDefaultLayout();

            IRuntimeEditor editor = IOC.Resolve<IRuntimeEditor>();
            if (editor == null) return;//YZL20220209:报null
            if (editor.IsOpened)
            {
                m_wm = IOC.Resolve<IWindowManager>();
                m_wm.SetLayout(DefaultLayout, RuntimeWindowType.Scene.ToString());
            }
        }

        protected override void OnEditorClosed()
        {
            base.OnEditorClosed();
            string layoutName = m_wm.DefaultPersistentLayoutName;
            if(IsSaveWhenExit)
                m_wm.SaveLayout(m_savedLayoutName);
            m_wm = null;
        }

        private void OnApplicationQuit()
        {
            if(m_wm != null)
            {
                if(IsSaveWhenExit)
                    m_wm.SaveLayout(m_savedLayoutName);
            }
        }

        private void OverrideDefaultLayout()
        {
            IWindowManager wm = IOC.Resolve<IWindowManager>();
            if (wm != null) wm.OverrideDefaultLayout(DefaultLayout, RuntimeWindowType.Scene.ToString());//YZL崩溃修改：20220124
        }

        protected virtual LayoutInfo DefaultLayout(IWindowManager wm)
        {
            Debug.LogError("AutoSaveLayout.DefaultLayout:"+m_savedLayoutName);
            if (wm.LayoutExist(m_savedLayoutName))
            {
                LayoutInfo layout = wm.GetLayout(m_savedLayoutName);
                if (layout.Child0 != null || layout.Child1 != null)
                {
                    if (wm.ValidateLayout(layout))
                    {
                        return layout;
                    }
                    else
                    {
                        Debug.LogWarning("Saved layout is corrupted. Restoring default layout");
                        Transform[] allWindows = wm.GetWindows();
                        for (int i = 0; i < allWindows.Length; ++i)
                        {
                            wm.DestroyWindow(allWindows[i]);
                        }
                    }
                }
            }

            return wm.GetBuiltInDefaultLayout();
        }
    }

}
