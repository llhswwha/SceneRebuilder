using UIWidgets;

namespace UIWidgetsSamples.Shops {
	/// <summary>
	/// Harbor list view.
	/// </summary>
	public class HarborListView : ListViewCustom<HarborListViewComponent,HarborOrderLine> {
		/// <summary>
		/// Sets component data with specified item.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <param name="item">Item.</param>
		protected override void SetData(HarborListViewComponent component, HarborOrderLine item)
		{
			component.SetData(item);
		}

		/// <summary>
		/// Set highlights colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void HighlightColoring(HarborListViewComponent component)
		{
			base.HighlightColoring(component);
			component.Name.color = HighlightedColor;
			component.BuyPrice.color = HighlightedColor;
			component.SellPrice.color = HighlightedColor;
			component.AvailableBuyCount.color = HighlightedColor;
			component.AvailableSellCount.color = HighlightedColor;
		}

		/// <summary>
		/// Set select colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void SelectColoring(HarborListViewComponent component)
		{
			base.SelectColoring(component);
			component.Name.color = SelectedColor;
			component.BuyPrice.color = SelectedColor;
			component.SellPrice.color = SelectedColor;
			component.AvailableBuyCount.color = SelectedColor;
			component.AvailableSellCount.color = SelectedColor;
		}

		/// <summary>
		/// Set default colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void DefaultColoring(HarborListViewComponent component)
		{
			base.DefaultColoring(component);
			component.Name.color = DefaultColor;
			component.BuyPrice.color = DefaultColor;
			component.SellPrice.color = DefaultColor;
			component.AvailableBuyCount.color = DefaultColor;
			component.AvailableSellCount.color = DefaultColor;
		}
	}
}