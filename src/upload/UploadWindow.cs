/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Glippy.Core.Extensions;
using Gtk;
using Mono.Unix;

namespace Glippy.Upload
{
	/// <summary>
	/// Upload window.
	/// </summary>
	internal partial class UploadWindow : Gtk.Window
	{
		/// <summary>
		/// Max image size.
		/// </summary>
		private const int MaxImageSize = 500;
		
		/// <summary>
		/// Image to upload.
		/// </summary>
		private Gdk.Pixbuf image;
		
		/// <summary>
		/// Upload text thread.
		/// </summary>
		private Thread uploadTextThread;
		
		/// <summary>
		/// Upload image thread.
		/// </summary>
		private Thread uploadImageThread;
		
		/// <summary>
		/// Progress bar thread.
		/// </summary>
		private Thread progressBarThread;
			
		/// <summary>
		/// Plugin.
		/// </summary>
		private Upload plugin;
		
		/// <summary>
		/// Initializes a new instance of class.
		/// </summary>
		/// <param name="plugin">Plugin.</param>
		public UploadWindow(Upload plugin) : base(WindowType.Toplevel)
		{
			this.Build();
			this.plugin = plugin;
			
			try
			{
				this.Icon = IconTheme.Default.LoadIcon("glippy", 128, IconLookupFlags.GenericFallback);
			}
			catch (Exception ex)
			{
				Core.Tools.PrintInfo(ex, this.GetType());
			}
			
			if (!string.IsNullOrWhiteSpace(Core.Settings.Instance[SettingsKeys.PastebinUserKey].AsString()))
		    	this.useAccount.Sensitive = this.useAccount.Active = true;				
			
			this.Destroyed += (s, e) => this.Purge();
			
			Core.Item item = Core.Clipboard.Instance.Items.FirstOrDefault();
				
			if (item == null)
			{
				radiobuttonImage.Sensitive = false;
			}
			else if (item.IsImage)
			{							
				int width, height;
				
				if (item.Image.Width < item.Image.Height)
				{
					if (item.Image.Height > MaxImageSize)
					{
						height = MaxImageSize;
						width = MaxImageSize * item.Image.Width / item.Image.Height;
					}
					else
					{
						height = item.Image.Height;
						width = item.Image.Width;
					}
				}
				else
				{
					if (item.Image.Width > MaxImageSize)
					{
						width = MaxImageSize;
						height = MaxImageSize * item.Image.Height / item.Image.Width;
					}
					else
					{					
						width = item.Image.Width;
						height = item.Image.Height;
					}
				}
				
				this.imageClip.Pixbuf = item.Image.ScaleSimple(width, height, Gdk.InterpType.Bilinear);
				this.image = new Gdk.Pixbuf(item.Image, 0, 0, item.Image.Width, item.Image.Height);
																
				this.radiobuttonImage.Active = true;
				this.SetItemsSensitivity(false);
				this.clipType.Page = 1;
			}
			else
			{
				TextBuffer buffer = new TextBuffer(null);
				
				if (item.IsData)
					buffer.Text = item.Target == Core.Targets.Html ? Encoding.UTF8.GetString(item.Data) : Encoding.UTF8.GetString(item.DataText);
				else						
					buffer.Text = item.Text;
				
				this.textClip.Buffer = buffer;	
				this.radiobuttonImage.Sensitive = false;				
			}								
		}
		
		/// <summary>
		/// Cleans window.
		/// </summary>
		private void Purge()
		{
			this.plugin.UploadWindow = null;
			
			if (this.Icon != null)
				this.Icon.Dispose();
			
			if (this.imageClip.Pixbuf != null)
				this.imageClip.Pixbuf = null;
			
			if (this.image != null)
				this.image.Dispose();
			
			this.AbortUpload();	
			this.Dispose();
		}
		
		/// <summary>
		/// Handles button cancel clicked event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnButtonCancelClicked(object sender, EventArgs args)
		{
			if (!this.buttonUpload.Sensitive)
			{
				this.AbortUpload();
				this.progressbarUpload.Fraction = 0;
				this.progressbarUpload.Text = string.Empty;
				this.buttonUpload.Sensitive = true;
			}
			else
			{
				this.Destroy();
			}					
		}
		
