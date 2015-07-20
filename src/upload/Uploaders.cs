/* 
 * Glippy
 * Copyright © 2010, 2011, 2012 Wojciech Kowalczyk
 * The program is distributed under the terms of the GNU General Public License Version 3.
 * See LICENCE for details.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using Glippy.Core;
using Mono.Unix;
	
namespace Glippy.Upload
{	
	/// <summary>
	/// Uploaders.
	/// </summary>
	internal static class Uploaders
	{
		/// <summary>
		/// Pastebin text uploader.
		/// </summary>
		public static class Pastebin
		{
			/// <summary>
			/// Pastebin API URL.
			/// </summary>
			public const string ApiUrl = "http://pastebin.com/api/api_post.php";									
			
			/// <summary>
			/// Pastebin login membership API URL.
			/// </summary>
			public const string UserApiUrl = "http://pastebin.com/api/api_login.php";									
			
			/// <summary>
			/// Pastebin API key.
			/// </summary>
			public const string Key = "d82e0944233b0271fc3574c48b30f607";
			
			/// <summary>
			/// Pastebin parameters.
			/// </summary>
			public static class Parameters
			{
				public const string ParamOption = "api_option";
				public const string ParamDevKey = "api_dev_key";
				public const string ParamCode = "api_paste_code";
				public const string ParamName = "api_paste_name";
				public const string ParamPrivate = "api_paste_private";
				public const string ParamExpireDate = "api_paste_expire_date";
				public const string ParamFormat = "api_paste_format";			
				public const string ParamUserKey = "api_user_key";
				public const string ParamUserName = "api_user_name";
				public const string ParamUserPassword = "api_user_password";
				public const string ExpireDateNever = "N";
				public const string ExpireDate10Minutes = "10M";
				public const string ExpireDate1Hour = "1H";
				public const string ExpireDate1Day = "1D";
				public const string ExpireDate1Month = "1M";						
			}
			
			/// <summary>
			/// Uploads text and returns link.
			/// </summary>
			/// <param name="text">Text to upload.</param>
			/// <param name="name">Uploader name.</param>
			/// <param name="privacy">Privacy level of paste.</param>
			/// <param name="expireDate">Time after which upload is deleted.</param>			
			/// <param name="userKey">User authentication key.</param>
			/// <returns>Link on pastebin.</returns>
			public static string Upload(string text, string name, int privacy, string expireDate, string userKey)
			{					
				string return_text;
				byte[] post_content;
				HttpWebRequest request;				
				
				StringBuilder post_builder = new StringBuilder();
				post_builder.Append(Parameters.ParamOption).Append("=").Append("paste");				
				post_builder.Append("&");
				post_builder.Append(Parameters.ParamDevKey).Append("=").Append(UrlEncoder.UrlEncode(Key));				
				post_builder.Append("&");
				post_builder.Append(Parameters.ParamCode).Append("=").Append(UrlEncoder.UrlEncode(text));				
				post_builder.Append("&");
				post_builder.Append(Parameters.ParamName).Append("=").Append(UrlEncoder.UrlEncode(name));
				post_builder.Append("&");
				post_builder.Append(Parameters.ParamPrivate).Append("=").Append(privacy);
				post_builder.Append("&");
				post_builder.Append(Parameters.ParamExpireDate).Append("=").Append(expireDate);
				
				if (!string.IsNullOrWhiteSpace(userKey))
				{
					post_builder.Append("&");
					post_builder.Append(Parameters.ParamUserKey).Append("=").Append(userKey);
				}
				    				
				post_content = Encoding.ASCII.GetBytes(post_builder.ToString());
					
				try
				{					
					request = (HttpWebRequest)HttpWebRequest.Create(ApiUrl);
					request.Method = "POST";						
					request.ContentLength = post_content.Length;		
					request.ContentType = "application/x-www-form-urlencoded";
					
					using (Stream writer = request.GetRequestStream())				
					{
						writer.Write(post_content, 0, post_content.Length);				
					}
					
					using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))								
					{
						return_text = reader.ReadToEnd();	
					}
					
					return_text = string.Format("<a href=\"{0}\">{0}</a>", return_text);
				}
				catch (System.Threading.ThreadAbortException ex)
				{
					throw ex;
				}
				catch (Exception ex)
				{
					Tools.PrintInfo(ex, typeof(Pastebin));
					return_text = Catalog.GetString("Error: ") + ex.Message;
				}
				
				return return_text;				
			}	
			
			/// <summary>
			/// Gets the user API key.
			/// </summary>
			/// <param name="username">Username.</param>
			/// <param name="password">Password.</param>
			/// <returns>User key or error message.</returns>			
			public static string GetUserKey(string username, string password)
			{
				string return_text;
				byte[] post_content;
				HttpWebRequest request;				
				
				StringBuilder post_builder = new StringBuilder();
				post_builder.Append(Parameters.ParamDevKey).Append("=").Append(UrlEncoder.UrlEncode(Key));				
				post_builder.Append("&");
				post_builder.Append(Parameters.ParamUserName).Append("=").Append(UrlEncoder.UrlEncode(username));				
				post_builder.Append("&");
				post_builder.Append(Parameters.ParamUserPassword).Append("=").Append(UrlEncoder.UrlEncode(password));
				post_content = Encoding.ASCII.GetBytes(post_builder.ToString());
					
				try
				{					
					request = (HttpWebRequest)HttpWebRequest.Create(UserApiUrl);
					request.Method = "POST";						
					request.ContentLength = post_content.Length;		
					request.ContentType = "application/x-www-form-urlencoded";
					
					using (Stream writer = request.GetRequestStream())				
					{
						writer.Write(post_content, 0, post_content.Length);				
					}
					
					using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))								
					{
						return_text = reader.ReadToEnd();	
					}									
				}
				catch (System.Threading.ThreadAbortException ex)
				{
					throw ex;
				}
				catch (Exception ex)
				{
					Tools.PrintInfo(ex, typeof(Pastebin));
					return_text = Catalog.GetString("Error: ") + ex.Message;
				}
				
				return return_text;
			}
		}
		
		/// <summary>
		/// Imgur uploader.
		/// </summary>
		public static class Imgur
		{				
			/// <summary>
			/// Imgur API URL.
			/// </summary>
			public const string Url = "http://imgur.com/api/upload.xml";
			
			/// <summary>
			/// API key (got from some bash script, sorry!).
			/// </summary>
			private const string Key = "5d317f0bee23b282473522e1aa68f621";
			
			/// <summary>
			/// Dunno WTF, if bigger, imgur screams INTERNAL ERROR AAAA!!1111
			/// </summary>
			private const int MaxContentLength = 2675843;
			
			/// <summary>
			/// Imgur parameters.
			/// </summary>
			public static class Parameters
			{
				public const string ParamKey = "key";
				public const string ParamImage = "image";
				public const string ResponseState = "stat";
				public const string ResponseOriginalImage = "original_image";
				public const string ResponseLargeThumbnail = "large_thumbnail";
				public const string ResponseSmallThumbnail = "small_thumbnail";
				public const string ResponseImgurPage = "imgur_page";
				public const string ResponseDeletePage= "delete_page";		
				public const string ResponseErrorCode = "error_code";
				public const string ResponseErrorMessage = "error_msg";
			}
			
			/// <summary>
			/// Uploads image and returns dictionary of received parameters.
			/// </summary>
			/// <param name="img">Image to upload.</param>
			/// <returns>Dictionary with response parameters.</returns>
			public static Dictionary<string, string> Upload(Gdk.Pixbuf img)
			{				
				byte[] post_content;
				HttpWebRequest request;
				int basic_length;				
				Dictionary<string, string> response = new Dictionary<string, string>();
				
				StringBuilder post_builder = new StringBuilder();
				post_builder.Append(Parameters.ParamKey).Append("=").Append(Key);				
				post_builder.Append("&");				
				post_builder.Append(Parameters.ParamImage).Append("=");	
				basic_length = post_builder.Length;
				post_builder.Append(UrlEncoder.UrlEncode(Convert.ToBase64String(img.SaveToBuffer("png"))));			
				post_content = Encoding.UTF8.GetBytes(post_builder.ToString());				
				
				if (post_content.Length > MaxContentLength)
				{
					post_builder.Remove(basic_length, post_builder.Length - basic_length);
					post_builder.Append(UrlEncoder.UrlEncode(Convert.ToBase64String(img.SaveToBuffer("jpeg"))));					
					post_content = Encoding.UTF8.GetBytes(post_builder.ToString());
					
					if (post_content.Length > MaxContentLength)
					{
						response.Add("FATAL", Catalog.GetString("Image size is too large."));
						return response;
					}
				}
				
				try
				{
					request = (HttpWebRequest)HttpWebRequest.Create(Url);
					request.Method = "POST";				
					request.ContentLength = post_content.Length;							
					request.ContentType = "application/x-www-form-urlencoded";												
					
					using (Stream writer = request.GetRequestStream())				
					{
						writer.Write(post_content, 0, post_content.Length);				
					}
									
					using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))									
					{
						using (XmlTextReader xmlreader = new XmlTextReader(reader))						
						{
							ParseResponseXml(response, xmlreader);
						}
					}					
				}
				catch (System.Threading.ThreadAbortException ex)
				{
					throw ex;
				}
				catch (Exception ex)
				{
					Tools.PrintInfo(ex, typeof(Imgur));
					response.Add("FATAL", ex.Message);
				}								
				
				return response;
			}						
		
			/// <summary>
			/// Parses Imgur's XML response.
			/// </summary>
			/// <param name="response">Dictionary where parsed data id added.</param>
			/// <param name="reader">XML reader.</param>
			private static void ParseResponseXml(Dictionary<string, string> response, XmlTextReader reader)
			{								
				try
				{
					reader.Read();
					
					while (reader.Read())			
					{
						if (reader.Name == "rsp")
						{
							response.Add(Catalog.GetString("State"), reader.GetAttribute(0));					
							break;
						}
					}
				
					if (response[Catalog.GetString("State")] == "ok")			
					{
						while (reader.Read())
						{
							if (reader.Name == Parameters.ResponseOriginalImage)
							{
								reader.Read();
								response.Add(Catalog.GetString("Original image"), string.Format("<a href=\"{0}\">{0}</a>", reader.Value));
								reader.Read();
							}
							else if (reader.Name == Parameters.ResponseLargeThumbnail)
							{
								reader.Read();
								response.Add(Catalog.GetString("Large thumbnail"), string.Format("<a href=\"{0}\">{0}</a>", reader.Value));
								reader.Read();
							}
							else if (reader.Name == Parameters.ResponseSmallThumbnail)
							{
								reader.Read();
								response.Add(Catalog.GetString("Small thumbnail"), string.Format("<a href=\"{0}\">{0}</a>", reader.Value));
								reader.Read();
							}
							else if (reader.Name == Parameters.ResponseImgurPage)
							{
								reader.Read();
								response.Add(Catalog.GetString("Imgur page"), string.Format("<a href=\"{0}\">{0}</a>", reader.Value));
								reader.Read();
							}
							else if (reader.Name == Parameters.ResponseDeletePage)
							{
								reader.Read();							
								response.Add(Catalog.GetString("Delete page"), string.Format("<a href=\"{0}\">{0}</a>", reader.Value));
								reader.Read();
							}
						}			
					}
					else
					{
						while (reader.Read())
						{
							if (reader.Name == Parameters.ResponseErrorCode)
							{
								reader.Read();
								response.Add(Catalog.GetString("Error code"), reader.Value);
								reader.Read();
							}
							else if (reader.Name == Parameters.ResponseErrorMessage)
							{
								reader.Read();
								response.Add(Catalog.GetString("Error message"), reader.Value);
								reader.Read();
							}
						}
					}
				}
				catch (System.Threading.ThreadAbortException ex)
				{
					throw ex;
				}
				catch (Exception ex)
				{
					Tools.PrintInfo(ex, typeof(Imgur));
					response.Clear();					
					response.Add("FATAL", ex.Message);
				}
			}										
		}		
	}		
}
