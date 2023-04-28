﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace UIWidgets {
	/// <summary>
	/// AutocompleteFilter.
	/// Startswith - value should beginnig with Input.
	/// Contains - Input occurs with value.
	/// </summary>
	public enum AutocompleteFilter {
		Startswith = 0,
		Contains = 1,
	}

	/// <summary>
	/// AutocompleteInput.
	/// Word - Use current word in input
	/// AllInput - use entire input.
	/// </summary>
	public enum AutocompleteInput {
		Word = 0,
		AllInput = 1,
	}

	/// <summary>
	/// AutocompleteResult.
	/// Append - append value to input.
	/// Result - replace input.
	/// </summary>
	public enum AutocompleteResult {
		Append = 0,
		Replace = 1,
	}

	/// <summary>
	/// Autocomplete.
	/// Allow quickly find and select from a list of values as user type.
	/// DisplayListView - used to display list of values.
	/// TargetListView - if specified selected value will be added to this list.
	/// DataSource - list of values.
	/// </summary>
	public abstract class AutocompleteCustom<TValue,TListViewComponent,TListView> : MonoBehaviour
		where TListView : ListViewCustom<TListViewComponent,TValue>
		where TListViewComponent : ListViewItem
	{
		/// <summary>
		/// InputField for autocomplete.
		/// </summary>
		[SerializeField]
		protected InputField InputField;

		IInputFieldProxy inputFieldProxy;

		/// <summary>
		/// Gets the InputFieldProxy.
		/// </summary>
		protected virtual IInputFieldProxy InputFieldProxy {
			get {
				if (inputFieldProxy==null)
				{
					inputFieldProxy = new InputFieldProxy(InputField);
				}
				return inputFieldProxy;
			}
		}

		/// <summary>
		/// ListView to display available values.
		/// </summary>
		[SerializeField]
		public TListView TargetListView;

		/// <summary>
		/// Selected value will be added to this ListView.
		/// </summary>
		[SerializeField]
		public TListView DisplayListView;

		/// <summary>
		/// List of values.
		/// </summary>
		[SerializeField]
		public List<TValue> DataSource;

		/// <summary>
		/// The filter.
		/// </summary>
		[SerializeField]
		protected AutocompleteFilter filter;

		/// <summary>
		/// Gets or sets the filter.
		/// </summary>
		/// <value>The filter.</value>
		public AutocompleteFilter Filter {
			get {
				return filter;
			}
			set {
				filter = value;
				CustomFilter = null;
			}
		}

		/// <summary>
		/// Is filter case sensitive?
		/// </summary>
		[SerializeField]
		public bool CaseSensitive;

		/// <summary>
		/// The delimiter chars to find word for autocomplete if InputType==Word.
		/// </summary>
		[SerializeField]
		public char[] DelimiterChars = new char[] {' '};

		/// <summary>
		/// Custom filter.
		/// </summary>
		public Func<string,ObservableList<TValue>> CustomFilter;

		/// <summary>
		/// Use entire input or current word in input.
		/// </summary>
		[SerializeField]
		protected AutocompleteInput InputType = AutocompleteInput.Word;

		/// <summary>
		/// Append value to input or replace input.
		/// </summary>
		[SerializeField]
		protected AutocompleteResult Result = AutocompleteResult.Append;

		/// <summary>
		/// OnOptionSelected event.
		/// </summary>
		public UnityEvent OnOptionSelected = new UnityEvent();

		/// <summary>
		/// Current word in input or whole input for autocomplete.
		/// </summary>
		[HideInInspector]
		protected string Input = string.Empty;

		/// <summary>
		/// InputField.caretPosition. Used to keep caretPosition with Up and Down actions.
		/// </summary>
		protected int CaretPosition;

		/// <summary>
		/// Determines whether the beginnig of value matches the Input.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>true if beginnig of value matches the Input; otherwise, false.</returns>
		public abstract bool Startswith(TValue value);

		/// <summary>
		/// Returns a value indicating whether Input occurs within specified value.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>true if the Input occurs within value parameter; otherwise, false.</returns>
		public abstract bool Contains(TValue value);

		/// <summary>
		/// Convert value to string.
		/// </summary>
		/// <returns>The string value.</returns>
		/// <param name="value">Value.</param>
		protected abstract string GetStringValue(TValue value);

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			InputFieldProxy.onValueChanged.AddListener(ApplyFilter);
			InputFieldProxy.onEndEdit.AddListener(HideOptions);

			var inputListener = InputField.GetComponent<InputFieldListener>();
			if (inputListener==null)
			{
				inputListener = InputField.gameObject.AddComponent<InputFieldListener>();
			}
			inputListener.OnMoveEvent.AddListener(SelectResult);
			inputListener.OnSubmitEvent.AddListener(SubmitResult);

			DisplayListView.gameObject.SetActive(false);
		}

		/// <summary>
		/// Canvas will be used as parent for DisplayListView.
		/// </summary>
		protected Transform CanvasTransform;
		
		/// <summary>
		/// To keep DisplayListView position if InputField inside scrollable area.
		/// </summary>
		protected Vector2 DisplayListViewAnchoredPosition;
		
		/// <summary>
		/// Default parent for DisplayListView.
		/// </summary>
		protected Transform DisplayListViewParent;

		/// <summary>
		/// Closes the options.
		/// </summary>
		/// <param name="input">Input.</param>
		protected virtual void HideOptions(string input)
		{
			HideOptions();
		}

		/// <summary>
		/// Closes the options.
		/// </summary>
		protected virtual void HideOptions()
		{
			if (CanvasTransform!=null)
			{
				DisplayListView.transform.SetParent(DisplayListViewParent);
				(DisplayListView.transform as RectTransform).anchoredPosition = DisplayListViewAnchoredPosition;
			}
			
			DisplayListView.gameObject.SetActive(false);
		}

		/// <summary>
		/// Shows the options.
		/// </summary>
		protected virtual void ShowOptions()
		{
			CanvasTransform = Utilites.FindTopmostCanvas(DisplayListView.transform);
			if (CanvasTransform!=null)
			{
				DisplayListViewAnchoredPosition = (DisplayListView.transform as RectTransform).anchoredPosition;
				DisplayListViewParent = DisplayListView.transform.parent;
				DisplayListView.transform.SetParent(CanvasTransform);
			}

			DisplayListView.gameObject.SetActive(true);
		}

		/// <summary>
		/// Gets the results.
		/// </summary>
		/// <returns>Values matches filter.</returns>
		protected virtual ObservableList<TValue> GetResults()
		{
			if (CustomFilter!=null)
			{
				return CustomFilter(Input);
			}
			else
			{
				if (Filter==AutocompleteFilter.Startswith)
				{
					return DataSource.Where(Startswith).ToObservableList();
				}
				else
				{
					return DataSource.Where(Contains).ToObservableList();
				}
			}
		}

		/// <summary>
		/// Sets the input.
		/// </summary>
		protected virtual void SetInput()
		{
			if (InputType==AutocompleteInput.AllInput)
			{
				Input = InputFieldProxy.text;
			}
			else
			{
				int end_position = InputFieldProxy.caretPosition;

				var text = InputFieldProxy.text.Substring(0, end_position);
				var start_position = text.LastIndexOfAny(DelimiterChars) + 1;

				Input = text.Substring(start_position).Trim();
			}
		}

		/// <summary>
		/// Applies the filter.
		/// </summary>
		/// <param name="input">Input.</param>
		protected virtual void ApplyFilter(string input)
		{
			SetInput();
			if (Input.Length==0)
			{
				HideOptions();
				return ;
			}

			DisplayListView.Start();
			DisplayListView.Multiple = false;

			DisplayListView.DataSource = GetResults();

			if (DisplayListView.DataSource.Count > 0)
			{
				ShowOptions();
				DisplayListView.SelectedIndex = 0;
			}
			else
			{
				HideOptions();
			}
		}

		/// <summary>
		/// Update this instance.
		/// </summary>
		protected virtual void Update()
		{
			CaretPosition = InputFieldProxy.caretPosition;
		}

		/// <summary>
		/// Selects the result.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void SelectResult(AxisEventData eventData)
		{
			if (!DisplayListView.gameObject.activeInHierarchy)
			{
				return ;
			}

			if (DisplayListView.DataSource.Count==0)
			{
				return ;
			}

			switch (eventData.moveDir)
			{
				case MoveDirection.Up:
					if (DisplayListView.SelectedIndex==0)
					{
						DisplayListView.SelectedIndex = DisplayListView.DataSource.Count - 1;
					}
					else
					{
						DisplayListView.SelectedIndex -= 1;
					}
					DisplayListView.ScrollTo(DisplayListView.SelectedIndex);
					InputFieldProxy.caretPosition = CaretPosition;
					break;
				case MoveDirection.Down:
					if (DisplayListView.SelectedIndex==(DisplayListView.DataSource.Count - 1))
					{
						DisplayListView.SelectedIndex = 0;
					}
					else
					{
						DisplayListView.SelectedIndex += 1;
					}
					DisplayListView.ScrollTo(DisplayListView.SelectedIndex);
					InputFieldProxy.caretPosition = CaretPosition;
					break;
				default:
					var oldInput = Input;
					SetInput();
					if (oldInput!=Input)
					{
						ApplyFilter("");
					}
					break;
			}
		}

		/// <summary>
		/// Submits the result.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void SubmitResult(BaseEventData eventData)
		{
			if (DisplayListView.SelectedIndex==-1)
			{
				return ;
			}

			if ((TargetListView!=null) && (DisplayListView.SelectedIndex!=-1))
			{
				TargetListView.Set(DisplayListView.DataSource[DisplayListView.SelectedIndex]);
			}

			int end_position = (DisplayListView.gameObject.activeInHierarchy) ? InputFieldProxy.caretPosition : CaretPosition;

			var text = InputFieldProxy.text.Substring(0, end_position);
			var start_position = text.LastIndexOfAny(DelimiterChars) + 1;

			var value = GetStringValue(DisplayListView.DataSource[DisplayListView.SelectedIndex]);
			InputFieldProxy.text = InputFieldProxy.text.Substring(0, start_position) + value + InputFieldProxy.text.Substring(end_position);


			#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0
			//InputField.gameObject.SetActive(false);
			//InputField.gameObject.SetActive(true);
			InputField.ActivateInputField();
			#else
			InputFieldProxy.caretPosition = start_position + value.Length;
			#endif

			OnOptionSelected.Invoke();
			
			HideOptions();
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (InputField!=null)
			{
				InputFieldProxy.onValueChanged.RemoveListener(ApplyFilter);
				InputFieldProxy.onEndEdit.RemoveListener(HideOptions);

				var inputListener = InputField.GetComponent<InputFieldListener>();
				if (inputListener!=null)
				{
					inputListener.OnMoveEvent.RemoveListener(SelectResult);
					inputListener.OnSubmitEvent.RemoveListener(SubmitResult);
				}
			}
		}
	}
}