using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UIWidgets;

namespace UIWidgetsSamples.Shops {
	/// <summary>
	/// Trader list view component.
	/// </summary>
	public class TraderListViewComponent : ListViewItem {
		/// <summary>
		/// The name.
		/// </summary>
		[SerializeField]
		public Text Name;

		/// <summary>
		/// The price.
		/// </summary>
		[SerializeField]
		public Text Price;

		/// <summary>
		/// The available count.
		/// </summary>
		[SerializeField]
		public Text AvailableCount;

		[SerializeField]
		Spinner Count;

		JRPGOrderLine OrderLine;

		protected override void Start()
		{
			Count.onValueChangeInt.AddListener(ChangeCount);
			base.Start();
		}

		/// <summary>
		/// Change count on left and right movements.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public override void OnMove(AxisEventData eventData)
		{
			switch (eventData.moveDir)
			{
				case MoveDirection.Left:
					Count.Value -= 1;
					break;
				case MoveDirection.Right:
					Count.Value += 1;
					break;
				default:
					base.OnMove(eventData);
					break;
			}

		}

		/// <summary>
		/// Sets the data.
		/// </summary>
		/// <param name="orderLine">Order line.</param>
		public void SetData(JRPGOrderLine orderLine)
		{
			OrderLine = orderLine;

			Name.text = OrderLine.Item.Name;
			Price.text = OrderLine.Price.ToString();
			AvailableCount.text = (OrderLine.Item.Count==-1) ? "∞" : OrderLine.Item.Count.ToString();

			Count.Min = 0;
			Count.Max = (OrderLine.Item.Count==-1) ? 9999 : OrderLine.Item.Count;
			Count.Value = OrderLine.Count;
		}

		void ChangeCount(int count)
		{
			OrderLine.Count = count;
		}

		protected override void OnDestroy()
		{
			if (Count!=null)
			{
				Count.onValueChangeInt.RemoveListener(ChangeCount);
			}
		}
	}
}