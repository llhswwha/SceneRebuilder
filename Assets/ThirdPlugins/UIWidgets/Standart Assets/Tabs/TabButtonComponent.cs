using UnityEngine;
using UnityEngine.UI;

namespace UIWidgets {
	/// <summary>
	/// Tab component.
	/// </summary>
	public class TabButtonComponent : MonoBehaviour {
		/// <summary>
		/// The name.
		/// </summary>
		[SerializeField]
		public Text Name;

		/// <summary>
		/// Sets the data.
		/// </summary>
		/// <param name="tab">Tab.</param>
		public virtual void SetButtonData(Tab tab)
		{
			Name.text = tab.Name;
		}
	}
}