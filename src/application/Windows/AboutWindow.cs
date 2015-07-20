/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using System.Reflection;
using Gdk;
using Gtk;
using Mono.Unix;	

namespace Glippy.Application
{
	/// <summary>
	/// About window.
	/// </summary>
	internal static class AboutWindow
	{				
		/// <summary>
		/// Shows about window.
		/// </summary>
		public static void Show()
		{										
			AboutDialog about = new AboutDialog();						
			Assembly asm = Assembly.GetExecutingAssembly();
			Pixbuf icon = null;		
			
			try
			{
				icon = IconTheme.Default.LoadIcon("glippy", 128, Gtk.IconLookupFlags.GenericFallback);
			}
			catch (System.Exception ex)
			{
				Core.Tools.PrintInfo(ex, typeof(AboutWindow));
			}
			
			about.ProgramName = Name;
			about.Version = Core.EnvironmentVariables.Version;
			about.Comments = Description;
			about.Copyright = (asm.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0] as AssemblyCopyrightAttribute).Copyright;
			about.Website = "https://launchpad.net/glippy";
			about.Authors = authors;
			about.Artists = artists;	
			about.License = license;	
			about.TranslatorCredits = translators;
			about.Icon = icon;			
			about.Logo = IconTheme.Default.LoadIcon("glippy", 48, Gtk.IconLookupFlags.GenericFallback);
			about.Run();
			
			if (icon != null)
				icon.Dispose();
			
			if (about.Logo != null)
				about.Logo.Dispose();
			
			about.Destroy();
			about.Dispose();			
		}
		
		public static readonly string Name = "Glippy";
		public static readonly string Description = Catalog.GetString("Not so simple clipboard manager for GNOME");
		private static readonly string[] authors = new string[] { "Wojciech Kowalczyk <bikooo@gmail.com>" };
		private static readonly string[] artists = new string[] { "Paweł Kolankowski" };		
		private static readonly string translators = @"Русский: Саша Пантюхин, Александр Уфимцев, discont, Oleg Koptev
Deutsch: Johannes Hell, Daniel Winzen, Jan Simon
Français: Thibault Févry, Ptitphysik, ktulu77, YannUbuntu, Quentin Pagès, Agmenor
Italiano: Enrico G, Renzo Bagnati
Español: Fitoschido, Paco Molinero, simon
slmb :العربية
Eesti: tabbernuk, René Pärts
日本語: YannUbuntu, 0guma, kawaji
Português (Brasil): YannUbuntu, Fabiano Leite
Norsk (bokmål)‬: Kjetil Rydland
Bahasa Indonesia: Andika Triwidada
";
		private static readonly string license =
		@"
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program. If not, see <http://www.gnu.org/licenses/>.";			
	}	
}
