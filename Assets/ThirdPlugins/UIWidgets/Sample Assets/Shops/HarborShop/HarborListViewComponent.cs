using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UIWidgets;

namespace UIWidgetsSamples.Shops {
	/// <summary>
	/// HarborListViewComponent.
	/// </summary>
	public class HarborListViewComponent : ListViewItem {
		[SerializeField]
		public Text Name;

		[SerializeField]
		public Text SellPrice;

		[SerializeField]
		public Text BuyPrice;
		
		[SerializeField]
		public Text AvailableBuyCount;

		[SerializeField]
		public Text AvailableSellCount;

		[SerializeField]
		CenteredSlider Count;
		
		public HarborOrderLine OrderLine;

		protected override void Start()
		{
			Count.OnValuesChange.AddListener(ChangeCount);
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
		public void SetData(HarborOrderLine orderLine)
		{
			OrderLine = orderLine;
			
			Name.text = OrderLine.Item.Name;

			BuyPrice.text = OrderLine.BuyPrice.ToString();
			SellPrice.text = OrderLine.SellPrice.ToString();

			AvailableBuyCount.text = OrderLine.BuyCount.ToString();
			AvailableSellCount.text = OrderLine.SellCount.ToString();

			Count.LimitMin = -OrderLine.SellCount;
			Count.LimitMax = OrderLine.BuyCount;
			Count.Value = OrderLine.Count;
		}
		
		void ChangeCount(int value)
		{
			OrderLine.Count = value;
		}

		protected override void OnDestroy()
		{
			if (Count!=null)
			{
				Count.OnValuesChange.RemoveListener(ChangeCount);
			}
		}
	}
}