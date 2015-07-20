/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using System;
using Glippy.Core;
using Gtk;
using Mono.Unix;

namespace Glippy.Screenshot
{
	/// <summary>
	/// Image window.
	/// </summary>
	internal partial class ImageWindow : Window
	{
		/// <summary>
		/// Max image size.
		/// </summary>
		private const int MaxImageSize = 600;
		
		/// <summary>
		/// Image.
		/// </summary>
		private Gdk.Pixbuf image;
		
		/// <summary>
		/// Initializes a new instance of the ImageWindow class.
		/// </summary>
		/// <param name="image">Image, which is copied for window needs.</param>
		public ImageWindow(Gdk.Pixbuf image) : base(WindowType.Toplevel)
		{
			this.Build();
			
			try
			{
				this.Icon = IconTheme.Default.LoadIcon("glippy", 128, IconLookupFlags.GenericFallback);
			}
			catch (Exception ex)
			{
				Tools.PrintInfo(ex, this.GetType());
			}
			
			this.image = new Gdk.Pixbuf(image, 0, 0, image.Width, image.Height);
			this.imageWidth.Text = this.image.Width.ToString() + "px";
			this.imageHeight.Text = this.image.Height.ToString() + "px";
			this.imageDate.Text = DateTime.Now.ToString();
			this.preview.Pixbuf = GetPreview(image);			
			this.Destroyed += (s, e) => this.Purge();
		}
		
		/// <summary>
		/// Gets preview of image.
		/// </summary>
		/// <param name="image">Original image.</param>
		/// <returns>Preview of original image.</returns>		
		private static Gdk.Pixbuf GetPreview(Gdk.Pixbuf image)
		{
			int width, height;
				
			if (image.Width < image.Height)
			{
				if (image.Height > MaxImageSize)
				{
					height = MaxImageSize;
					width = MaxImageSize * image.Width / image.Height;
				}
				else
				{
					height = image.Height;
					width = image.Width;
				}
			}
			else
			{
				if (image.Width > MaxImageSize)
				{
					width = MaxImageSize;
					height = MaxImageSize * image.Height / image.Width;
				}
				else
				{					
					width = image.Width;
					height = image.Height;
				}
			}
			
			return image.ScaleSimple(width, height, Gdk.InterpType.Bilinear);
		}
		
		/// <summary>
		/// Cleans window.
		/// </summary>
		private void Purge()
		{
			if (this.image != null)
			{
				this.image.Dispose();				
			}
			
			if (this.preview.Pixbuf != null)
			{
				this.preview.Dispose();
			}
			
			this.Dispose();			
		}
		
		/// <summary>
		/// Saves image to file.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnButtonSaveToFileClicked(object sender, EventArgs args)
		{
			using (FileChooserDialog fc = new FileChooserDialog(Catalog.GetString("Save image"), this, FileChooserAction.Save, Catalog.GetString("Cancel"), ResponseType.Cancel, Catalog.GetString("Save"), ResponseType.Accept))
			{
				FileFilter png_filter = new FileFilter();
				png_filter.Name = "PNG (*.png)";
				png_filter.AddPattern("*.png");
				png_filter.AddMimeType("image/png");				
				
				FileFilter jpg_filter = new FileFilter();
				jpg_filter.Name = "JPEG (*.jpg)";
				jpg_filter.AddPattern("*.jpg");
				jpg_filter.AddPattern("image/jpeg");
				
				fc.AddFilter(png_filter);				
				fc.AddFilter(jpg_filter);
				
				if (fc.Run() == (int)ResponseType.Accept)
				{
					try
					{
						string ext = string.Empty;
						
						if (fc.Filter == png_filter)
							ext = ".png";
						else if (fc.Filter == jpg_filter)
							ext = ".jpg";
						
						string file = fc.Filename.ToLower().EndsWith(ext) ? fc.Filename : fc.Filename + ext;						
						this.image.Save(file, ext.Remove(0, 1));
					}
					catch (Exception ex)
					{
						Tools.PrintInfo(ex, this.GetType());
					}					
				}
				
				fc.Destroy();			
			}
		}
		
		/// <summary>
		/// Closes window.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnButtonCloseClicked(object sender, EventArgs args)
		{
			this.Destroy();
		}
		
		/// <summary>
		/// Sets image as clipboard content.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnButtonCopyToClipboardClicked(object sender, EventArgs args)
		{
			Core.Item item = new Core.Item(new Gdk.Pixbuf(this.image, 0, 0, this.image.Width, this.image.Height));
			Core.Clipboard.Instance.SetAsContent(item);
			this.buttonCopyToClipboard.Sensitive = false;
		}
	}
}