		/// <summary>
		/// Handles button upload clicked event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnButtonUploadClicked(object sender, EventArgs args)
		{
			this.AbortUpload();
			this.progressbarUpload.Visible = true;
			
			this.progressBarThread = new Thread(() =>
			{
				this.progressbarUpload.Text = Catalog.GetString("Uploading...");
				
				for (;;)
				{
					Thread.Sleep(50);
					Application.Invoke((s, e) => this.progressbarUpload.Pulse());					
				}				
			});			
			
			if (this.radiobuttonText.Active)
			{				
				if (this.textClip.Buffer.Text.Length == 0)
				{
					this.ShowMessage(Catalog.GetString("Text field is empty."), MessageType.Error);
					return;
				}		
				
				string key = null;
					
				if (this.useAccount.Active)
				{
					key = Core.Settings.Instance[SettingsKeys.PastebinUserKey].AsString();
					
					if (string.IsNullOrWhiteSpace(key))
					{
						this.useAccount.Sensitive = this.useAccount.Active = false;
						return;
					}
				}
					
				this.uploadTextThread = new Thread(() =>
				{								
					string expiration_date;
					
					switch (this.expirationDate.Active)
					{
						case 1:
							expiration_date = Uploaders.Pastebin.Parameters.ExpireDate1Month;
							break;
						
						case 2:
							expiration_date = Uploaders.Pastebin.Parameters.ExpireDate1Day;
							break;
						
						case 3:
							expiration_date = Uploaders.Pastebin.Parameters.ExpireDate1Hour;
							break;
						
						case 4:
							expiration_date = Uploaders.Pastebin.Parameters.ExpireDate10Minutes;
							break;		
						
						default:
							expiration_date = Uploaders.Pastebin.Parameters.ExpireDateNever;
							break;
					}
					
					string result = Uploaders.Pastebin.Upload(textClip.Buffer.Text, name.Text, privacy.Active, expiration_date, key);				
								
					Application.Invoke((s, e) =>
					{						
						this.FinishProgressBar();
						
						if (result.StartsWith(Catalog.GetString("Error: ")))						
							this.ShowMessage(result, MessageType.Error);
						else
							this.ShowMessage(string.Format("{0}:\n\n{1}", Catalog.GetString("Upload finished"), result), MessageType.Info);
						
						this.Destroy();
					});							
				});		
			
				this.progressBarThread.Start();
				this.uploadTextThread.Start();				
				this.buttonUpload.Sensitive = false;
			}
			else if (radiobuttonImage.Active)
			{				
				if (image == null)
				{
					this.ShowMessage(Catalog.GetString("Image is empty."), MessageType.Error);
					return;
				}
							
				this.uploadImageThread = new Thread(() =>
				{				
					Dictionary<string, string> result = Uploaders.Imgur.Upload(this.image);					
					StringBuilder result_text = new StringBuilder();
					
					if (result.ContainsKey("FATAL"))
					{
						result_text.Append(result["FATAL"]);
					}
					else
					{
						foreach (KeyValuePair<string, string> entry in result)
						{
							result_text.Append(entry.Key).Append(": ").Append(entry.Value).Append(Environment.NewLine);
						}							
					}
					
					Application.Invoke((s, e) =>
					{
						this.FinishProgressBar();
						this.ShowMessage(string.Format("{0}:\n\n{1}", Catalog.GetString("Upload finished"), result_text), MessageType.Info);
						this.Destroy();
					});
				});					
								
				this.progressBarThread.Start();
				this.uploadImageThread.Start();		
				this.buttonUpload.Sensitive = false;
			}					
		}
		
		/// <summary>
		/// Handles key press event event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnKeyPressEvent(object sender, KeyPressEventArgs args)
		{
			if (args.Event.Key == Gdk.Key.Escape)
				this.Destroy();
		}						
		
		/// <summary>
		/// Handles radiobutton image toggled event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnRadiobuttonImageToggled(object sender, EventArgs args)
		{
			this.clipType.CurrentPage = 1;
			this.SetItemsSensitivity(false);
		}
		
		/// <summary>
		/// Handles radiobutton text toggled event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnRadiobuttonTextToggled(object sender, EventArgs args)
		{
			this.clipType.CurrentPage = 0;
			this.SetItemsSensitivity(true);
		}
		
		/// <summary>
		/// Updates privacy combobox.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event arguments.</param>
		private void OnUseAccountToggled(object sender, EventArgs e)
		{
			if (this.radiobuttonText.Active && this.useAccount.Active)
			{
				this.privacy.AppendText(Catalog.GetString("Private"));
			}
			else
			{
				if (this.privacy.Active == 2)
					this.privacy.Active = 1;
					
				this.privacy.RemoveText(2);
			}
		}
				
		/// <summary>
		/// Sets widgets sensitivity.
		/// </summary>
		/// <param name="val">Value to set.</param>
		private void SetItemsSensitivity(bool val)
		{
			foreach (Widget w in vboxTextLabels.Children)
				w.Sensitive = val;
			
			foreach (Widget w in vboxTextEntries.Children)
				w.Sensitive = val;					
			
			foreach (Widget w in vboxTextEntries1.Children)
				w.Sensitive = val;	
			
			bool account_available = !string.IsNullOrWhiteSpace(Core.Settings.Instance[SettingsKeys.PastebinUserKey].AsString());			
			
			if (!account_available)
				this.useAccount.Active = false;
			
			this.useAccount.Sensitive = this.radiobuttonText.Active && account_available;					
		}
		
		/// <summary>
		/// Aborts upload.
		/// </summary>
		private void AbortUpload()
		{			
			if (this.uploadTextThread != null && this.uploadTextThread.IsAlive)
				this.uploadTextThread.Abort();			
			
			if (this.uploadImageThread != null && this.uploadImageThread.IsAlive)
				this.uploadImageThread.Abort();
			
			if (this.progressBarThread != null && this.progressBarThread.IsAlive)
				this.progressBarThread.Abort();			
		}
		
		/// <summary>
		/// Finishes progress bar.
		/// </summary>
		private void FinishProgressBar()
		{
			this.progressBarThread.Abort();
			this.progressbarUpload.Fraction = 1;
			this.progressbarUpload.Text = string.Empty;
		}
		
		/// <summary>
		/// Shows the message.
		/// </summary>
		/// <param name="msg">Message.</param>
		/// <param name="type">Type.</param>
		private void ShowMessage(string msg, MessageType type)
		{
			this.AbortUpload();
			MessageDialog dialog = new MessageDialog(this, DialogFlags.Modal, type, ButtonsType.Ok, false, msg);
			dialog.UseMarkup = true;
			dialog.Run();					
			dialog.Destroy();
			dialog.Dispose();		
			
			if (type == MessageType.Info)
			{
				this.progressbarUpload.Text = string.Empty;
				this.progressbarUpload.Fraction = 0;
			}
		}					
	}
}
