using Battlehub.ProBuilderIntegration;
using Battlehub.RTCommon;
using Battlehub.RTEditor;

namespace Battlehub.RTBuilder.HDRP
{
    public class ProBuilderInitHDRP : EditorExtension
    {
        protected override void OnEditorExist()
        {
            if (RenderPipelineInfo.Type != RPType.HDRP)
            {
                Destroy(this);
                return;
            }

            base.OnEditorExist();
            PBSelectionPicker.Renderer = new PBSelectionPickerRendererHDRP();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
       
        }

        protected override void OnEditorClosed()
        {
            base.OnEditorClosed();
        }
    }
}
