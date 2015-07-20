/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using System;
using Gtk;
using Mono.Unix;

namespace Glippy.Snippets
{
	/// <summary>
	/// Edit snippet window.
	/// </summary>
	internal partial class EditSnippetWindow : Window
	{
		/// <summary>
		/// Edited snippet.
		/// </summary>
		private Snippet snippet;
		
		/// <summary>
		/// Snippets treeview iterator.
		/// </summary>
		private TreeIter iter;
		
		/// <summary>
		/// Snippets treeview path.
		/// </summary>
		private TreePath path;
		
		/// <summary>
		/// List of snippets.
		/// </summary>
		private ListStore list;
		
		/// <summary>
		/// Plugin.
		/// </summary>
		private Snippets plugin;
		
		/// <summary>
		/// Initializes a new instance of class.
		/// </summary>
		/// <param name="plugin">Plugin.</param>
		/// <param name="list">List of plugins.</param>
		public EditSnippetWindow(Snippets plugin, ListStore list) : base(WindowType.Toplevel)
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
		/// <param name="newSnippetText">Text of new snippet.</param>
		public EditSnippetWindow(Snippets plugin, ListStore list, string newSnippetText) : this(plugin, list)
		{
			this.content.Buffer.Text = newSnippetText;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Glippy.Snippets.EditSnippetWindow"/> class.
		/// </summary>
		/// <param name="plugin">Plugin.</param>
		/// <param name="list">List of plugins.</param>
		/// <param name="snippet">Snippet to edit.</param>
		/// <param name="path">Snippets treeview path.</param>
		/// <param name="iter">Snippets treeview iterator.</param>
		public EditSnippetWindow(Snippets plugin, ListStore list, Snippet snippet, TreePath path, TreeIter iter) : this(plugin, list)
		{
			this.snippet = snippet;
			this.iter = iter;
			this.path = path;
			this.label.Text = snippet.Label;
			this.content.Buffer.Text = snippet.Content;
			this.Title = Catalog.GetString("Edit snippet");			
		}
		
		/// <summary>
		/// Cleans window.
		/// </summary>
		private void Purge()
		{
			this.plugin.EditSnippetWindow = null;		
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
				if (this.snippet != null)
				{
					this.snippet.Label = this.label.Text;
					this.snippet.Content = this.content.Buffer.Text;					
					this.list.EmitRowChanged(this.path, this.iter);
				}
				else
				{
					this.snippet = new Snippet();
					this.snippet.Label = this.label.Text;
					this.snippet.Content = this.content.Buffer.Text;					
					this.list.AppendValues(snippet);
				}
				
				this.Destroy();
			}			
		}		
	}
}
