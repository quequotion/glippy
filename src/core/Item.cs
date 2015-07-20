/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using System;
using System.Text;
using Gdk;
using Glippy.Core.Extensions;
using Gtk;

namespace Glippy.Core
{	
	/// <summary>
	/// Clipboard item.
	/// </summary>
	public class Item : IDisposable
	{
		/// <summary>
		/// Gets item label.
		/// </summary>
		public string Label { get; set; }
		
		/// <summary>
		/// Gets item text.
		/// </summary>
		public string Text { get; private set; }				
								
		/// <summary>
		/// Gets item image.
		/// </summary>
		public Pixbuf Image { get; private set; }					
		
		/// <summary>
		/// Gets complex data target.
		/// </summary>
		public Atom Target { get; private set; }
		
		/// <summary>
		/// Gets complex data.
		/// </summary>
		public byte[] Data { get; private set; }
		
		/// <summary>
		/// Gets subtitute text of complex data.
		/// </summary>
		public byte[] DataText { get; private set ; }
		
		/// <summary>
		/// Gets information whether element has been disposed.
		/// </summary>
		public bool IsDisposed { get; private set; }
		
		/// <summary>
		/// Gets a value indicating whether item is text type.
		/// </summary>
		public bool IsText { get; private set; }
		
		/// <summary>
		/// Gets a value indicating whether item is image type.
		/// </summary>
		public bool IsImage { get; private set; }
		
		/// <summary>
		/// Gets a value indicating whether item is data type.
		/// </summary>
		public bool IsData { get; private set; }
		
		/// <summary>
		/// Creates new text item.
		/// </summary>
		/// <param name="text">Item text.</param>
		public Item(string text)
		{		
			this.IsText = true;
			this.Text = text;
			StringBuilder sb = new StringBuilder();
			sb.Append(text);
			CreateLabel(sb);
			this.Label = sb.ToString();					
		}			
		
		/// <summary>
		/// Creates new image item.
		/// </summary>
		/// <param name="image">Pixbuf.</param>
		public Item(Pixbuf image)
		{
			this.IsImage = true;
			this.Image = image;
			this.Label = string.Format("{0} [{1}x{2} / {3}]", UnicodeCharacters.Image, image.Width.ToString(), image.Height.ToString(), System.DateTime.Now.ToString());
			this.Text = this.Label;
		}
		
		/// <summary>
		/// Creates new data item.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="text">Substitute text.</param>
		public Item(Atom target, byte[] data, string text)
		{
			this.IsData = true;
			this.IsText = true;
			this.Data = data;
			this.Text = text ?? string.Empty;
			this.DataText = System.Text.Encoding.UTF8.GetBytes(this.Text);
			this.Target = target;			
			StringBuilder sb = new StringBuilder();
			sb.Append(text);
			CreateLabel(sb);
			
			if (target.Name == Targets.File)
			{
				this.Label = string.Format("{0} {1}", UnicodeCharacters.Closet, sb.ToString());
			}
			else if (target.Name == Targets.Html)
			{
				this.Label = string.Format("{0} {1}", UnicodeCharacters.Tag, sb.ToString());
			}					
		}
		
		/// <summary>
		/// Creates label using length from configuration.
		/// </summary>
		/// <param name="text"></param>		
		public static void CreateLabel(StringBuilder text)
		{			
			int item_length = Settings.Instance[Settings.Keys.UI.LabelLength].AsInteger();
			
			if (text.Length > item_length)
			{
				text.Remove(item_length/2 - 1, text.Length - item_length);
				text.Insert(text.Length/2, "...");
			}			
			
			text.Replace("\n", UnicodeCharacters.Return);
			text.Replace("\r\n", UnicodeCharacters.Return);
			text.Replace("\t", UnicodeCharacters.Tab);
			text.Replace("_", " ");
		}			
		
		/// <summary>
		/// Releases all resource used by the Item object.
		/// </summary>
		public void Dispose()
		{
			if (!this.IsDisposed)
			{
				if (this.Image != null)
				{
					this.Image.Dispose();
					this.Image = null;
				}
				
				this.IsDisposed = true;
			}
		}
		
		/// <summary>
		/// Gets complex data from item and sets as content when requested.
		/// </summary>
		/// <param name="clipboard">Clipboard.</param>
		/// <param name="selectionData">Selection data.</param>
		/// <param name="info">Target index.</param>
		internal void GetDataFunc(Gtk.Clipboard clipboard, SelectionData selectionData, uint info)
		{
			switch (info)
			{
				case 0:
					selectionData.Set(this.Target, 8, this.Data);
					break;
				
				case 1:
				default:
					selectionData.Set(Targets.Atoms[Targets.UtfString], 8, this.DataText);
					break;
			}								
		}
		
		/// <summary>
		/// Clears clipboard.
		/// </summary>
		/// <param name="clipboard">Clipboard.</param>
		internal void ClearFunc(Gtk.Clipboard clipboard)
		{
		}
	}		
}
