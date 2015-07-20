/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Gdk;
using Glippy.Core.Extensions;
using Gtk;

namespace Glippy.Core
{	
	/// <summary>
	/// Clipboard manager.
	/// </summary>
	public sealed class Clipboard
	{		
		/// <summary>
		/// Timeout for checking mouse clipboard content.
		/// </summary>
		private const int MouseClipboardDelay = 500;
		
		/// <summary>
		/// Clipboard instance.
		/// </summary>
		private static readonly Clipboard instance = new Clipboard();
		
		/// <summary>
		/// Pixbuf supported formats.
		/// </summary>
		private PixbufFormat[] pixbufFormats;
						
		/// <summary>
		/// GTK keyboard clipboard.
		/// </summary>
		private Gtk.Clipboard keyboardClipboard;
		
		/// <summary>
		/// GTK mouse (primary) clipboard.
		/// </summary>
		private Gtk.Clipboard mouseClipboard;
				
		/// <summary>
		/// Local mutex used in synchronizing.
		/// </summary>
		private bool synchronizing;
		
		/// <summary>
		/// Timer which postpones mouse clipboard text change event.
		/// </summary>
		private Timer timer;
		
		/// <summary>
		/// Timer lock.
		/// </summary>
		private object timerLock;
		
		/// <summary>
		/// Gets or sets a value indicating whether new mouse item will be added to history.
		/// </summary>
		public bool LockMouseClipboard { get; set; }
		
		/// <summary>
		/// Gets current keyboard clipboard item.
		/// </summary>
		public Item KeyboardItem { get; private set; }
		
		/// <summary>
		/// Gets current mouse clipboard item.
		/// </summary>
		public Item MouseItem { get; private set; }
		
		/// <summary>
		/// Gets list with stored clips.
		/// </summary>
		public ItemsCollection Items { get; private set; }		
		
		/// <summary>
		/// Gets instance of Clipboard.
		/// </summary>
		public static Clipboard Instance
		{
			get { return instance; }
		}
		
		/// <summary>
		/// Raised when clipboard's content has changed.
		/// </summary>
		public event ClipboardChanged ClipboardChanged;
		
		/// <summary>
		/// Creates clipboard object.
		/// </summary>
		private Clipboard()
		{
			this.timerLock = new object();
			this.KeyboardItem = null;
			this.MouseItem = null;
			this.Items = new ItemsCollection();
			this.keyboardClipboard = Gtk.Clipboard.Get(Gdk.Selection.Clipboard);
			this.keyboardClipboard.OwnerChange += this.OnKeyboardClipboardOwnerChanged;
			this.mouseClipboard = Gtk.Clipboard.Get(Gdk.Selection.Primary);			
			this.mouseClipboard.OwnerChange += this.OnMouseClipboardOwnerChanged;
			this.pixbufFormats = Pixbuf.Formats;			
			this.OnKeyboardClipboardOwnerChanged(this, null);
			this.OnMouseClipboardOwnerChanged(this, null);
		}
				
		/// <summary>
		/// Clears clipboards and removes all item from list.
		/// </summary>
		public void Clear()
		{						
			this.Items.Clear();	
			this.MouseItem = null;
			this.KeyboardItem = null;
			this.keyboardClipboard.Text = string.Empty;
			this.keyboardClipboard.Clear();			
			this.mouseClipboard.Clear();			
			
			if (this.ClipboardChanged != null)
				this.ClipboardChanged(this, null);
		}						
				
		/// <summary>
		/// Clears old items and loads new to clipboard.
		/// </summary>
		/// <param name="items">List of new items.</param>
		public void Load(IEnumerable<Item> items)
		{
			this.Items.Clear();
			
			foreach (Item i in items)
			{
				this.Items.Add(i);
			}
			
			Item item;
			
			if (this.KeyboardItem != null)
			{
				item = this.KeyboardItem;
				this.KeyboardItem = null;
				this.SetAsKeyboardContent(item);
				item.Dispose();
			}
			else if (this.Items.Count > 0)
			{
				this.SetAsKeyboardContent(this.Items[0]);	
			}
					
			if (this.MouseItem != null)
			{
				item = this.MouseItem;
				this.MouseItem = null;
				this.SetAsMouseContent(this.MouseItem);
				this.MouseItem.Dispose();
			}				
			else if (this.Items.Count > 0)
			{
				this.SetAsMouseContent(this.Items[0]);
			}
			
			if (this.ClipboardChanged != null)
				this.ClipboardChanged(this, null);
		}
		
