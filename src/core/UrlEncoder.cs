// 
// System.Web.HttpUtility
//
// Authors:
//   Patrik Torstensson (Patrik.Torstensson@labs2.com)
//   Wictor Wilén (decode/encode functions) (wictor@ibizkit.se)
//   Tim Coleman (tim@timcoleman.com)
//   Gonzalo Paniagua Javier (gonzalo@ximian.com)
//
// Copyright (C) 2005 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
namespace Glippy.Core
{
	/// <summary>
	/// Part of System.Web.HttpUtility class.
	/// </summary>
	public static class UrlEncoder
	{
		private static char [] hexChars = "0123456789abcdef".ToCharArray ();
		
		public static string UrlEncode(string s) 
		{
			if (s == null)
				return null;

			if (s == "")
				return "";

			bool needEncode = false;
			int len = s.Length;
			for (int i = 0; i < len; i++) {
				char c = s [i];
				if ((c < '0') || (c < 'A' && c > '9') || (c > 'Z' && c < 'a') || (c > 'z')) {
					if (NotEncoded (c))
						continue;

					needEncode = true;
					break;
				}
			}

			if (!needEncode)
				return s;
			
			byte [] bytes = new byte[System.Text.Encoding.UTF8.GetMaxByteCount(s.Length)];
			int realLen = System.Text.Encoding.UTF8.GetBytes (s, 0, s.Length, bytes, 0);
			return System.Text.Encoding.UTF8.GetString(UrlEncodeToBytes(bytes, 0, realLen));
		}
		
		public static byte [] UrlEncodeToBytes (byte [] bytes, int offset, int count)
		{
			if (bytes == null)
				return null;

			int len = bytes.Length;
			if (len == 0)
				return new byte [0];

			if (offset < 0 || offset >= len)
				throw new System.ArgumentOutOfRangeException("offset");

			if (count < 0 || count > len - offset)
				throw new System.ArgumentOutOfRangeException("count");

			System.IO.MemoryStream result = new System.IO.MemoryStream (count);
			int end = offset + count;
			for (int i = offset; i < end; i++)
				UrlEncodeChar ((char)bytes [i], result, false);

			return result.ToArray();
		}
		
		private static bool NotEncoded(char c)
		{
			return (c == '!' || c == '\'' || c == '(' || c == ')' || c == '*' || c == '-' || c == '.' || c == '_');
		}
		
		private static void UrlEncodeChar(char c, System.IO.Stream result, bool isUnicode)
		{
			if (c > 255) {
				int idx;
				int i = (int) c;
	
				result.WriteByte ((byte)'%');
				result.WriteByte ((byte)'u');
				idx = i >> 12;
				result.WriteByte ((byte)hexChars [idx]);
				idx = (i >> 8) & 0x0F;
				result.WriteByte ((byte)hexChars [idx]);
				idx = (i >> 4) & 0x0F;
				result.WriteByte ((byte)hexChars [idx]);
				idx = i & 0x0F;
				result.WriteByte ((byte)hexChars [idx]);
				return;
			}
			
			if (c > ' ' && NotEncoded (c)) {
				result.WriteByte ((byte)c);
				return;
			}
			if (c==' ') {
				result.WriteByte ((byte)'+');
				return;
			}
			if (	(c < '0') ||
				(c < 'A' && c > '9') ||
				(c > 'Z' && c < 'a') ||
				(c > 'z')) {
				if (isUnicode && c > 127) {
					result.WriteByte ((byte)'%');
					result.WriteByte ((byte)'u');
					result.WriteByte ((byte)'0');
					result.WriteByte ((byte)'0');
				}
				else
					result.WriteByte ((byte)'%');
				
				int idx = ((int) c) >> 4;
				result.WriteByte ((byte)hexChars [idx]);
				idx = ((int) c) & 0x0F;
				result.WriteByte ((byte)hexChars [idx]);
			}
			else
				result.WriteByte ((byte)c);
		}					
	}
}
