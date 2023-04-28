using UnityEngine;
using System;
using UIWidgets;

namespace UIWidgetsSamples {
	public class Table : ListViewCustom<TableRowComponent,TableRow> {
		// this function is required
		protected override void SetData(TableRowComponent component, TableRow item)
		{
			component.SetData(item);
		}

		// those functions are optional
		protected override void HighlightColoring(TableRowComponent component)
		{
			//base.HighlightColoring(component);
			component.Cell01Text.color = HighlightedColor;
			component.Cell02Text.color = HighlightedColor;
			component.Cell04Text.color = HighlightedColor;
		}
		
		protected override void SelectColoring(TableRowComponent component)
		{
			//base.SelectColoring(component);
			component.Cell01Text.color = SelectedColor;
			component.Cell02Text.color = SelectedColor;
			component.Cell04Text.color = SelectedColor;
		}
		
		protected override void DefaultColoring(TableRowComponent component)
		{
			//base.DefaultColoring(component);
			component.Cell01Text.color = DefaultColor;
			component.Cell02Text.color = DefaultColor;
			component.Cell04Text.color = DefaultColor;
		}
	}
}