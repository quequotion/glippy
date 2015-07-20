/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Gdk;

namespace Glippy.XHotkeys
{
	/// <summary>
	/// Hotkeys support manager.
	/// </summary>
	public sealed partial class Hotkeys
	{			
		/// <summary>
		/// XHotkeys instance.
		/// </summary>
		private static Hotkeys instance;
		
		/// <summary>
		/// Defines time how long ms X server waits before firing fake event.
		/// </summary>
		private const ulong PasteEventDelay = 100;				
		
		/// <summary>
		/// Type of processing X events.
		/// </summary>
		private const int GrabModeAsync = 1;
		
		/// <summary>
		/// X event key press type.
		/// </summary>
		private const int XEventTypeKeyPress = 2;
		
		/// <summary>
		/// Modifiers omitted on grabing key combination from keyboard.
		/// </summary>
		private static ModifierType[] OmittedModifiers = { ModifierType.None, ModifierType.LockMask, ModifierType.Mod2Mask, ModifierType.Mod5Mask };
		
		/// <summary>
		/// Keys omitted on grabing key combination from keyboard.
		/// </summary>
		private static Key[] OmittedKeys = { Key.Control_L, Key.Control_R, Key.Alt_L, Key.Alt_R, Key.Super_L, Key.Super_R, Key.Shift_L, Key.Shift_R };
		
		/// <summary>
		/// Control key keycode.
		/// </summary>
		private uint controlKeycode;
		
		/// <summary>
		/// V key keycode.
		/// </summary>
		private uint vKeycode;			
		
		/// <summary>
		/// Indicates whether hotkeys support in enabled.
		/// </summary>
		private bool enabled;
		
		/// <summary>
		/// Indicates whether keys should be regrabed.
		/// </summary>
		private bool regrab;
		
		/// <summary>
		/// XHotkey objects dictionary.
		/// </summary>
		private Dictionary<string, Hotkey> hotkeys;
		
		/// <summary>
		/// List of grabbed hotkeys clones.
		/// </summary>
		private List<Hotkey> grabbedHotkeys;
		
		/// <summary>
		/// Function raised when hotkey is grabbed.
		/// </summary>
		private HotkeyGrabbed hotkeyGrabbedFunc;
		
		/// <summary>
		/// Gets main GDK window.
		/// </summary>
		public Window RootWindow { get; private set; }
		
		/// <summary>
		/// Gets XWindow pointer.
		/// </summary>
		public IntPtr XWindow { get; private set; }
		
		/// <summary>
		/// Gets XDisplay pointer.
		/// </summary>
		public IntPtr XDisplay { get; private set; }
		
		/// <summary>
		/// Gets current system architecture.
		/// </summary>
		public Architectures Architecture { get; private set; }			
		
		/// <summary>
		/// Gets instance of Hotkeys class.
		/// </summary>
		public static Hotkeys Instance
		{
			get { return instance; }
		}
				
		/// <summary>
		/// Gets or sets whether hotkeys support is enabled.
		/// </summary>
		public bool Enabled
		{ 
			get
			{
				return this.enabled;
			}			
			set
			{
				if (value && !this.enabled)
				{
					this.GrabHotkeys();
					this.enabled = true;
				}
				else if (!value && this.enabled)
				{
					this.UngrabHotkeys();
					this.enabled = false;
				}
			}
		}
		
		/// <summary>
		/// Gets hotkey instance associated with key. 
		/// </summary>
		/// <param name="key">Key.</param>
		public Hotkey this [string key]
		{
			get
			{
				try
				{
					return this.hotkeys[key];
				}
				catch
				{
					return null;
				}
			}
		}			
		
		/// <summary>
		/// Creates new instance of Hotkeys class.
		/// </summary>
		static Hotkeys()
		{
			instance = new Hotkeys();
		}
		
		/// <summary>
		/// Creates new instance of Hotkeys class.
		/// </summary>
		private Hotkeys()
		{			
			this.enabled = false;
			this.Architecture = Stuff.Architecture();
			this.RootWindow = Global.DefaultRootWindow;
			this.XWindow = gdk_x11_get_default_root_xwindow();
			this.XDisplay = gdk_x11_display_get_xdisplay(this.RootWindow.Display.Handle);
			this.controlKeycode = (uint)XKeysymToKeycode(this.XDisplay, (int)Key.Control_L);
			this.vKeycode = (uint)XKeysymToKeycode(this.XDisplay, (int)Key.v);
			this.hotkeys = new Dictionary<string, Hotkey>();
			this.grabbedHotkeys = new List<Hotkey>();
			XSetErrorHandler(Marshal.GetFunctionPointerForDelegate(new XErrorHandlerFunc(this.OnXError)));
		}
		
		/// <summary>
		/// Sends fake Ctrl+V key combination to X server which causes paste emulation.
		/// </summary>
		public void Paste()
		{
			XTestFakeKeyEvent(this.XDisplay, this.controlKeycode, true, PasteEventDelay);
			XTestFakeKeyEvent(this.XDisplay, this.vKeycode, true, PasteEventDelay);
			XTestFakeKeyEvent(this.XDisplay, this.controlKeycode, false, 0);
			XTestFakeKeyEvent(this.XDisplay, this.vKeycode, false, 0);							
		}
			
