/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using Glippy.Core;
using Mono.Unix;

namespace Glippy.UrlShortener
{
	/// <summary>
	/// Shorteners.
	/// </summary>
	internal static class Shorteners
	{
		/// <summary>
		/// Bit.ly shortener.
		/// </summary
		public static class BitLy
		{
			/// <summary>
			/// API URL.
			/// </summary>
			public const string ApiUrl = "http://api.bitly.com";
			
			/// <summary>
			/// API shorten operation url.
			/// </summary>
			public const string ApiShortenUrl = "/v3/shorten";
			
			/// <summary>
			/// Bit.ly parameters.
			/// </summary>
			public static class Parameters
			{
				public const string Format = "format";
				public const string FormatXml = "xml";
				public const string Login = "login";
				public const string ApiKey = "apiKey";
				public const string Url = "longUrl";
				public const string ResponseStatusCode = "status_code";
				public const string ResponseUrl = "url";				
			}
			
			/// <summary>
			/// Shortens the specified url.
			/// </summary>
			/// <param name="url">URL to shorten.</param>
			/// <param name="login">Bit.ly login.</param>
			/// <param name="apiKey">Bit.ly API key,</param>
			/// <returns>Shortened URL or error message.</returns>
			public static string Shorten(string url, string login, string apiKey)
			{
				string return_text;
				HttpWebRequest request;				
				
				StringBuilder get_builder = new StringBuilder(ApiShortenUrl);
				get_builder.Append("?");
				get_builder.Append(Parameters.Format).Append("=").Append(Parameters.FormatXml);
				get_builder.Append("&");
				get_builder.Append(Parameters.Login).Append("=").Append(UrlEncoder.UrlEncode(login));				
				get_builder.Append("&");
				get_builder.Append(Parameters.ApiKey).Append("=").Append(UrlEncoder.UrlEncode(apiKey));				
				get_builder.Append("&");
				get_builder.Append(Parameters.Url).Append("=").Append(UrlEncoder.UrlEncode(url));											
					
				try
				{					
					request = (HttpWebRequest)HttpWebRequest.Create(ApiUrl + get_builder.ToString());
					request.Method = "GET";						
					
					using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))									
					{
						using (XmlTextReader xmlreader = new XmlTextReader(reader))						
						{
							return_text = ParseResponseXml(xmlreader);
						}
					}					
					
					if (!return_text.StartsWith(Catalog.GetString("Error: ")))
					{
						return_text = string.Format("<a href=\"{0}\">{0}</a>", return_text);
					}					
				}
				catch (System.Threading.ThreadAbortException ex)
				{
					throw ex;
				}
				catch (Exception ex)
				{
					Tools.PrintInfo(ex, typeof(BitLy));
					return_text = Catalog.GetString("Error: ") + ex.Message;
				}
				
				return return_text;	
			}
			
			/// <summary>
			/// Parses Bit.ly's XML response.
			/// </summary>			
			/// <param name="reader">XML reader.</param>
			/// <returns>Url or error message.</returns>
			private static string ParseResponseXml(XmlTextReader reader)
			{								
				string return_text = null;
								
				try
				{
					reader.Read();
					string status_code = null;
					
					while (reader.Read())			
					{
						if (reader.Name == Parameters.ResponseStatusCode)
						{
							reader.Read();
							status_code = reader.Value;
							break;
						}
					}
				
					if (status_code == "200")
					{
						while (reader.Read())
						{
							if (reader.Name == Parameters.ResponseUrl)
							{
								reader.Read();
								return_text = reader.Value;
								break;
							}
						}
					}
					else
					{
						return Catalog.GetString("Error: ") + Catalog.GetString("Invalid login or API key");
					}									
				}
				catch (System.Threading.ThreadAbortException ex)
				{
					throw ex;
				}
				catch (Exception ex)
				{
					Tools.PrintInfo(ex, typeof(BitLy));
					return_text = Catalog.GetString("Error: ") + ex.Message;
				}
				
				return return_text;
			}	
		}
		
		/// <summary>
		/// TinyURL.com shortener.
		/// </summary>
		public static class TinyURL
		{
			/// <summary>
			/// API URL.
			/// </summary>
			public const string ApiUrl = "http://tinyurl.com/api-create.php?url=";
			
			/// <summary>
			/// Shortens the specified url.
			/// </summary>
			/// <param name="url">URL to shorten.</param>
			/// <returns>Shortened URL or error message.</returns>
			public static string Shorten(string url)
			{
				string return_text;
				HttpWebRequest request;				
				
				try
				{					
					request = (HttpWebRequest)HttpWebRequest.Create(ApiUrl + UrlEncoder.UrlEncode(url));
					request.Method = "GET";						
					
					using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))									
					{
						return_text = reader.ReadToEnd();
					}					
					
					return_text = return_text.StartsWith("http://") ? string.Format("<a href=\"{0}\">{0}</a>", return_text) : Catalog.GetString("Error: ") + Catalog.GetString("Unspecified error.");					
				}
				catch (System.Threading.ThreadAbortException ex)
				{
					throw ex;
				}
				catch (Exception ex)
				{
					Tools.PrintInfo(ex, typeof(TinyURL));
					return_text = Catalog.GetString("Error: ") + ex.Message;
				}
				
				return return_text;	
			}
		}
	}
}
