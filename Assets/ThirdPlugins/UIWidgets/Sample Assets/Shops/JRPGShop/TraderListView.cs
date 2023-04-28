using UnityEngine;
using System.Collections;
using UIWidgets;

namespace UIWidgetsSamples.Shops {
	/// <summary>
	/// Trader list view.
	/// </summary>
	public class TraderListView : ListViewCustom<TraderListViewComponent,JRPGOrderLine> {
		/// <summary>
		/// Sets component data with specified item.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <param name="item">Item.</param>
		protected override void SetData(TraderListViewComponent component, JRPGOrderLine item)
		{
			component.SetData(item);
		}

		/// <summary>
		/// Set highlights colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void HighlightColoring(TraderListViewComponent component)
		{
			base.HighlightColoring(component);
			component.Name.color = HighlightedColor;
			component.Price.color = HighlightedColor;
			component.AvailableCount.color = HighlightedColor;
		}

		/// <summary>
		/// Set select colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void SelectColoring(TraderListViewComponent component)
		{
			base.SelectColoring(component);
			component.Name.color = SelectedColor;
			component.Price.color = SelectedColor;
			component.AvailableCount.color = SelectedColor;
		}

		/// <summary>
		/// Set default colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void DefaultColoring(TraderListViewComponent component)
		{
			base.DefaultColoring(component);
			component.Name.color = DefaultColor;
			component.Price.color = DefaultColor;
			component.AvailableCount.color = DefaultColor;
		}
	}
}