		/// <summary>
		/// Registers new hotkey.
		/// </summary>
		/// <param name="key">Key used to access hotkey from collection.</param>
		/// ]<param name="hotkey">XHotkey object.</param>
		public void RegisterHotkey(string key, Hotkey hotkey)
		{
			if (this.IsKeyCombinationInUse(hotkey.Modifiers, hotkey.Key))
				throw new ArgumentException("Key combination in use");
			
			this.hotkeys.Add(key, hotkey);
			this.Refresh();
		}
		
		/// <summary>
		/// Unregisters hotkey.
		/// </summary>
		/// <param name="key">Key used to access hotkey from collection.</param>
		public void UnregisterHotkey(string key)
		{
			if (this.enabled)
			{
				this.Enabled = false;
				this.hotkeys.Remove(key);
				this.Enabled = true;
			}
			else
			{
				this.hotkeys.Remove(key);
			}
		}
		
		/// <summary>
		/// Checks whether any hotkey with selected key combination exists.
		/// </summary>
		/// <param name="modifiers">Hotkey modifiers.</param>
		/// <param name="key">Hotkey key.</param>
		/// <returns>True if hotkey exists, false otherwise.</returns>
		public bool IsKeyCombinationInUse(ModifierType modifiers, Key key)
		{
			return this.hotkeys.Any(h => h.Value.Key == key && h.Value.Modifiers == modifiers);
		}
		
		/// <summary>
		/// Refreshes hotkeys if support is enabled.
		/// </summary>
		public void Refresh()
		{
			if (this.enabled)
			{
				this.Enabled = false;
				this.Enabled = true;
			}
		}
		
		/// <summary>
		/// Grabs keyboard and prepares to catch key combination (which is emitted in HotkeyGrabbed event).
		/// </summary>
		/// <param name="hotkeyGrabbedFunc">Function called when hotkey is grabbed.</param>
		public void GrabKeyboard(HotkeyGrabbed hotkeyGrabbedFunc)
		{
			this.hotkeyGrabbedFunc = hotkeyGrabbedFunc;
			this.regrab = this.enabled;
			this.Enabled = false;							
			Keyboard.Grab(this.RootWindow, false, 0);					
			this.RootWindow.AddFilter(this.OnHotkeyGrabbed);			
		}
		
		/// <summary>
		/// Ungrabs keyboard.
		/// </summary>
		private void UngrabKeyboard()
		{		
			Keyboard.Ungrab(0);
			this.RootWindow.RemoveFilter(this.OnHotkeyGrabbed);
			
			if (this.regrab)
				this.Enabled = true;						
		}
		
		
		/// <summary>
		/// Removes duplicated key combinations.
		/// </summary>
		private void Distinct()
		{
			this.hotkeys = this.hotkeys.Distinct(new XHotkeyComparer()).ToDictionary(k => k.Key, v => v.Value);
		}
		
		/// <summary>
		/// Grabs hotkeys.
		/// </summary>
		private void GrabHotkeys()
		{
			foreach (KeyValuePair<string, Hotkey> hotkey in this.hotkeys)
			{
				if (!hotkey.Value.Enabled)
					continue;
								
				this.GrabHotkey(hotkey.Value);				
				this.grabbedHotkeys.Add((Hotkey)hotkey.Value.Clone());								
			}			
			
			this.RootWindow.AddFilter(this.OnHotkeyPressed);
		}
		
		/// <summary>
		/// Ungrabs hotkeys.
		/// </summary>
		private void UngrabHotkeys()
		{			
			foreach (Hotkey hotkey in this.grabbedHotkeys)			
				this.UngrabHotkey(hotkey);			
			
			this.RootWindow.RemoveFilter(this.OnHotkeyPressed);
			this.grabbedHotkeys.Clear();
			
			return;			
		}
		
		/// <summary>
		/// Grabs single hotkey.
		/// </summary>
		/// <param name="hotkey">Hotkey instance.</param>
		private void GrabHotkey(Hotkey hotkey)
		{			
			uint modifiers = (uint)hotkey.Modifiers;
			XGrabKey(this.XDisplay, hotkey.KeyCode, modifiers, this.XWindow, false, GrabModeAsync, GrabModeAsync);			
			modifiers += (uint)ModifierType.LockMask;
			XGrabKey(this.XDisplay, hotkey.KeyCode, modifiers, this.XWindow, false, GrabModeAsync, GrabModeAsync);
			modifiers += (uint)ModifierType.Mod2Mask;
			XGrabKey(this.XDisplay, hotkey.KeyCode, modifiers, this.XWindow, false, GrabModeAsync, GrabModeAsync);
			modifiers -= (uint)ModifierType.LockMask;
			XGrabKey(this.XDisplay, hotkey.KeyCode, modifiers, this.XWindow, false, GrabModeAsync, GrabModeAsync);
		}
		
