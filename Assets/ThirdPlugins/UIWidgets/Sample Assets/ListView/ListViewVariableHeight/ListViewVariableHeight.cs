using UnityEngine;
using UIWidgets;

namespace UIWidgetsSamples {
	
	public class ListViewVariableHeight : ListViewCustomHeight<ListViewVariableHeightComponent,ListViewVariableHeightItemDescription> {
		protected override void SetData(ListViewVariableHeightComponent component, ListViewVariableHeightItemDescription item)
		{
			component.SetData(item);
		}
		
		protected override void HighlightColoring(ListViewVariableHeightComponent component)
		{
			base.HighlightColoring(component);
			component.Name.color = HighlightedColor;
			component.Text.color = HighlightedColor;
		}
		
		protected override void SelectColoring(ListViewVariableHeightComponent component)
		{
			base.SelectColoring(component);
			component.Name.color = SelectedColor;
			component.Text.color = SelectedColor;
		}
		
		protected override void DefaultColoring(ListViewVariableHeightComponent component)
		{
			base.DefaultColoring(component);
			component.Name.color = DefaultColor;
			component.Text.color = DefaultColor;
		}
	}
}