 /* 
  * Glippy
  * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
  * The program is distributed under the terms of the GNU General Public License Version 3.
  * See LICENCE for details.
  */

using System;
using System.Runtime.InteropServices;

namespace Glippy.XHotkeys
{
	/// <summary>
	/// Hotkeys support manager DLL imports.
	/// </summary>
	public sealed partial class Hotkeys
	{				
		/// <summary>
		/// Converts keysym to keycode.
		/// </summary>
		/// <param name="display">XDisplay pointer.</param>
		/// <param name="key">Keysym.</param>
		/// <returns>Keycode.</returns>
		[DllImport("libX11.so.6")]
		internal static extern int XKeysymToKeycode(IntPtr display, uint key);
		
		/// <summary>
		/// Converts keycode to keysym.
		/// </summary>
		/// <param name="display">XDisplay pointer.</param>
		/// <param name="key">Keycode</param>
		/// <param name="index">Index.</param>
		/// <returns>Keysym.</returns>
		[DllImport("libX11.so.6")]
		internal static extern uint XKeycodeToKeysym(IntPtr display, uint key, int index);	    
							
		/// <summary>
		/// Sends fake XEvent.
		/// </summary>
		/// <param name="display">XDisplay pointer.</param>
		/// <param name="keycode">Keycode.</param>
		/// <param name="is_press">Whether key is pressed.</param>		
		/// <param name="delay">Event delay.</param>
		/// <returns>Error code.</returns>
		[DllImport("libXtst.so.6")]
		private static extern int XTestFakeKeyEvent(IntPtr display, uint keycode, bool is_press, ulong delay);				
		
		/// <summary>
		/// Sets X error handler.
		/// </summary>
		/// <param name="handler">Pointer to function.</param>
		/// <returns>Error code.</returns>
		[DllImport("libX11.so.6")]
		private static extern int XSetErrorHandler(IntPtr handler);
		
		/// <summary>
		/// Establishes a passive grab on the keyboard.
		/// </summary>
		/// <param name="display">XDisplay.</param>
		/// <param name="keycode">Keycode to grab.</param>
		/// <param name="modifiers">Modifiers to grab.</param>
		/// <param name="grab_window">Window to grab.</param>
		/// <param name="owner_events">Specifies whether the keyboard events are to be reported as usual.</param>
		/// <param name="pointer_mode">Specifies futher processing of pointer events.</param>
		/// <param name="keyboard_mode">Specifies further processing of keyboard events.</param>
		/// <returns>Error code.</returns>
		[DllImport("libX11.so.6")]
		private static extern int XGrabKey(IntPtr display, uint keycode, uint modifiers, IntPtr grab_window, bool owner_events, int pointer_mode, int keyboard_mode);
	    
		/// <summary>
		/// Releases the key combination on the specified window if it was grabbed by this client.
		/// </summary>
		/// <param name="display">XDisplay.</param>
		/// <param name="keycode">Keycode to ungrab.</param>
		/// <param name="modifiers">Modifiers to ungrab.</param>
		/// <param name="grab_window">Window to ungrab.</param>
		/// <returns>Error code.</returns>
		[DllImport("libX11.so.6")]
		private static extern int XUngrabKey(IntPtr display, uint keycode, uint modifiers, IntPtr grab_window);
		
		/// <summary>
		/// Returns the X display of a Gdk display.
		/// </summary>
		/// <param name="gdk_display">GDK display pointer.</param>	
		/// <returns>XDisplay pointer.</returns>
		[DllImport("libgtk-x11-2.0.so.0")]
		private static extern IntPtr gdk_x11_display_get_xdisplay (IntPtr gdk_display);
		
		/// <summary>
		/// Gets the root window of the default screen.
		/// </summary>
		/// <returns>XLib window pointer.</returns>
		[DllImport("libgtk-x11-2.0.so.0")]
		private static extern IntPtr gdk_x11_get_default_root_xwindow();
	}
}
