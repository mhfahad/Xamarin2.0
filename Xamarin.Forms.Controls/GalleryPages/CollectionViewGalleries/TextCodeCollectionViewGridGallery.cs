﻿namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries
{
	internal class TextCodeCollectionViewGridGallery : ContentPage
	{
		ItemsSourceGenerator generator;

		public TextCodeCollectionViewGridGallery(ItemsLayoutOrientation orientation = ItemsLayoutOrientation.Vertical)
		{
			var layout = new Grid
			{
				RowDefinitions = new RowDefinitionCollection
				{
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Star }
				}
			};

			var itemsLayout = new GridItemsLayout(2, orientation);

			var collectionView = new CollectionView { ItemsLayout = itemsLayout, AutomationId = "collectionview" };

			generator = new ItemsSourceGenerator(collectionView);
			var spanSetter = new SpanSetter(collectionView);

			layout.Children.Add(generator);
			layout.Children.Add(spanSetter);
			Grid.SetRow(spanSetter, 1);
			layout.Children.Add(collectionView);
			Grid.SetRow(collectionView, 2);

			Content = layout;

			spanSetter.UpdateSpan();
			generator.GenerateItems();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			generator.SubscribeEvents();
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			generator.UnsubscribeEvents();
		}
	}
}