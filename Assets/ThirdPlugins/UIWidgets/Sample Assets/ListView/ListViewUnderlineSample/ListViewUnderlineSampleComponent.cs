using UnityEngine;
using UnityEngine.UI;
using UIWidgets;

namespace UIWidgetsSamples {
	public class ListViewUnderlineSampleComponent : ListViewItem {
		// specify components for displaying item data
		[SerializeField]
		public Image Icon;
		
		[SerializeField]
		public Text Text;
		
		[SerializeField]
		public Image Underline;
		
		// Displaying item data
		public void SetData(ListViewUnderlineSampleItemDescription item)
		{
			Icon.sprite = item.Icon;
			Text.text = item.Name;

			Icon.SetNativeSize();
			
			Icon.color = (Icon.sprite==null) ? Color.clear : Color.white;
		}
	}
}