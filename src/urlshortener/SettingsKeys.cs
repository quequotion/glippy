/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

namespace Glippy.UrlShortener
{
	/// <summary>
	/// URL shortener plugin settings keys.
	/// </summary>
	internal static class SettingsKeys
	{
		/// <summary>
		/// Current URL shortener.
		/// </summary>
		public const string UrlShortener = "UrlShortener.UrlShortener";
		
		/// <summary>
		/// Bit.ly username.
		/// </summary>
		public const string BitLyUsername = "UrlShortener.BitLyUsername";
		
		/// <summary>
		/// Bit.ly user API key.
		/// </summary>
		public const string BitLyApiKey = "UrlShortener.BitLyApiKey";
	}
}
