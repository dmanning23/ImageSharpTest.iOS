using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using ImageSharpTest.Models;
using ImageSharpTest.Views;
using System.IO;
using System.Linq;

using Image = SixLabors.ImageSharp.Image;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace ImageSharpTest.ViewModels
{
	public class ItemsViewModel : BaseViewModel
	{
		public ObservableCollection<Item> Items { get; set; }
		public Command LoadItemsCommand { get; set; }

		public ItemsViewModel()
		{
			Title = "Browse";
			Items = new ObservableCollection<Item>();
			LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand().ConfigureAwait(false));

			MessagingCenter.Subscribe<NewItemPage, Item>(this, "AddItem", async (obj, item) =>
			{
				var newItem = item as Item;
				Items.Add(newItem);
				await DataStore.AddItemAsync(newItem);
			});
		}

		async Task ExecuteLoadItemsCommand()
		{
			try
			{
				Items.Clear();

				await RunThisOnYourDevice().ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		public Task RunThisOnYourDevice()
		{
			string inputImagesRoot = "Content"; // https://github.com/SixLabors/ImageSharp/tree/master/tests/Images/Input
												//string reportFile = Path.Combine("WhateverPathYouCanWriteTo", "Report.txt");

			string[] files = Directory.EnumerateFiles(
					inputImagesRoot,
					"*.*",
					SearchOption.AllDirectories).Where(
					f => Path.GetExtension(f).ToLower() == ".png"
					|| Path.GetExtension(f).ToLower() == ".jpg"
					|| Path.GetExtension(f).ToLower() == ".jpeg"
					|| Path.GetExtension(f).ToLower() == ".bmp"
					|| Path.GetExtension(f).ToLower() == ".gif"
					)
				.ToArray();

			foreach (var file in files)
			{
				var item = new Item { Id = file };

				Console.WriteLine($"Testing {file}");

				try
				{
					using (var img = Image.Load(file, out IImageFormat format))
					{
					}

					item.Text = "   ... SUCCESS!";
				}
				catch (NotSupportedException)
				{
					item.Text = $"  ... NotSupportedException (invalid input)";
				}
				catch (ImageFormatException)
				{
					item.Text = $"  ... ImageFormatException (invalid input)";
				}
				catch (Exception ex)
				{
					item.Text = $"!!! FAILED: {ex.GetType().Name} : {ex.Message}";
					item.Description = ex.StackTrace;
				}

				Console.WriteLine(item.Text);

				Items.Add(item);
			}

			return Task.CompletedTask;
		}
	}
}