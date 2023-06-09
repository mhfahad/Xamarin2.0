﻿using System;
using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.Runtime;
using Android.Util;
using Firebase.AppIndexing;
using Actions = Firebase.AppIndexing.Builders.Actions;
using GMSTask = Android.Gms.Tasks.Task;
using IndexingAction = Firebase.AppIndexing.IAction;

namespace Xamarin.Forms.Platform.Android.AppLinks
{
	[Preserve(AllMembers = true)]
	public class AndroidAppLinks : IAppLinks, IDisposable
	{
		bool _disposed;

		public static bool IsInitialized { get; private set; }

		public static Context Context { get; private set; }

		public static void Init(Activity activity)
		{
			if (IsInitialized)
				return;
			IsInitialized = true;

			Context = activity;
		}

		public AndroidAppLinks(Context context)
		{
		}

		public void DeregisterLink(IAppLinkEntry appLink)
		{
			RemoveFromIndexItemAsync(appLink.AppLinkUri.ToString());
		}

		public void DeregisterLink(Uri appLinkUri)
		{
			RemoveFromIndexItemAsync(appLinkUri.ToString());
		}

		public void RegisterLink(IAppLinkEntry appLink)
		{
			IndexItemAsync(appLink);

		}

		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing && !_disposed)
			{
				_disposed = true;
			}
		}

		void IndexItemAsync(IAppLinkEntry appLink)
		{
			//IndexingAction indexAction = BuildIndexAction(appLink);
			var url = global::Android.Net.Uri.Parse(appLink.AppLinkUri.AbsoluteUri).ToString();
			IIndexable indexable = GetIndexable(appLink, url);
			IndexingAction indexAction = GetAction(appLink, url);
			/* If you’re logging an action on an item that has already been added to the index,
			 * you don’t have to add the following update line. See
			 * https://firebase.google.com/docs/app-indexing/android/personal-content#update-the-index for
			 * adding content to the index 
            */
			FirebaseAppIndex.GetInstance(Context).Update(indexable);
			GMSTask gmsTask = FirebaseUserActions.GetInstance(Context)
												 .Start(indexAction)
												 .AddOnSuccessListener(Context.GetActivity(),
																	   new AndroidActionSuccessListener(appLink as AppLinkEntry, indexAction))
												 .AddOnFailureListener(Context.GetActivity(),
																	   new AndroidActionFailureListener(appLink as AppLinkEntry, indexAction));
		}

		void RemoveFromIndexItemAsync(string identifier)
		{
			FirebaseAppIndex.GetInstance(Context).Remove(identifier);
		}

		IIndexable GetIndexable(IAppLinkEntry appLink, string url)
		{
			var indexableBuilder = new IndexableBuilder();
			indexableBuilder.SetName(appLink.Title);
			indexableBuilder.SetUrl(url);
			indexableBuilder.SetDescription(appLink.Description);
			return indexableBuilder.Build();
		}

		IndexingAction GetAction(IAppLinkEntry applink, string url)
		{
			return Actions.NewView(applink.Title, url);
		}

		internal class AndroidActionSuccessListener : Java.Lang.Object, IOnSuccessListener
		{
			readonly AppLinkEntry appLink;
			readonly IndexingAction indexAction;

			public AndroidActionSuccessListener(AppLinkEntry appLink, IndexingAction indexAction)
			{
				this.appLink = appLink;
				this.indexAction = indexAction;
			}

			public void OnSuccess(Java.Lang.Object result)
			{
				if (appLink != null)
				{
					appLink.PropertyChanged += (sender, e) =>
					{
						if (e.PropertyName == AppLinkEntry.IsLinkActiveProperty.PropertyName)
						{
							if (appLink.IsLinkActive)
							{
								FirebaseUserActions.GetInstance(Context).Start(indexAction);
							}
							else
							{
								FirebaseUserActions.GetInstance(Context).End(indexAction);
							}
						}
					};
				}
			}
		}
		internal class AndroidActionFailureListener : Java.Lang.Object, IOnFailureListener
		{
			readonly AppLinkEntry appLink;
			readonly IndexingAction indexAction;

			public AndroidActionFailureListener(AppLinkEntry appLink, IndexingAction indexAction)
			{
				this.appLink = appLink;
				this.indexAction = indexAction;
			}

			public void OnFailure(Java.Lang.Exception e)
			{
				Log.Error(this.Class.Name, e, $" [{DateTime.Now}] - [AndroidAppLinks Failure] - {e.Message}");
				throw e;
			}
		}
	}
}

