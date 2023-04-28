using UnityEngine;

namespace UIWidgets
{
	/// <summary>
	/// TabsIcons.
	/// </summary>
	[AddComponentMenu("UI/UIWidgets/TabsIcons")]
	public class TabsIcons : TabsCustom<TabIcons,TabIconButton>
	{
		/// <summary>
		/// Sets the name of the button.
		/// </summary>
		/// <param name="button">Button.</param>
		/// <param name="index">Index.</param>
		protected override void SetButtonData(TabIconButton button, int index)
		{
			button.SetData(TabObjects[index]);
		}
	}
}