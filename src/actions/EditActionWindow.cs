/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using System;
using Gtk;
using Mono.Unix;

namespace Glippy.Actions
{
	/// <summary>
	/// Edit action window.
	/// </summary>
	internal partial class EditActionWindow : Window
	{
		/// <summary>
		/// Edited action.
		/// </summary>
		private Action action;
		
		/// <summary>
		/// Tree iterator.
		/// </summary>
		private TreeIter iter;
		
		/// <summary>
		/// Tree path.
		/// </summary>
		private TreePath path;
		
		/// <summary>
		/// List of actions.
		/// </summary>
		private ListStore list;
		
		/// <summary>
		/// Plugin.
		/// </summary>
		private Actions plugin;
		
		/// <summary>
		/// Initializes a new instance of class.
		/// </summary>
		/// <param name="plugin">Plugin.</param>
		/// <param name="list">List of actions.</param>
		public EditActionWindow(Actions plugin, ListStore list) : base(WindowType.Toplevel)
		{
			this.Build();
			this.list = list;
			this.plugin = plugin;
			
			try
			{
				this.Icon = IconTheme.Default.LoadIcon("glippy", 128, IconLookupFlags.GenericFallback);
			}
			catch (Exception ex)
			{
				Core.Tools.PrintInfo(ex, this.GetType());
			}			
			
			this.Destroyed += (s, e) => this.Purge();
		}
		
		/// <summary>
		/// Initializes a new instance of class.
		/// </summary>
		/// <param name="plugin">Plugin.</param>
		/// <param name="list">List of plugins.</param>
		/// <param name="action">Edited action.</param>
		/// <param name="path">Tree path of edited element.</param>
		/// <param name="iter">Tree iterator of edited element.</param>
		public EditActionWindow(Actions plugin, ListStore list, Action action, TreePath path, TreeIter iter) : this(plugin, list)
		{
			this.action = action;
			this.iter = iter;
			this.path = path;
			this.label.Text = action.Label;
			this.content.Buffer.Text = action.Content;
			this.Title = Catalog.GetString("Edit action");						
		}
		
		/// <summary>
		/// Cleans window.
		/// </summary>
		public void Purge()
		{
			this.plugin.EditActionWindow = null;
			this.Dispose();
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
		/// Handles button cancel clicked event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnButtonCancelClicked(object sender, EventArgs args)
		{
			this.Destroy();
		}
		
		/// <summary>
		/// Handles button apply clicked event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnButtonApplyClicked(object sender, EventArgs args)
		{
			MessageDialog dialog;
			
			if (this.label.Text.Length == 0)			
			{
				dialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, false, Catalog.GetString("Label is required."));
				dialog.Run();
				dialog.Destroy();
				dialog.Dispose();
			}
			else if (this.content.Buffer.Text.Length == 0)
			{
				dialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, false, Catalog.GetString("Content is required."));
				dialog.Run();
				dialog.Destroy();
				dialog.Dispose();
			}
			else
			{
				if (this.action != null)
				{
					this.action.Label = this.label.Text;
					this.action.Content = this.content.Buffer.Text;					
					this.list.EmitRowChanged(this.path, this.iter);
				}
				else
				{
					this.action = new Action();
					this.action.Label = this.label.Text;
					this.action.Content = this.content.Buffer.Text;					
					this.list.AppendValues(action);
				}
				
				this.Destroy();
			}			
		}
	}
}
