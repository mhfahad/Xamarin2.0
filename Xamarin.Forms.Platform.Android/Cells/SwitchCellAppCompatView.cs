﻿using Android.Content;
using Android.Widget;

namespace Xamarin.Forms.Platform.Android
{
	public class SwitchCellAppCompatView : BaseCellAppCompatView, CompoundButton.IOnCheckedChangeListener
	{
		public SwitchCellAppCompatView(Context context, Cell cell) : base(context, cell)
		{
			var sw = new global::Android.Widget.Switch(context);
			sw.SetOnCheckedChangeListener(this);

			SetAccessoryView(sw);

			SetImageVisible(false);
		}

		public SwitchCell Cell { get; set; }

		public void OnCheckedChanged(CompoundButton buttonView, bool isChecked)
		{
			Cell.On = isChecked;
		}
	}
}