using UnityEngine;
using UnityEngine.UI;

namespace UIWidgets {
	/// <summary>
	/// DialogButtonComponent.
	/// Control how button name will be displayed.
	/// </summary>
	public class DialogButtonComponent : MonoBehaviour {
		/// <summary>
		/// The name.
		/// </summary>
		[SerializeField]
		public Text Name;

		/// <summary>
		/// Sets the button name.
		/// </summary>
		/// <param name="name">Name.</param>
		public virtual void SetButtonName(string name)
		{
			Name.text = name;
		}
	}
}