		/// <summary>
		/// Ungrabs single hotkey.
		/// </summary>
		/// <param name="hotkey">Hotkey instance.</param>
		private void UngrabHotkey(Hotkey hotkey)
		{
			uint modifiers = (uint)hotkey.Modifiers;
			XUngrabKey(this.XDisplay, hotkey.KeyCode, modifiers, this.XWindow);
			modifiers += (uint)ModifierType.LockMask;
			XUngrabKey(this.XDisplay, hotkey.KeyCode, modifiers, this.XWindow);
			modifiers += (uint)ModifierType.Mod2Mask;
			XUngrabKey(this.XDisplay, hotkey.KeyCode, modifiers, this.XWindow);
			modifiers -= (uint)ModifierType.LockMask;
			XUngrabKey(this.XDisplay, hotkey.KeyCode, modifiers, this.XWindow);							
		}
		
		/// <summary>
		/// Handles X errors.
		/// </summary>
		/// <param name="display">X display.</param>
		/// <param name="xEvent">X event arguments.</param>
		/// <returns>Error code.</returns>
		private int OnXError(IntPtr display, IntPtr xEvent) // TODO: Serious handling.
		{						
			return -1;
		}
		
		/// <summary>
		/// Matches catched key combination to hotkey and executes it's action.
		/// </summary>
		/// <param name="xEvent">X event arguments.</param>
		/// <param name="gdkEvent">Gdk event arguments.</param>
		private FilterReturn OnHotkeyPressed(IntPtr xEvent, Event gdkEvent)
		{
			try
			{			
				IXKeyEvent evnt = null;
				ModifierType modifiers;
					
				switch (this.Architecture)
				{
					case Architectures.X86:
						evnt = (XKeyEvent32)Marshal.PtrToStructure(xEvent, typeof(XKeyEvent32));
						break;	
					
					case Architectures.X86_64:
						evnt = (XKeyEvent64)Marshal.PtrToStructure(xEvent, typeof(XKeyEvent64));
						break;					
				}
								
				if (evnt.Type == XEventTypeKeyPress)
				{					
					modifiers = evnt.Modifiers;
					
					if (modifiers.ToString().Contains(ModifierType.LockMask.ToString()))			
						modifiers = (ModifierType)((int)modifiers - (int)ModifierType.LockMask);
					if (modifiers.ToString().Contains(ModifierType.Mod2Mask.ToString()))
						modifiers = (ModifierType)((int)modifiers - (int)ModifierType.Mod2Mask);
				
					Hotkey hotkey = this.grabbedHotkeys.FirstOrDefault(h => h.KeyCode == evnt.KeyCode && h.Modifiers == modifiers);
					
					if (hotkey != null && hotkey.OnHotkeyPressed != null)
						hotkey.OnHotkeyPressed();					
				}												
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
			}
			
			return FilterReturn.Continue;
		}
												
		/// <summary>
		/// Catches key combination end emits HotkeyGrabbed event.
		/// </summary>
		/// <param name="xEvent">X event arguments.</param>
		/// <param name="gdkEvent">Gdk event arguments.</param>
		/// <returns>Filter state.</returns>
		private FilterReturn OnHotkeyGrabbed(IntPtr xEvent, Event gdkEvent)
		{		
			try
			{
				IXKeyEvent evnt = null;
				Key key;
				ModifierType modifiers = ModifierType.None;
				
				switch (this.Architecture)
				{
					case Architectures.X86:
						evnt = (XKeyEvent32)Marshal.PtrToStructure(xEvent, typeof(XKeyEvent32));
						break;	
					
					case Architectures.X86_64:
						evnt = (XKeyEvent64)Marshal.PtrToStructure(xEvent, typeof(XKeyEvent64));
						break;					
				}
				
				key = (Key)XKeycodeToKeysym(this.XDisplay, evnt.KeyCode, 0);
				modifiers = evnt.Modifiers;			
					
				if (key == Key.Escape || key == Key.BackSpace)
				{
					this.UngrabKeyboard();
					
					if (this.hotkeyGrabbedFunc != null)
						this.hotkeyGrabbedFunc(this, null);
					
					return FilterReturn.Continue;
				}
				
				if (OmittedModifiers.Contains(modifiers) || OmittedKeys.Contains(key))
					return FilterReturn.Continue;
				
				if (modifiers.ToString().Contains(ModifierType.LockMask.ToString()))			
					modifiers = (ModifierType)((int)modifiers - (int)ModifierType.LockMask);
				
				if (modifiers.ToString().Contains(ModifierType.Mod2Mask.ToString()))
					modifiers = (ModifierType)((int)modifiers - (int)ModifierType.Mod2Mask);
							
				
				if (this.hotkeyGrabbedFunc != null)
					this.hotkeyGrabbedFunc(this, new HotkeyGrabbedArgs(modifiers, key));
				
				this.UngrabKeyboard();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
			}
			
			return FilterReturn.Continue;
		}					
	}
}
