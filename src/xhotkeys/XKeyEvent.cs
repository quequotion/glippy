/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using System;
using System.Runtime.InteropServices;
using Gdk;

namespace Glippy.XHotkeys
{
	/// <summary>
	/// Interface which defines fields used to identify key combination from X event.
	/// </summary>
	internal interface IXKeyEvent
	{
		/// <summary>
		/// Gets key modifiers.
		/// </summary>
		ModifierType Modifiers { get; }
		
		/// <summary>
		/// Gets keycode.
		/// </summary>
		uint KeyCode { get; }
		
		/// <summary>
		/// Gets event type.
		/// </summary>
		int Type { get; }
	}
	
	/// <summary>
	/// XKeyEvent structure used for x86 architecture.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
    internal struct XKeyEvent32 : IXKeyEvent
    {
        public int type;
        public uint serial;
        public int send_event;
        public IntPtr display;
        public uint window;
        public uint root;
        public uint subwindow;
        public uint time;
        public int x, y;
        public int x_root, y_root;
        public uint state;
        public uint keycode;
        public int same_screen;
		
		/// <summary>
		/// Gets key modifiers.
		/// </summary>
		public ModifierType Modifiers
		{
			get	{ return (ModifierType)this.state; }
		}
		
		/// <summary>
		/// Gets keycode.
		/// </summary>
		public uint KeyCode
		{
			get { return this.keycode; }
		}
		
		/// <summary>
		/// Gets event type.
		/// </summary>
		public int Type
		{
			get { return this.type; }
		}
    }
	
	/// <summary>
	/// XKeyEvent structure used for x86_64 architecture.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
    internal struct XKeyEvent64 : IXKeyEvent
    {
        public int type;
        public ulong serial;
        public int send_event;
        public IntPtr display;			
		public ulong window;
        public ulong root;
        public ulong subwindow;
        public ulong time;					
        public int x, y;
        public int x_root, y_root;
        public uint state;
        public uint keycode;
        public int same_screen;	    
					
		/// <summary>
		/// Gets key modifiers.
		/// </summary>
		public ModifierType Modifiers
		{
			get	{ return (ModifierType)this.state; }
		}
		
		/// <summary>
		/// Gets keycode.
		/// </summary>
		public uint KeyCode
		{
			get { return this.keycode; }
		}
		
		/// <summary>
		/// Gets event type.
		/// </summary>
		public int Type
		{
			get { return this.type; }
		}
	}
}
