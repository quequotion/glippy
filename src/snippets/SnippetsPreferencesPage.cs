/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using System;
using Glippy.Core.Extensions;
using Gtk;
using Mono.Unix;

namespace Glippy.Snippets
{
	/// <summary>
	/// Snippets preferences page.
	/// </summary>
	internal partial class SnippetsPreferencesPage : Bin
	{
		/// <summary>
		/// Plugin.
		/// </summary>
		private Snippets plugin;
		
		/// <summary>
		/// If true, events are handled.
		/// </summary>
		private bool handleEvents;
		
		/// <summary>
		/// List of snippets.
		/// </summary>
		private ListStore list;
		
		/// <summary>
		/// Initializes a new instance of class.
		/// </summary>
		/// <param name="plugin">Plugin.</param>
		/// <param name="list">List of snippets.</param>
		public SnippetsPreferencesPage(Snippets plugin, ListStore list)
		{
			this.Build();
			
			this.plugin = plugin;
			this.list = list;			
			this.enable.Active = Core.Settings.Instance[SettingsKeys.Enable].AsBoolean();
			this.pasteOnSnippetSelection.Active = Core.Settings.Instance[SettingsKeys.PasteOnSelection].AsBoolean();
			
			TreeViewColumn column = new TreeViewColumn();
			column.Title = Catalog.GetString("Label");
			CellRendererText cell = new CellRendererText();
			column.PackStart(cell, true);		
			column.SetCellDataFunc(cell, delegate (TreeViewColumn col, CellRenderer c, TreeModel m, TreeIter i)
			{
				Snippet s = (Snippet)m.GetValue(i, 0);
				((CellRendererText)c).Text = s.Label;
			});
			this.snippets.AppendColumn(column);
			
			column = new TreeViewColumn();
			column.Title = Catalog.GetString("Content");
			cell = new CellRendererText();			
			column.PackStart(cell, true);		
			column.SetCellDataFunc(cell, delegate (TreeViewColumn col, CellRenderer c, TreeModel m, TreeIter i)
			{
				Snippet s = (Snippet)m.GetValue(i, 0);
				((CellRendererText)c).Text = s.Content;
			});
			this.snippets.AppendColumn(column);					
			
			this.snippets.Model = list;	
			this.snippets.ShowAll();										
			
			this.SetSnippetsSensitivity();
			this.handleEvents = true;			
		}		
		
		/// <summary>
		/// Sets widgets sensitivity.
		/// </summary>
		private void SetSnippetsSensitivity()
		{
			this.snippets.Sensitive = this.buttonAddSnippet.Sensitive = this.buttonEditSnippet.Sensitive = this.buttonRemoveSnippet.Sensitive = this.pasteOnSnippetSelection.Sensitive = this.enable.Active;
		}
		
		/// <summary>
		/// Handles enable toggled event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnEnableToggled(object sender, EventArgs args)
		{
			if (!this.handleEvents)
				return;
			
			Core.Settings.Instance[SettingsKeys.Enable] = this.enable.Active;
			this.SetSnippetsSensitivity();
		}
		
		/// <summary>
		/// Handles paste on snippet selection toggled event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnPasteOnSnippetSelectionToggled(object sender, System.EventArgs e)
		{
			if (!this.handleEvents)
				return;
			
			Core.Settings.Instance[SettingsKeys.PasteOnSelection] = this.pasteOnSnippetSelection.Active;
		}
		
		/// <summary>
		/// Handles button add snippet clicked event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnButtonAddSnippetClicked(object sender, EventArgs args)
		{
			if (this.plugin.EditSnippetWindow == null)
			{
				this.plugin.EditSnippetWindow = new EditSnippetWindow(this.plugin, this.list);
				this.plugin.EditSnippetWindow.ShowAll();
			}
			else
			{
				this.plugin.EditSnippetWindow.Present();
			}
		}
		
		/// <summary>
		/// Handles the button remove snippet clicked event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnButtonRemoveSnippetClicked(object sender, EventArgs args)
		{
			if (this.plugin.EditSnippetWindow == null)
			{
				TreeIter iter;
				this.snippets.Selection.GetSelected(out iter);		
				this.list.Remove(ref iter);						
			}
			else
			{
				this.plugin.EditSnippetWindow.Present();
			}
		}
		
		/// <summary>
		/// Handles button edit snippet clicked event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnButtonEditSnippetClicked(object sender, EventArgs args)
		{
			if (this.plugin.EditSnippetWindow != null)
			{
				this.plugin.EditSnippetWindow.Present();
				return;
			}
			
			TreeIter iter;
			this.snippets.Selection.GetSelected(out iter);					
			Snippet snippet = this.list.GetValue(iter, 0) as Snippet;
			
			if (snippet != null)
			{				
				int pos = 0;
				
				foreach (object[] o in this.list)
				{
					if (o[0] == snippet)
						break;
					
					pos++;
				}
				
				this.plugin.EditSnippetWindow = new EditSnippetWindow(this.plugin, this.list, snippet, new TreePath(pos.ToString()), iter);
				this.plugin.EditSnippetWindow.ShowAll();				
			}					
		}	
	}
}
