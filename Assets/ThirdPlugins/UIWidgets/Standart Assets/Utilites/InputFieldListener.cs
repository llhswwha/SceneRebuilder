using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

namespace UIWidgets {
	public class MoveEvent : UnityEvent<AxisEventData> {
	}

	public class SubmitEvent : UnityEvent<BaseEventData> {
	}

	public class InputFieldListener : MonoBehaviour, IUpdateSelectedHandler {

		[SerializeField]
		public MoveEvent OnMoveEvent = new MoveEvent();

		[SerializeField]
		public SubmitEvent OnSubmitEvent = new SubmitEvent();

		public virtual void OnUpdateSelected(BaseEventData eventData)
		{
			if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				var axisEvent = new AxisEventData(EventSystem.current);
				axisEvent.moveDir = MoveDirection.Left;
				OnMoveEvent.Invoke(axisEvent);
				return ;
			}
			if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				var axisEvent = new AxisEventData(EventSystem.current);
				axisEvent.moveDir = MoveDirection.Right;
				OnMoveEvent.Invoke(axisEvent);
				return ;
			}
			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				var axisEvent = new AxisEventData(EventSystem.current);
				axisEvent.moveDir = MoveDirection.Up;
				OnMoveEvent.Invoke(axisEvent);
				return ;
			}
			if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				var axisEvent = new AxisEventData(EventSystem.current);
				axisEvent.moveDir = MoveDirection.Down;
				OnMoveEvent.Invoke(axisEvent);
				return ;
			}
			//if (Input.GetKeyDown(KeyCode.Tab))
			if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
			{
				OnSubmitEvent.Invoke(eventData);
				return ;
			}
		}
	}
}