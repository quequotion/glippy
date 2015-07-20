/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using System;
using System.Runtime.InteropServices;
using System.Threading;
using Gdk;
using Glippy.Core;
using Glippy.Core.Api;
using Mono.Unix;

namespace Glippy.Screenshot
{
	/// <summary>
	/// Screenshot class.
	/// </summary>
	public class Screenshot : IPlugin
	{
		/// <summary>
		/// Gdk root window.
		/// </summary>
		private Window rootWindow;
				
		/// <summary>
		/// Gtk window which shows selected rectangle.
		/// </summary>
		private Gtk.Window window;
		
		/// <summary>
		/// Indicates whether window is composited.
		/// </summary>
		private bool composited;
		
		/// <summary>
		/// Indicates whether pointer and keyboard are grabbed.
		/// </summary>
		private bool grabbed;
		
		/// <summary>
		/// Screenshot of desktop.
		/// </summary>
		private Gdk.Pixbuf pixbuf;
		
		/// <summary>
		/// Selection rectangle.
		/// </summary>
		private Rectangle rectangle;
		
		/// <summary>
		/// Current system architecture.
		/// </summary>
		private Architectures architecture;
		
		/// <summary>
		/// Menu item submenu.
		/// </summary>
		private Gtk.Menu submenu;				
		
		/// <summary>
		/// Gets plugin name. Used in plugins tab and as label of preferences page.
		/// </summary>
		public string Name { get; private set; }
		
		/// <summary>
		/// Gets plugin description.
		/// </summary>
		public string Description { get; private set; }
		
		/// <summary>
		/// Gets menu item which is put in menu.
		/// </summary>
		public Gtk.MenuItem MenuItem
		{
			get
			{
				Gtk.MenuItem menuitem = new Gtk.MenuItem(Catalog.GetString("Sc_reen"));							
				menuitem.Submenu = submenu;			
				
				return menuitem;
			}	
		}
		
		/// <summary>
		/// Gets container, which is attached to preferences page.
		/// </summary>
		public Gtk.Container PreferencesPage
		{
			get { return null; }
		}
		
		/// <summary>
		/// Initializes a new instance of the Screenshot class.
		/// </summary>
		public Screenshot()
		{
			this.Name = Catalog.GetString("Screenshot");
			this.Description = Catalog.GetString("Create screenshots, pick colors from screen.");
		}
		
		/// <summary>
		/// Loads plugin.
		/// </summary>
		public void Load()
		{
			this.rootWindow = Global.DefaultRootWindow;
			this.architecture = Stuff.Architecture();			
			
			this.submenu = new Gtk.Menu();			
			
			Gtk.MenuItem mi = new Gtk.MenuItem(Catalog.GetString("_Take region screenshot"));
			this.submenu.Append(mi);
			mi.Activated += (s, e) => this.TakeRegionScreenshot();				
			mi.ButtonPressEvent += (s, e) => this.TakeRegionScreenshot();
			
			mi = new Gtk.MenuItem(Catalog.GetString("Take screenshot of _entire screen"));
			this.submenu.Append(mi);
			mi.Activated += (s, e) => this.TakeScreenScreenshot();				
			mi.ButtonPressEvent += (s, e) => this.TakeScreenScreenshot();
			
			mi = new Gtk.MenuItem(Catalog.GetString("_Pick color"));
			this.submenu.Append(mi);			
			mi.Activated += (s, e) => this.PickColor();
			mi.ButtonPressEvent += (s, e) => this.PickColor();			
		}			
				
		/// <summary>
		/// Releases all resource used by the Screenshot object.
		/// </summary>
		public void Dispose()
		{
			this.DisposePart();			
		}
		
		/// <summary>
		/// Releases temporary resource used by the Screenshot object.
		/// </summary>
		private void DisposePart()
		{
			if (this.window != null)
			{
				this.window.Destroy();
				this.window.Dispose();				
			}
			
			if (this.pixbuf != null)
			{
				this.pixbuf.Dispose();
				this.pixbuf = null;
			}
		}
		
