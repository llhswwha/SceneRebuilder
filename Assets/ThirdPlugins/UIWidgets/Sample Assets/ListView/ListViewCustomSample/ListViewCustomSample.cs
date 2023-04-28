using System;
using UIWidgets;

namespace UIWidgetsSamples {

	public class ListViewCustomSample : ListViewCustom<ListViewCustomSampleComponent,ListViewCustomSampleItemDescription> {
		bool isStartedListViewCustomSample = false;

		Comparison<ListViewCustomSampleItemDescription> itemsComparison = (x, y) => {
			return x.Name.CompareTo(y.Name);
		};

		protected override void Awake()
		{
			Start();
		}

		public override void Start()
		{
			if (isStartedListViewCustomSample)
			{
				return ;
			}
			isStartedListViewCustomSample = true;

			base.Start();
			DataSource.Comparison = itemsComparison;
		}

		protected override void SetData(ListViewCustomSampleComponent component, ListViewCustomSampleItemDescription item)
		{
			component.SetData(item);
		}

		protected override void HighlightColoring(ListViewCustomSampleComponent component)
		{
			base.HighlightColoring(component);
			component.Text.color = HighlightedColor;
		}

		protected override void SelectColoring(ListViewCustomSampleComponent component)
		{
			base.SelectColoring(component);
			component.Text.color = SelectedColor;
		}

		protected override void DefaultColoring(ListViewCustomSampleComponent component)
		{
			base.DefaultColoring(component);
			component.Text.color = DefaultColor;
		}
	}
}