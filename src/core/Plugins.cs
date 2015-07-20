/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using Glippy.Core.Api;

namespace Glippy.Core
{
	/// <summary>
	/// Plugins management.
	/// </summary>
	public static class Plugins
	{
		/// <summary>
		/// Loads plugin.
		/// </summary>
		/// <param name="dllName">Name of library which contains plugin.</param>
		/// <returns>Loaded plugin instance or null.</returns>
		public static IBase LoadPlugin(string dllName)
		{
			IBase plugin = null;			
			Assembly asm = Assembly.LoadFile((dllName.Contains(AppDomain.CurrentDomain.BaseDirectory) ? string.Empty : AppDomain.CurrentDomain.BaseDirectory) + dllName + (dllName.EndsWith(".dll") ? string.Empty : ".dll"));
			
			foreach (Type t in asm.GetExportedTypes())
			{
				if (t.GetInterface(typeof(IBase).Name) != null)
					plugin = Activator.CreateInstance(t) as IBase;					
			}
			
			return plugin;
		}
		
		/// <summary>
		/// Loads all plugins.
		/// </summary>
		public static List<IBase> LoadAllPlugins()
		{
			List<IBase> plugins = new List<IBase>();
			
			foreach (string dll in System.IO.Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll"))
			{	
				try
				{
					IBase plugin = LoadPlugin(dll);
					
					if (plugin != null)
						plugins.Add(plugin);
				}
				catch { }
			}
			
			return plugins;
		}
	}
}