		/// <summary>
		/// Takes entire screen screenshot.
		/// </summary>
		private void TakeScreenScreenshot()
		{
			ThreadPool.QueueUserWorkItem((o) =>
			{
				Thread.Sleep(500);
				Gtk.Application.Invoke((s, e) =>
				{
					using (Pixbuf pb = Pixbuf.FromDrawable(this.rootWindow, this.rootWindow.Colormap, 0, 0, 0, 0, this.rootWindow.Screen.Width, this.rootWindow.Screen.Height))
					{
						new ImageWindow(pb).ShowAll();
					}			
				});
			});			
		}
		
		/// <summary>
		/// Takes region screenshot.
		/// </summary>
		private void TakeRegionScreenshot()
		{
			ThreadPool.QueueUserWorkItem((o) =>
			{
				Thread.Sleep(500);
				Gtk.Application.Invoke((s, e) =>
				{
					if (this.Grab())			
						this.rootWindow.AddFilter(this.OnScreenshotMouseChanged);
				});
			});
		}
		
		/// <summary>
		/// Takes the color from screen.
		/// </summary>
		private void PickColor()
		{
			ThreadPool.QueueUserWorkItem((o) =>
			{
				Thread.Sleep(500);
				Gtk.Application.Invoke((s, e) =>
				{
					if (this.Grab())						
						this.rootWindow.AddFilter(this.OnColorMouseChanged);
				});
			});
		}
		
		/// <summary>
		/// Grabs mouse pointer and keyboard. Adds event handler.
		/// </summary>
		private bool Grab()
		{	
			if (!grabbed)
			{								
				this.grabbed = true;				
				Pointer.Grab(this.rootWindow, false, EventMask.PointerMotionMask, null, new Cursor(CursorType.Crosshair), 0);
				Keyboard.Grab(this.rootWindow, false, 0);				
				
				return true;
			}
			
			return false;
		}
		
		/// <summary>
		/// Ungrabs mouse and keyboard. Removes event handler.
		/// </summary>
		private void Ungrab()
		{
			this.DisposePart();
			
			if (grabbed)
			{
				this.grabbed = false;
				this.rootWindow.RemoveFilter(this.OnScreenshotMouseChanged);
				this.rootWindow.RemoveFilter(this.OnColorMouseChanged);
				Keyboard.Ungrab(0);
				Pointer.Ungrab(0);				
			}
		}
		
		/// <summary>
		/// Handles X events. Gets first and second selection points and sets selected screen region as clipboard content.
		/// </summary>
		/// <param name="xEvent">X event.</param>
		/// <param name="gdkEvent">Gdk event.</param>
		private FilterReturn OnScreenshotMouseChanged(IntPtr xEvent, Event gdkEvent)
		{
			try
			{			
				IXButtonEvent evnt = null;
					
				switch (this.architecture)
				{
					case Architectures.X86:
						evnt = (XButtonEvent32)Marshal.PtrToStructure(xEvent, typeof(XButtonEvent32));
						break;	
					
					case Architectures.X86_64:
						evnt = (XButtonEvent64)Marshal.PtrToStructure(xEvent, typeof(XButtonEvent64));
						break;					
				}
				
				switch (evnt.Type)
				{
					case EventType.ButtonPress:
						this.rectangle = new Rectangle(evnt.X, evnt.Y);
						this.pixbuf = Pixbuf.FromDrawable(this.rootWindow, this.rootWindow.Colormap, 0, 0, 0, 0, this.rootWindow.Screen.Width, this.rootWindow.Screen.Height);
						this.CreateWindow();						
												
						break;
						
					case EventType.ButtonRelease:				
						this.rectangle.SetEndPoint(evnt.X, evnt.Y);
						Pixbuf pb = this.GetPixmapFromRectangle();
						
						this.Ungrab();												
						this.rectangle = null;
					
						if (pb != null)
						{
							new ImageWindow(pb).ShowAll();
							pb.Dispose();
						}
						
						break;
						
					case EventType.MotionNotify:
						if (this.rectangle != null && this.rectangle.X2 != evnt.X && this.rectangle.Y2 != evnt.Y)
						{
							this.rectangle.SetEndPoint(evnt.X, evnt.Y);
							this.window.Move(this.rectangle.X1 < this.rectangle.X2 ? this.rectangle.X1 : this.rectangle.X2, this.rectangle.Y1 < this.rectangle.Y2 ? this.rectangle.Y1 : this.rectangle.Y2);
							this.window.Resize(this.rectangle.Width, this.rectangle.Height);						
						}
						
						break;		
						
					case EventType.KeyPress:
					case EventType.KeyRelease:
						this.Ungrab();			
						this.rectangle = null;
						
						break;
				}
			}
			catch (Exception ex)
			{
				this.Ungrab();			
				Tools.PrintInfo(ex, this.GetType());
			}
			
			return FilterReturn.Continue;
		}			

