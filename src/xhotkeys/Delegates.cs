/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using System;
using Gdk;

namespace Glippy.XHotkeys
{
	/// <summary>
	/// X error function handler delegate.
	/// </summary>
	internal delegate int XErrorHandlerFunc(IntPtr display, IntPtr xEvent);			
	
	/// <summary>
	/// XHotkey function delegate.
	/// </summary>
	public delegate void XHotkeyFunc();		
	
	/// <summary>
	/// XHotkey grabbed event delegate.
	/// </summary>
	public delegate void HotkeyGrabbed(object sender, HotkeyGrabbedArgs args);
	
	/// <summary>
	/// XHotkey grabbed event arguments.
	/// </summary>
	public class HotkeyGrabbedArgs
	{
		/// <summary>
		/// Gets combination modifiers.
		/// </summary>
		public ModifierType Modifiers { get; private set; }
		
		/// <summary>
		/// Gets combination key.
		/// </summary>
		public Key Key { get; private set; }
		
		/// <summary>
		/// Creates new instance of HotkeyGrabbedArgs.
		/// </summary>
		/// <param name="modifiers">Combination modifiers.</param>
		/// <param name="key">Combination key.</param>
		public HotkeyGrabbedArgs(ModifierType modifiers, Key key)
		{
			this.Modifiers = modifiers;
			this.Key = key;
		}
	}
}
