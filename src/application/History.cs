/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Glippy.Core;
	
namespace Glippy.Application
{	
	/// <summary>
	/// Synchronizes list of clips with history file.
	/// </summary>
	public static class History
	{
		/// <summary>
		/// The name of the history file.
		/// </summary>
		public static readonly string FileName = "history.xml";
		
		/// <summary>
		/// Loads history from file.
		/// </summary>
		/// <returns>List of loaded items.</returns>
		public static List<Item> Load()
		{
			Item item;
			List<Item> list = new List<Item>();
			StringBuilder text = new StringBuilder();										
			
			try
			{
				if (!System.IO.Directory.Exists(EnvironmentVariables.ConfigPath))
					System.IO.Directory.CreateDirectory(EnvironmentVariables.ConfigPath);
				
				using (XmlTextReader reader = new XmlTextReader(EnvironmentVariables.ConfigPath + FileName))
				{					
					reader.Read();
					
					while (reader.Read())
					{
						text.Remove(0, text.Length);
						
						if (reader.Name == "text")
						{							
							reader.Read();								
							text.Append(reader.Value);
							text.Replace("&lt;", "<");
							text.Replace("&gt;", ">");							
							
							item = new Item(text.ToString());
							list.Add(item);								
							reader.Read();
						}
					}								
				}				
			}			
			catch (Exception ex)
			{					
				Tools.PrintInfo(ex, typeof(History));
			}				
				
			return list;
		}
	
		/// <summary>
		/// Saves history to file.
		/// </summary>
		/// <param name="list">List of items.</param>
		public static void Save(ItemsCollection list)
		{
			StringBuilder text = new StringBuilder();				
			
			try
			{
				if (!System.IO.Directory.Exists(EnvironmentVariables.ConfigPath))
					System.IO.Directory.CreateDirectory(EnvironmentVariables.ConfigPath);
				
				using (XmlTextWriter writer = new XmlTextWriter(EnvironmentVariables.ConfigPath + FileName, Encoding.UTF8))
				{										
					writer.Formatting = Formatting.Indented;			
					writer.WriteProcessingInstruction ("xml", "version='1.0'");
					writer.WriteStartElement("items");
																		
					foreach (Item i in list)
					{
						if (i.IsText)
						{
							text.Remove(0, text.Length);												
							text.Append(i.Text);
							text.Replace("<", "&lt;");
							text.Replace(">", "&gt;");
		
							writer.WriteStartElement("item");
								writer.WriteStartElement("text");							
									writer.WriteString(text.ToString());
								writer.WriteEndElement();
							writer.WriteEndElement();						
						}
					}						
							
					writer.WriteEndElement();
					writer.Close();					
				}
			}
			catch (Exception ex)
			{
				Tools.PrintInfo(ex, typeof(History));
			}
		}				
	}
}