		/// <summary>
		/// Handles X events. Gets point and reads it's color.
		/// </summary>
		/// <param name="xEvent">X event.</param>
		/// <param name="gdkEvent">Gdk event.</param>
		private FilterReturn OnColorMouseChanged(IntPtr xEvent, Event gdkEvent)
		{
			try
			{							
				IXButtonEvent evnt = null;
					
				switch (this.architecture)
				{
					case Architectures.X86:
						evnt = (XButtonEvent32)Marshal.PtrToStructure(xEvent, typeof(XButtonEvent32));
						break;	
					
					case Architectures.X86_64:
						evnt = (XButtonEvent64)Marshal.PtrToStructure(xEvent, typeof(XButtonEvent64));
						break;					
				}
				
				switch (evnt.Type)
				{
					case EventType.ButtonRelease:
						this.pixbuf = Pixbuf.FromDrawable(this.rootWindow, this.rootWindow.Colormap, evnt.X, evnt.Y, 0, 0, 1, 1);						
						byte r, g, b;
					
						unsafe
						{
							byte* ptr = (byte*)this.pixbuf.Pixels.ToPointer();						
							r = *ptr;
							g = *(++ptr);
							b = *(++ptr);
						}
						
						this.Ungrab();				
						new ColorWindow(r, g, b).ShowAll();
					
						break;
					
					case EventType.KeyPress:
					case EventType.KeyRelease:
						this.Ungrab();								
						break;
				}
			}
			catch (Exception ex)
			{
				this.Ungrab();			
				Tools.PrintInfo(ex, this.GetType());
			}
			
			return FilterReturn.Continue;
		}
		
		/// <summary>
		/// Gets the pixmap from screen using selected rectangle.
		/// </summary>
		/// <returns>The pixmap from screen.</returns>
		private Pixbuf GetPixmapFromRectangle()
		{
			if (this.pixbuf == null || this.rectangle == null || this.rectangle.X1 == this.rectangle.X2 || this.rectangle.Y1 == this.rectangle.Y2)
				return null;
			
			return new Pixbuf(this.pixbuf,
				this.rectangle.X1 < this.rectangle.X2 ? this.rectangle.X1 : this.rectangle.X2,
				this.rectangle.Y1 < this.rectangle.Y2 ? this.rectangle.Y1 : this.rectangle.Y2,
				this.rectangle.Width,
				this.rectangle.Height);	
		}	
		
		/// <summary>
		/// Creates window which emulates selection.
		/// </summary>
		private void CreateWindow()
		{
			this.window = new Gtk.Window(Gtk.WindowType.Toplevel);
			this.window.SetDefaultSize(1, 1);
			this.window.AppPaintable = true;
			this.window.Decorated = false;
			this.window.KeepAbove = true;
			this.window.SkipPagerHint = true;
			this.window.SkipTaskbarHint = true;
			this.composited = this.window.Screen.IsComposited;
			
			Colormap map = this.composited ? this.window.Screen.RgbaColormap : this.window.Screen.RgbColormap;
			
			this.window.ScreenChanged += (s, e) => this.window.Colormap = map;		
			this.window.ExposeEvent += (object s, Gtk.ExposeEventArgs args) =>
			{
				if (this.rectangle == null)
					return;
				
				using (Cairo.Context context = CairoHelper.Create(this.window.GdkWindow))
				{					
					context.Operator = Cairo.Operator.Source;
					
					if (this.composited)
						context.SetSourceRGBA(0.0, 0.0, 0.0, 0.3);												
					else
						context.SetSourceRGB(0.3, 0.3, 0.3);														
					
					context.Rectangle(0, 0, this.rectangle.Width, this.rectangle.Height);					
					context.Fill();
					context.Stroke();					
				}				
			};
			
			this.window.Colormap = map;
			this.window.ShowAll();
		}			
	}
}
