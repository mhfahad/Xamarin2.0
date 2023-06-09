﻿using System;
using System.Collections.Generic;
using System.Text;
using Android.Views.Accessibility;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.View;
using AndroidX.Core.View.Accessibility;
using Xamarin.Forms.Platform.Android.FastRenderers;

namespace Xamarin.Forms.Platform.Android
{
	class AccessibilityDelegateAutomationId : AccessibilityDelegateCompat

	{
		BindableObject _element;

		public AccessibilityDelegateAutomationId(BindableObject element) : base()
		{
			_element = element;
		}


		public override void OnInitializeAccessibilityNodeInfo(global::Android.Views.View host, AccessibilityNodeInfoCompat info)
		{
			base.OnInitializeAccessibilityNodeInfo(host, info);

			if (_element == null)
				return;

			if (Flags.IsAccessibilityExperimentalSet())
			{
				var value = AutomationPropertiesProvider.ConcatenateNameAndHelpText(_element);
				if (!string.IsNullOrWhiteSpace(value))
				{
					host.ContentDescription = value;
				}
				else if (host.ContentDescription == (_element as Element)?.AutomationId)
				{
					host.ContentDescription = null;
				}

				if (_element is Element element &&
					!string.IsNullOrWhiteSpace(element.AutomationId) &&
					host?.Context != null)
				{
					// This is used by Appium and other automation testing frameworks
					// to locate views
					info.ViewIdResourceName = $"{host.Context.PackageName}:id/{element.AutomationId}";
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			_element = null;
			base.Dispose(disposing);
		}

	}
}
