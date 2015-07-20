/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using System.Collections;
using System.Collections.Generic;
using Glippy.Core.Extensions;

namespace Glippy.Core
{
	/// <summary>
	/// Items collection.
	/// </summary>
	public class ItemsCollection : IEnumerable<Item>
	{
		/// <summary>
		/// List of items.
		/// </summary>
		private List<Item> items;
		
		/// <summary>
		/// Gets items count.
		/// </summary>
		public int Count
		{
			get { return this.items.Count; }
		}
				
		/// <summary>
		/// Initializes a new instance of the ItemsCollection class.
		/// </summary>
		public ItemsCollection()
		{
			this.items = new List<Item>();	
		}
		
		/// <summary>
		/// Gets element at the specified index.
		/// </summary>
		/// <param name="index">Index.</param>
		public Item this [int index]
		{
			get { return this.items[index];	}
		}
		
		/// <summary>
		/// Adds the specified item to collection.
		/// </summary>
		/// <param name="item">Item.</param>
		internal void Add(Item item)
		{
			this.items.Add(item);
			this.RemoveRedundantItems();
		}
		
		/// <summary>
		/// Inserts the specified index and item.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="item">Item.</param>
		internal void Insert(int index, Item item)
		{
			this.items.Insert(index, item);
			this.RemoveRedundantItems();
		}
		
		/// <summary>
		/// Removes and disposes the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		internal void Remove(Item item)
		{
			item.Dispose();
			this.items.Remove(item);			
		}
		
		/// <summary>
		/// Removes item at index.
		/// </summary>
		/// <param name="index">Index.</param>
		internal void RemoveAt(int index)
		{
			this.items[index].Dispose();
			this.items.RemoveAt(index);			
		}
		
		/// <summary>
		/// Moves item to the top of collection.
		/// </summary>
		/// <param name="item">Item.</param>
		internal void MoveTop(Item item)
		{
			this.items.Remove(item);
			this.items.Insert(0, item);
		}
		
		/// <summary>
		/// Clears collection.
		/// </summary>
		internal void Clear()
		{
			foreach (Item i in this.items)
			{
				i.Dispose();
			}
			
			this.items.Clear();
		}
		
		/// <summary>
		/// Determines whether collection contains specified item.
		/// </summary>
		/// <param name="item">Item to determine.</param>
		/// <returns>True if collection contains item, false otherwise.</returns>
		internal bool Have(Item item)
		{
			return this.items.Contains(item);
		}
		
		/// <summary>
		/// Removes redundant items from list. 
		/// </summary>
		private void RemoveRedundantItems()
		{
			int size = Settings.Instance[Settings.Keys.UI.Size].AsInteger();
			
			if (this.items.Count > size)	
			{
				while (this.items.Count != size)
				{
					for (int i = this.items.Count - 1; i >= 0; i--)
					{						
						if (this.items[i] != Clipboard.Instance.KeyboardItem && this.items[i] != Clipboard.Instance.MouseItem)
						{
							this.items[i].Dispose();								
							this.items.RemoveAt(i);
							
							break;
						}
					}
				}
			}
		}		
		
		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>The enumerator.</returns>
		public IEnumerator<Item> GetEnumerator()
		{
			return this.items.GetEnumerator();
		}
		
		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>The enumerator.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.items.GetEnumerator();
		}			
	}
}
