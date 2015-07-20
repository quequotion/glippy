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

namespace Glippy.Actions
{
	/// <summary>
	/// Actions preferences page.
	/// </summary>
	internal partial class ActionsPreferencesPage : Bin
	{
		/// <summary>
		/// Plugin.
		/// </summary>
		private Actions plugin;
		
		/// <summary>
		/// If true, events are handled.
		/// </summary>
		private bool handleEvents;
		
		/// <summary>
		/// List of actions.
		/// </summary>
		private ListStore list;
		
		/// <summary>
		/// Initializes a new instance of class.
		/// </summary>
		/// <param name="plugin">Plugin.</param>
		/// <param name="list">List of actions.</param>
		public ActionsPreferencesPage(Actions plugin, ListStore list)
		{
			this.Build();
			this.plugin = plugin;
			this.list = list;			
			this.enable.Active = Core.Settings.Instance[SettingsKeys.Enable].AsBoolean();
			
			TreeViewColumn column = new TreeViewColumn();
			column.Title = Catalog.GetString("Label");
			CellRendererText cell = new CellRendererText();
			column.PackStart(cell, true);		
			column.SetCellDataFunc(cell, delegate (TreeViewColumn col, CellRenderer c, TreeModel m, TreeIter i)
			{
				Action a = (Action)m.GetValue(i, 0);
				((CellRendererText)c).Text = a.Label;
			});
			this.actions.AppendColumn(column);
			
			column = new TreeViewColumn();
			column.Title = Catalog.GetString("Content");
			cell = new CellRendererText();			
			column.PackStart(cell, true);		
			column.SetCellDataFunc(cell, delegate (TreeViewColumn col, CellRenderer c, TreeModel m, TreeIter i)
			{
				Action a = (Action)m.GetValue(i, 0);
				((CellRendererText)c).Text = a.Content;
			});
			this.actions.AppendColumn(column);					
			
			this.actions.Model = list;	
			this.actions.ShowAll();										
			
			this.SetActionsSensitivity();
			this.handleEvents = true;
		}		
		
		/// <summary>
		/// Sets widgets sensitivity.
		/// </summary>
		private void SetActionsSensitivity()
		{
			this.actions.Sensitive = this.buttonAddSnippet.Sensitive = this.buttonEditSnippet.Sensitive = this.buttonRemoveSnippet.Sensitive = this.enable.Active;
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
			this.SetActionsSensitivity();
		}
		
		/// <summary>
		/// Handles button add snippet clicked event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnButtonAddSnippetClicked(object sender, EventArgs args)
		{
			if (this.plugin.EditActionWindow == null)
			{
				this.plugin.EditActionWindow = new EditActionWindow(this.plugin, this.list);
				this.plugin.EditActionWindow.ShowAll();
			}			
			else
			{
				this.plugin.EditActionWindow.Present();
			}
		}
		
		/// <summary>
		/// Handles button remove snippet clicked event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnButtonRemoveSnippetClicked(object sender, EventArgs args)
		{
			if (this.plugin.EditActionWindow == null)
			{
				TreeIter iter;
				this.actions.Selection.GetSelected(out iter);		
				this.list.Remove(ref iter);						
			}
			else
			{
				this.plugin.EditActionWindow.Present();
			}
		}
		
		/// <summary>
		/// Handles button edit snippet clicked event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnButtonEditSnippetClicked(object sender, EventArgs args)
		{
			if (this.plugin.EditActionWindow != null)
			{
				this.plugin.EditActionWindow.Present();
				return;
			}
			
			TreeIter iter;
			this.actions.Selection.GetSelected(out iter);					
			Action action = this.list.GetValue(iter, 0) as Action;
			
			if (action != null)
			{				
				int pos = 0;
				
				foreach (object[] o in this.list)
				{
					if (o[0] == action)
						break;
					
					pos++;
				}
				
				this.plugin.EditActionWindow = new EditActionWindow(this.plugin, this.list, action, new TreePath(pos.ToString()), iter);
				this.plugin.EditActionWindow.ShowAll();							
			}					
		}
	}
}
