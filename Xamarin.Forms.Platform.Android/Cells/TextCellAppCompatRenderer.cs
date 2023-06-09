﻿using System.ComponentModel;
using Android.Content;
using Android.Views;
using Xamarin.Forms.Internals;
using AView = Android.Views.View;

namespace Xamarin.Forms.Platform.Android
{
	public class TextCellAppCompatRenderer : CellRenderer
	{
		internal TextCellAppCompatView View { get; private set; }

		protected override AView GetCellCore(Cell item, AView convertView, ViewGroup parent, Context context)
		{
			if ((View = convertView as TextCellAppCompatView) == null)
				View = new TextCellAppCompatView(context, item);

			UpdateMainText();
			UpdateDetailText();
			UpdateHeight();
			UpdateIsEnabled();
			UpdateFlowDirection();
			UpdateAutomationId();
			View.SetImageVisible(false);

			return View;
		}

		protected override void OnCellPropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			if (View.IsDisposed())
			{
				return;
			}

			if (args.PropertyName == TextCell.TextProperty.PropertyName || args.PropertyName == TextCell.TextColorProperty.PropertyName)
				UpdateMainText();
			else if (args.PropertyName == TextCell.DetailProperty.PropertyName || args.PropertyName == TextCell.DetailColorProperty.PropertyName)
				UpdateDetailText();
			else if (args.PropertyName == Cell.IsEnabledProperty.PropertyName)
				UpdateIsEnabled();
			else if (args.PropertyName == "RenderHeight")
				UpdateHeight();
			else if (args.PropertyName == VisualElement.FlowDirectionProperty.PropertyName)
				UpdateFlowDirection();
			else if (args.PropertyName == VisualElement.AutomationIdProperty.PropertyName)
				UpdateAutomationId();
		}

		void UpdateAutomationId()
		{
			View.ContentDescription = Cell.AutomationId;
		}

		void UpdateDetailText()
		{
			var cell = (TextCell)Cell;
			View.DetailText = cell.Detail;
			View.SetDetailTextColor(cell.DetailColor);
		}

		void UpdateHeight()
		{
			View.SetRenderHeight(Cell.RenderHeight);
		}

		void UpdateIsEnabled()
		{
			var cell = (TextCell)Cell;
			View.SetIsEnabled(cell.IsEnabled);
		}

		void UpdateFlowDirection()
		{
			View.UpdateFlowDirection(ParentView);
		}

		void UpdateMainText()
		{
			var cell = (TextCell)Cell;
			View.MainText = cell.Text;

			if (!cell.GetIsGroupHeader<ItemsView<Cell>, Cell>())
				View.SetDefaultMainTextColor(Color.Accent);
			else
				View.SetDefaultMainTextColor(Color.Default);

			View.SetMainTextColor(cell.TextColor);
		}

		// ensure we don't get other people's BaseCellView's
		internal class TextCellAppCompatView : BaseCellAppCompatView
		{
			public TextCellAppCompatView(Context context, Cell cell) : base(context, cell)
			{
			}
		}
	}
}