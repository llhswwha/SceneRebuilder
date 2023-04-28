using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UIWidgets {

	/// <summary>
	/// List view item component.
	/// </summary>
	[AddComponentMenu("UI/UIWidgets/ListViewStringComponent")]
	public class ListViewStringComponent : ListViewItem
	{
		/// <summary>
		/// The Text component.
		/// </summary>
		public Text Text;

		public virtual void SetData(string text)
		{
			Text.text = text.Replace("\\n", "\n");
		}
	}
}