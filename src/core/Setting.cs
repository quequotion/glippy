/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using System;

namespace Glippy.Core
{	
	/// <summary>
	/// Single setting.
	/// </summary>
	internal class Setting
	{		
		/// <summary>
		/// Option value.
		/// </summary>
		private object val;
		
		/// <summary>
		/// Option value type.
		/// </summary>
		private Type type;
		
		/// <summary>
		/// Gets GConf key of option.
		/// </summary>
		public string Key { get; private set; }		
		
		/// <summary>
		/// Gets or sets option value.
		/// </summary>
		public object Value
		{
			get { return this.val; }
			set
			{
				if (value.GetType() == this.type)
					this.val = value;
				else
					throw new ArgumentException("Value", string.Format("Value is of type {0}, but settings requires {1} type.", value.GetType().Name, this.type.Name));
			}
		}
		
		/// <summary>
		/// Gets or sets option value type.
		/// </summary>
		public SettingTypes Type { get; private set; }
		
		/// <summary>
		/// Gets a value indicating whether option change requires menu rebuild.
		/// </summary>
		public bool RequiresMenuRebuild { get; private set; }
		
		/// <summary>
		/// Creates objects.
		/// </summary>
		/// <param name="key">Key used in GConf.</param>
		/// <param name="type">Value type.</param>
		/// <param name="requiresMenuRebuild">If true, SettingChanged event propagates information whether menu rebuild is required.</param>		
		public Setting(string key, SettingTypes type, bool requiresMenuRebuild)
		{
			this.Key = key;
			this.RequiresMenuRebuild = requiresMenuRebuild;
			this.Type = type;
				
			switch (type)
			{
				case SettingTypes.Boolean:
					this.type = typeof(bool);
					break;
				
				case SettingTypes.Integer:
					this.type = typeof(int);
					break;
					
				case SettingTypes.String:
					this.type = typeof(string);
					break;				
			}			
		}						
	}	
}
