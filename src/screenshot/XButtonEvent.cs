/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using System;
using System.Runtime.InteropServices;
using Gdk;

namespace Glippy.Screenshot
{
	/// <summary>
	/// Interface which defines fields used to identify mouse event type and state combination from X event.
	/// </summary>
	internal interface IXButtonEvent
	{		
		/// <summary>
		/// Gets x position.
		/// </summary>
		int X { get; }
		
		/// <summary>
		/// Gets y position.
		/// </summary>
		int Y { get; }
		
		/// <summary>
		/// Gets event type.
		/// </summary>
		EventType Type { get; }		
	}
	
	/// <summary>
	/// XButtonEvent structure used for x86 architecture.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
    internal struct XButtonEvent32 : IXButtonEvent
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
        public uint button;
        public int same_screen;
		
		/// <summary>
		/// Gets x position.
		/// </summary>		
		public int X
		{
			get { return this.x; }
		}
		
		/// <summary>
		/// Gets y position.
		/// </summary>
		public int Y
		{
			get { return this.y; }
		}
		
		/// <summary>
		/// Gets event type.
		/// </summary>
		public EventType Type
		{
			get
			{
				switch (this.type)
				{
					case 6:
						return EventType.MotionNotify;
					
					case 4:
						return EventType.ButtonPress;
						
					case 5:
						return EventType.ButtonRelease;
						
					case 2:
						return EventType.KeyPress;
					
					case 3:
						return EventType.KeyRelease;
					
					default:
						return EventType.Nothing;
				}
			}
		}	
    }
	
	/// <summary>
	/// XButtonEvent structure used for x86_64 architecture.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
    internal struct XButtonEvent64 : IXButtonEvent
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
        public uint button;
        public int same_screen;	    
					
		/// <summary>
		/// Gets X coordinate.
		/// </summary>		
		public int X
		{
			get { return this.x; }
		}
		
		/// <summary>
		/// Gets Y coordinate.
		/// </summary>
		public int Y
		{
			get { return this.y; }
		}
		
		/// <summary>
		/// Gets event type.
		/// </summary>
		public EventType Type
		{
			get
			{
				switch (this.type)
				{
					case 6:
						return EventType.MotionNotify;
					
					case 4:
						return EventType.ButtonPress;
						
					case 5:
						return EventType.ButtonRelease;
						
					case 2:
						return EventType.KeyPress;
					
					case 3:
						return EventType.KeyRelease;
						
					default:
						return EventType.Nothing;
				}
			}
		}	
	}
}
