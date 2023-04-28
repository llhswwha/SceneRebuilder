using UIWidgets;

namespace UIWidgetsSamples {
	/// <summary>
	/// ChatView.
	/// </summary>
	public class ChatView : ListViewCustomHeight<ChatLineComponent,ChatLine> {
		/// <summary>
		/// Sets component data with specified item.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <param name="item">Item.</param>
		protected override void SetData(ChatLineComponent component, ChatLine item)
		{
			component.SetData(item);
		}

		// leave coloring functions empty
		protected override void HighlightColoring(ChatLineComponent component)
		{
		}

		protected override void SelectColoring(ChatLineComponent component)
		{
		}
		
		protected override void DefaultColoring(ChatLineComponent component)
		{
		}
	}
}