		/// <summary>
		/// Sets item as clipboard's content and disposed it.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetAsContent(Item item)
		{						
			if (Settings.Instance[Settings.Keys.Core.SynchronizeClipboards].AsBoolean() && Settings.Instance[Settings.Keys.Core.KeyboardClipboard].AsBoolean() && Settings.Instance[Settings.Keys.Core.MouseClipboard].AsBoolean())
			{
				this.SetAsKeyboardContent(item, false);
				this.SetAsMouseContent(item);
			}
			else
			{
				if (Settings.Instance[Settings.Keys.Core.KeyboardClipboard].AsBoolean())
					this.SetAsKeyboardContent(item, false);
				
				if (Settings.Instance[Settings.Keys.Core.MouseClipboard].AsBoolean())
        	    	this.SetAsMouseContent(item, false);
        	    	
	    		if (!this.Items.Have(item))
					item.Dispose();
			}		
		}
		
		/// <summary>
		/// Sets item as content in keyboard clipboard.
		/// </summary>
		/// <param name="item">Item to set as content.</param>
		/// <param name="dispose">If true tries to dispose item.</param>
		public void SetAsKeyboardContent(Item item, bool dispose = true)
		{
			if (item.IsData)
			{
				this.keyboardClipboard.SetWithData(new TargetEntry[] { new TargetEntry(item.Target.Name, TargetFlags.App, 0), new TargetEntry(Targets.String, TargetFlags.App, 1) }, item.GetDataFunc, item.ClearFunc);
			}
			else if (item.IsText)
			{
				this.keyboardClipboard.Text = item.Text;
			}
			else if (item.IsImage)
			{
				this.keyboardClipboard.Image = item.Image;			
				this.Items.Remove(item);				
			}					
			
			if (dispose && !this.Items.Have(item))
				item.Dispose();
		}		
		
		/// <summary>
		/// Sets item as content in keyboard clipboard.
		/// </summary>
		/// <param name="item">Item to set as content.</param>
		/// <param name="dispose">If true tries to dispose item.</param>		
		public void SetAsMouseContent(Item item, bool dispose = true)
		{
			if (item.IsText)
				this.mouseClipboard.Text = item.Text;
				
			if (dispose && !this.Items.Have(item))
				item.Dispose();
		}		
				
		/// <summary>
		/// Handles change in keyboard clipboard.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnKeyboardClipboardOwnerChanged(object sender, OwnerChangeArgs args)
		{	
			if (!Settings.Instance[Settings.Keys.Core.KeyboardClipboard].AsBoolean())
				return;
			
			if (Settings.Instance[Settings.Keys.Core.Images].AsBoolean() && this.keyboardClipboard.WaitIsImageAvailable())
			{
				this.OnKeyboardImageReceived(this.keyboardClipboard, this.keyboardClipboard.WaitForImage());				
			}
			else if (this.keyboardClipboard.WaitIsTargetAvailable(Targets.Atoms[Targets.File]))
			{
				this.OnKeyboardContentReceived(this.keyboardClipboard, this.keyboardClipboard.WaitForContents(Targets.Atoms[Targets.File]));
			}
			else if (this.keyboardClipboard.WaitIsTargetAvailable(Targets.Atoms[Targets.Html]))
			{
				this.OnKeyboardContentReceived(this.keyboardClipboard, this.keyboardClipboard.WaitForContents(Targets.Atoms[Targets.Html]));
			}
			else if (this.keyboardClipboard.WaitIsTextAvailable())
			{
				this.OnKeyboardTextReceived(this.keyboardClipboard, this.keyboardClipboard.WaitForText());				
			}			
			else if (args != null && args.Event.Reason == OwnerChange.Close)			
			{
				if (this.KeyboardItem != null)
					this.SetAsKeyboardContent(this.KeyboardItem);
			}
		}
				
		/// <summary>
		/// Handles change in mouse clipboard.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnMouseClipboardOwnerChanged(object sender, OwnerChangeArgs args)
		{			
			if (!Settings.Instance[Settings.Keys.Core.MouseClipboard].AsBoolean())
				return;			
											
			if (this.mouseClipboard.WaitIsTextAvailable())
			{
				lock (this.timerLock)
				{					
					if (this.timer != null)
					{
						this.timer.Change(MouseClipboardDelay, System.Threading.Timeout.Infinite);
					}
					else
					{
						this.timer = new Timer((o) =>
						{
							lock (this.timerLock)
							{
								Gtk.Application.Invoke((s, e) => this.OnMouseTextReceived(this.mouseClipboard, this.mouseClipboard.WaitForText()));																	
								this.timer.Dispose();
								this.timer = null;
							}
						}, null, MouseClipboardDelay, System.Threading.Timeout.Infinite);												
					}								
				}																				
			}
			else
			{
				if (this.MouseItem != null)
					this.SetAsMouseContent(this.MouseItem);
			}
		}
		
