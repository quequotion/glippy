/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using System;
using Glippy.Core;
using Gtk;

namespace Glippy.Screenshot
{
	/// <summary>
	/// Color window.
	/// </summary>
	internal partial class ColorWindow : Window
	{
		/// <summary>
		/// Initializes a new instance of the ColorWindow class.
		/// </summary>
		/// <param name="r">Red color value.</param>
		/// <param name="g">Green color value.</param>
		/// <param name="b">Blue color value.</param>
		public ColorWindow (byte r, byte g, byte b) : base(WindowType.Toplevel)
		{
			this.Build();
			
			try
			{
				this.Icon = IconTheme.Default.LoadIcon("glippy", 128, IconLookupFlags.GenericFallback);
			}
			catch (Exception ex)
			{
				Tools.PrintInfo(ex, this.GetType());
			}
			
			this.red.Text = r.ToString();
			this.green.Text = g.ToString();
			this.blue.Text = b.ToString();
			
			float h, ss, v;
			RgbToHsv(r, g, b, out h, out ss, out v);			
			this.hue.Text = Math.Round(h).ToString();
			this.saturation.Text = Math.Round(ss).ToString();
			this.value.Text = Math.Round(v).ToString();
			
			string hr = r < 16 ? "0" : string.Empty;
			string hg = g < 16 ? "0" : string.Empty;					
			string hb = b < 16 ? "0" : string.Empty;			
			hr += r.ToString("X");
			hg += g.ToString("X");
			hb += b.ToString("X");
			this.hex.Text = hr + hg + hb;
						
			this.color.ModifyBg(StateType.Normal, new Gdk.Color(r, g, b));		
			
			this.Destroyed += (s, e) => this.Purge();
		}
		
		/// <summary>
		/// Converts RGB color to HSV.
		/// </summary>
		/// <param name="r">Source color red value.</param>
		/// <param name="g">Source color green value.</param>
		/// <param name="b">Source color blue value.</param>
		/// <param name="h">Destination hue variable.</param>
		/// <param name="s">Destination saturation variable.</param>
		/// <param name="v">Destination value variable.</param>
		private static void RgbToHsv(byte r, byte g, byte b, out float h, out float s, out float v)
		{
			float t = Math.Min(r, Math.Min(g, b));
			h = s = v = 0;			
			v = Math.Max(r, Math.Max(g, b));
			
			if (t == v)
			{
				h = 0;
			}
			else
			{
				if (r == v)
					h = (g - b) * 60.0f / (v - t);
				else if (g == v)
					h = 120.0f + (b - r) * 60.0f / (v - t);
				else if (b == v)
					h = 240.0f + (r - g) * 60.0f / (v - t);				
			}
			
			if (h < 0)
				h += 360.0f;
			
			s = v == 0 ? 0.0f : (v - t) * 100.0f / v;
			v = 100.0f * v / 255.0f;					
		}
				
		/// <summary>
		/// Cleans window.
		/// </summary>
		private void Purge()
		{
			this.Dispose();
		}
		
		/// <summary>
		/// Closes window.
		/// </summary>
		/// <param name="sender">Sender</param>
		/// <param name="args">Event arguments.</param>
		private void OnButtonCloseClicked(object sender, EventArgs args)
		{
			this.Destroy();			
		}
		
		/// <summary>
		/// Closes window on Escape press.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Event arguments.</param>
		private void OnKeyPressEvent(object sender, KeyPressEventArgs args)
		{
			if (args.Event.Key == Gdk.Key.Escape)
				this.OnButtonCloseClicked(sender, null);
		}
	}
}
