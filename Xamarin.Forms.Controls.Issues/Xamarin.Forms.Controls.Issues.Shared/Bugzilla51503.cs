﻿using System;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[NUnit.Framework.Category(Core.UITests.UITestCategories.Bugzilla)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 51503, "NullReferenceException on VisualElement Finalize", PlatformAffected.All)]
	public class Bugzilla51503 : TestNavigationPage
	{
		protected override void Init()
		{
			PushAsync(new _51503RootPage());
		}

		[Preserve(AllMembers = true)]
		class _51503RootPage : ContentPage
		{
			public _51503RootPage()
			{
				Button button = new Button
				{
					AutomationId = "Button",
					Text = "Open"
				};

				button.Clicked += Button_Clicked;

				Content = button;
			}

			async void Button_Clicked(object sender, EventArgs e)
			{
				GarbageCollectionHelper.Collect();

				await Navigation.PushAsync(new ChildPage());
			}
		}

		[Preserve(AllMembers = true)]
		class ChildPage : ContentPage
		{
			public ChildPage()
			{
				Content = new Label
				{
					AutomationId = "VisualElement",
					Text = "Navigate 3 times to this page",
					Triggers =
					{
						new EventTrigger()
					}
				};
			}
		}

#if UITEST
		[Test]
		public void Issue51503Test()
		{
			for (int i = 0; i < 3; i++)
			{
				RunningApp.WaitForElement(q => q.Marked("Button"), timeout: TimeSpan.FromSeconds(2), postTimeout: TimeSpan.FromSeconds(1));

				RunningApp.Tap(q => q.Marked("Button"));

				RunningApp.WaitForElement(q => q.Marked("VisualElement"), timeout: TimeSpan.FromSeconds(2), postTimeout: TimeSpan.FromSeconds(1));

				RunningApp.Back();
			}
		}
#endif
	}
}