		/// <summary>
		/// Handles text received from keyboard clipboard.
		/// </summary>
		/// <param name="clip">Clipboard.</param>
		/// <param name="text">Text.</param>
		private void OnKeyboardTextReceived(Gtk.Clipboard clip, string text)
		{			
			if (string.IsNullOrEmpty(text) || (this.KeyboardItem != null && text == this.KeyboardItem.Text))
				return;
			
			if (this.synchronizing)
			{
				this.synchronizing = false;									
				this.KeyboardItem = this.MouseItem;
				
				if (this.ClipboardChanged != null)
					this.ClipboardChanged(this, null);													
				
				return;			
			}
			
			Item item = this.Items.FirstOrDefault(i => i.Text == text);
			
			if (item != null)
			{
				this.Items.MoveTop(item);
			}
			else
			{
				item = new Item(text);			
				string label = item.Label;
				int c = 1;				
				
				while (this.Items.Any(i => i.Label == item.Label))
				{
					item.Label = label + c.ToString();
					c++;
				}
				
				this.Items.Insert(0, item);				
			}
			
			ClipboardChangedArgs args = new ClipboardChangedArgs(Gdk.Selection.Clipboard, this.KeyboardItem, item);
			
			this.KeyboardItem = item;
			
			if (Settings.Instance[Settings.Keys.Core.SynchronizeClipboards].AsBoolean() && Settings.Instance[Settings.Keys.Core.MouseClipboard].AsBoolean() && !this.synchronizing)
			{
				this.synchronizing = true;
				this.mouseClipboard.Text = text;				
			}
			else 
			{
				if (this.ClipboardChanged != null)
					this.ClipboardChanged(this, args);													
			}
		}
		
		/// <summary>
		/// Handles image received from keyboard clipboard.
		/// </summary>
		/// <param name="clip">Clipboard.</param>
		/// <param name="pixbuf">Image.</param>
		private void OnKeyboardImageReceived(Gtk.Clipboard clip, Pixbuf pixbuf)
		{		
			if (pixbuf == null)
				return;
			
			if (Settings.Instance[Settings.Keys.Core.SynchronizeClipboards].AsBoolean() && Settings.Instance[Settings.Keys.Core.MouseClipboard].AsBoolean())				
			{
				this.MouseItem = null;
				this.mouseClipboard.Clear();
			}
											
			Item item = new Item(pixbuf);
			
			ClipboardChangedArgs args = new ClipboardChangedArgs(Gdk.Selection.Clipboard, this.KeyboardItem, item);
			
			this.KeyboardItem = item;
			
			this.Items.Insert(0, item);
			
			if (this.ClipboardChanged != null)
				this.ClipboardChanged(this, args);			
		}	
		
		/// <summary>
		/// Handles more complicated content from keyboard clipboard.
		/// </summary>
		/// <param name="clipboard">Clipboard.</param>
		/// <param name="selectionData">Data.</param>
		private void OnKeyboardContentReceived(Gtk.Clipboard clipboard, SelectionData selectionData)
		{
			Item item = this.Items.FirstOrDefault(i => i.IsData && i.Target.Name == selectionData.Target.Name && i.Data.SequenceEqual(selectionData.Data));
												
			if (item != null)
			{
				this.Items.MoveTop(item);
			}
			else
			{
				Gtk.SelectionData sub_data = this.keyboardClipboard.WaitForContents(Targets.Atoms[Targets.UtfString]);
				
				if (sub_data == null)
					sub_data = this.keyboardClipboard.WaitForContents(Targets.Atoms[Targets.String]);
				
				if (Settings.Instance[Settings.Keys.Core.SupportedFilesAsImages].AsBoolean() && selectionData.Target.Name == Targets.File && sub_data != null && sub_data.Text != null && sub_data.Text.Length > 1)
				{
					string ext = System.IO.Path.GetExtension(sub_data.Text);
					
					if (ext.Length > 1)
					{
						ext = ext.Substring(1, ext.Length - 1).ToLower();					
						PixbufFormat format = this.pixbufFormats.FirstOrDefault(f => f.Extensions.Contains(ext));
						
						if (format != null)
						{
							try
							{	
								this.OnKeyboardImageReceived(clipboard, new Pixbuf(sub_data.Text));
								return;
							}
							catch (Exception ex)
							{
								Tools.PrintInfo(ex, this.GetType());
							}
						}
					}
				}
								
				string sub_text = sub_data != null ? sub_data.Text : System.Text.Encoding.UTF8.GetString(selectionData.Data);							
				
				item = new Item(selectionData.Target ,selectionData.Data, sub_text);
				
				string label = item.Label;
				int c = 1;				
				
				while (this.Items.Any(i => i.Label == item.Label))
				{
					item.Label = label + c.ToString();
					c++;
				}
				
				this.Items.Insert(0, item);								
			}
												
			Item text_item = this.Items.FirstOrDefault(i => !i.IsData && i.Text == item.Text);
			
			if (text_item != null)
			{
				this.Items.Remove(text_item);
				
				if (this.MouseItem == text_item)
				{
					this.SetAsMouseContent(item);
				 	MouseItem = item;
				}
			}
			
			ClipboardChangedArgs args = new ClipboardChangedArgs(Gdk.Selection.Clipboard, this.KeyboardItem, item);
			
			this.KeyboardItem = item;
			
			if (Settings.Instance[Settings.Keys.Core.SynchronizeClipboards].AsBoolean() && Settings.Instance[Settings.Keys.Core.MouseClipboard].AsBoolean() && item.Target.Name == Targets.Html)
			{			
				synchronizing = true;
				this.SetAsMouseContent(item);
			}
			else
			{
				if (this.ClipboardChanged != null)
					this.ClipboardChanged(this, args);															
			}
		}		
		
