/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using System;
using System.Text;
using Glippy.Core;
using Gtk;

namespace Glippy.Application
{
	/// <summary>
	/// Edit content window.
	/// </summary>
	internal partial class EditContentWindow : Window
	{
		/// <summary>
		/// User interface.
		/// </summary>
		private Ui ui;
		
		/// <summary>
		/// Initializes a new instance class.
		/// </summary>
		/// <param name="ui">User interface.</param>
		public EditContentWindow(Ui ui) : base(WindowType.Toplevel)
		{
			this.Build();
			this.ui = ui;
			
			try
			{
				this.Icon = IconTheme.Default.LoadIcon("glippy", 128, Gtk.IconLookupFlags.GenericFallback);
			}
			catch (Exception ex)
			{
				Tools.PrintInfo(ex, this.GetType());
			}
						
			if (Core.Clipboard.Instance.Items.Count > 0)
			{
				Core.Item item = Core.Clipboard.Instance.Items[0];
				
				if (item.IsData)				
					this.content.Buffer.Text = item.Target.Name == Core.Targets.Html ? Encoding.Unicode.GetString(item.Data) : Encoding.Unicode.GetString(item.DataText);
				else
					this.content.Buffer.Text = item.Text;				
			}			
			
			this.Destroyed += (s, e) => this.Purge();
		}		
		
		/// <summary>
		/// Purges window.
		/// </summary>
		private void Purge()
		{	
			this.ui.EditContentWindow = null;
			this.Dispose();
		}
		
		/// <summary>
		/// Handles button apply clicked event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnButtonApplyClicked(object sender, EventArgs args)
		{
			if (this.content.Buffer.Text.Length == 0)
				return;
			
			Core.Item item = new Core.Item(this.content.Buffer.Text);			
			Core.Clipboard.Instance.SetAsContent(item);			
			this.OnButtonCancelClicked(sender, args);
		}
				
		/// <summary>
		/// Handles button cancel clicked event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnButtonCancelClicked(object sender, System.EventArgs args)
		{
			this.Destroy();
		}			
		
		/// <summary>
		/// Closes window on Escape press.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnKeyPressEvent(object sender, KeyPressEventArgs args)
		{
			if (args.Event.Key == Gdk.Key.Escape)
				this.Destroy();
		}
	}
}
