/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using System;
using System.Collections.Generic;
using System.Text;
using Gdk;

namespace Glippy.XHotkeys
{	
	/// <summary>
	/// Single XHotkey class.
	/// </summary>
	public class Hotkey : ICloneable
	{
		/// <summary>
		/// Key.
		/// </summary>
		private Key key;
		
		/// <summary>
		/// Gets hotkey modifiers.
		/// </summary>
		public ModifierType Modifiers { get; private set; }
		
		/// <summary>
		/// Gets hotkey keycode.
		/// </summary>
		public uint KeyCode { get; private set; }
		
		/// <summary>
		/// Gets or sets hotkey key.
		/// </summary>
		public Key Key
		{
			get
			{
				return this.key;
			}
			
			set
			{
				this.key = value;
				this.KeyCode = (uint)Hotkeys.XKeysymToKeycode(Hotkeys.Instance.XDisplay, (uint)value);
			}
		}
		
		/// <summary>
		/// Whether hotkey is enabled.
		/// </summary>
		public bool Enabled { get; set; }
		
		/// <summary>
		/// Function raised when hotkey is pressed.
		/// </summary>
		public XHotkeyFunc OnHotkeyPressed { get; set; }
		
		/// <summary>
		/// Creates new instance of XHotkey class.
		/// </summary>
		/// <param name="modifiers">Modifiers.</param>
		/// <param name="key">Key.</param>
		/// <param name="enabled">Whether hotkey is enabled.</param>
		/// <param name="onHotkeyPressed">Function raised when hotkey is pressed.</param>
		public Hotkey(ModifierType modifiers, Key key, bool enabled, XHotkeyFunc onHotkeyPressed)
		{
			this.Modifiers = modifiers;
			this.Key = key;
			this.Enabled = enabled;
			this.OnHotkeyPressed = onHotkeyPressed;
		}			
		
		/// <summary>
		/// Gets a deep copy of current instance.
		/// </summary>
		/// <returns>XHotkey instance.</returns>
		public object Clone()
		{
			return new Hotkey(this.Modifiers, this.Key, this.Enabled, this.OnHotkeyPressed);
		}
		
		/// <summary>
		/// Gets string representation of hotkey.
		/// </summary>
		/// <returns>String representation of hotkey.</returns>
		public override string ToString()
		{		
			return new StringBuilder()
				.Append("<").Append(this.Modifiers.ToString()).Append("> ").Append(this.Key.ToString())
				.Replace("Mask", "").Replace("Mod4", "Super").Replace("Mod1", "Alt")
				.ToString();					
		}
	}
	
	/// <summary>
	/// XHotkey comparer.
	/// </summary>
	internal class XHotkeyComparer : IEqualityComparer<KeyValuePair<string, Hotkey>>
	{
		/// <summary>
		/// Compares two hotkeys and returns true if KeyCode and modifiers are the same.
		/// </summary>
		/// <param name="h1">First XHotkey.</param>
		/// <param name="h2">Second XHotkey.</param>
		/// <returns>True or false.</returns>
		public bool Equals(KeyValuePair<string, Hotkey> h1, KeyValuePair<string, Hotkey> h2)
		{
			return h1.Value.KeyCode == h2.Value.KeyCode && h1.Value.Modifiers == h2.Value.Modifiers;
		}
		
		/// <summary>
		/// Generates hash code for XHotkey object.
		/// </summary>
		/// <param name="hotkey">XHotkey.</param>
		/// <returns>Hash code.</returns>
		public int GetHashCode(KeyValuePair<string, Hotkey> hotkey)
		{
			return hotkey.Value.Modifiers.GetHashCode() ^ hotkey.Value.KeyCode.GetHashCode();				
		}
	}	
}
