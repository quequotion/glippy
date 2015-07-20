/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

namespace Glippy.Screenshot
{
	/// <summary>
	/// Rectangle class.
	/// </summary>
	internal class Rectangle
	{
		/// <summary>
		/// Gets begin point X coordinate.
		/// </summary>
		public int X1 { get; private set; }					
		
		/// <summary>
		/// Gets end point X coordinate.
		/// </summary>		
		public int X2 { get; private set; }
		
		/// <summary>
		/// Gets begin point Y coordinate.
		/// </summary>		
		public int Y1 { get; private set; }
		
		/// <summary>
		/// Gets end point Y coordinate.
		/// </summary>		
		public int Y2 { get; private set; }
		
		/// <summary>
		/// Gets rectangle width.
		/// </summary>
		public int Width { get; private set; }		
		
		/// <summary>
		/// Gets rectangle height.
		/// </summary>
		public int Height { get; private set; }
		
		/// <summary>
		/// Initializes a new instance of the Rectangle class.
		/// </summary>
		/// <param name="x1">Begin point X coordinate.</param>
		/// <param name="y1">Begin point Y coordinate.</param>
		public Rectangle(int x1, int y1)
		{
			this.X1 = this.X2 = x1;
			this.Y1 = this.Y2 = y1;
			this.SetSize();
		}
		
		/// <summary>
		/// Sets end point coordinates.
		/// </summary>
		/// <param name="x2">X coordinate.</param>
		/// <param name="y2">Y coordinate.</param>
		public void SetEndPoint(int x2, int y2)
		{
			this.X2 = x2;
			this.Y2 = y2;
			this.SetSize();
		}
		
		/// <summary>
		/// Sets rectangle width and height.
		/// </summary>
		private void SetSize()
		{
			if (this.X1 < this.X2)
				this.Width = this.X2 - this.X1;
			else if (this.X1 > this.X2)
				this.Width = this.X1 - this.X2;
			else
				this.Width = 1;
			
			if (this.Y1 < this.Y2)
				this.Height = this.Y2 - this.Y1;
			else if (this.Y1 > this.Y2)
				this.Height = this.Y1 - this.Y2;
			else
				this.Height = 1;			
		}
	}
}