		/// <summary>
		/// Handles text received from mouse clipboard.
		/// </summary>
		/// <param name="clip">Clipboard.</param>
		/// <param name="text">Text.</param>
		private void OnMouseTextReceived(Gtk.Clipboard clip, string text)
		{
			if (string.IsNullOrEmpty(text))
				return;
			
			if (this.synchronizing)
			{
				this.synchronizing = false;
				this.MouseItem = this.KeyboardItem;
				
				if (this.ClipboardChanged != null)
					this.ClipboardChanged(this, null);															
			}
			
			if (this.MouseItem != null && text == this.MouseItem.Text)
				return;
			
			Item item = this.Items.FirstOrDefault(i => i.Text == text);
			Item prev_item = this.MouseItem;
			
			if (item != null)
			{
				this.Items.MoveTop(item);
				this.MouseItem = item;
				
				if (Settings.Instance[Settings.Keys.Core.SynchronizeClipboards].AsBoolean() && Settings.Instance[Settings.Keys.Core.KeyboardClipboard].AsBoolean())
				{
					this.synchronizing = true;
					this.keyboardClipboard.Text = text;									
				}	
			}
			else
			{
				item = new Item(text);
				
				string label = item.Label;
				int c = 1;				
				
				while (this.Items.Any(i => i.Label == item.Label))
				{
					item.Label = label + c.ToString();
					c++;
				}
				
				if (!this.LockMouseClipboard || this.MouseItem == null)
				{
					this.Items.Insert(0, item);
					
					if (this.MouseItem != null)
					{
						// TODO: myszkosprawdzaczka				
						// Checks which way current text is changing, if none, we're in begining.
						if (!(text.StartsWith(this.MouseItem.Text) && text.Length > this.MouseItem.Text.Length)
						    && !(this.MouseItem.Text.StartsWith(text) && text.Length < this.MouseItem.Text.Length)
					    	&& !(text.EndsWith(this.MouseItem.Text) && text.Length > this.MouseItem.Text.Length)
					    	&& !(this.MouseItem.Text.EndsWith(text) && text.Length < this.MouseItem.Text.Length))
						{
							if ((text.Contains(this.MouseItem.Text) && text.Length > this.MouseItem.Text.Length)
						    	|| (text.Length == 1 && this.MouseItem.Text.Length == 1 && text != this.MouseItem.Text))
							{
								if (this.Items.Count > 1 && (this.KeyboardItem == null || this.KeyboardItem.Text != this.MouseItem.Text))
								{
									this.Items.RemoveAt(1);
								}
							}
						}
						else
						{
							if (this.Items.Count > 1 && (this.KeyboardItem == null || this.KeyboardItem.Text != this.MouseItem.Text))
							{
								this.Items.RemoveAt(1);
							}
						}				
					}
					
					this.MouseItem = item;
					
					if (Settings.Instance[Settings.Keys.Core.SynchronizeClipboards].AsBoolean()
						&& Settings.Instance[Settings.Keys.Core.KeyboardClipboard].AsBoolean())
					{
						this.synchronizing = true;					
						this.keyboardClipboard.Text = text;							
						return;
					}			
				}									
			}
			
			if (this.ClipboardChanged != null)
				this.ClipboardChanged(this, new ClipboardChangedArgs(Gdk.Selection.Primary, prev_item, item));
		}							
	}	
}
