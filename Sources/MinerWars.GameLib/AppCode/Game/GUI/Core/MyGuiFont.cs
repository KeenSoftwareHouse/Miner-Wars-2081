using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Utils;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Managers;

using MinerWarsMath;
/*
using MinerWarsMath;

using MinerWarsMath.Graphics;
  */

using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;


//  Bitmap font class for XNA

namespace MinerWars.AppCode.Game.GUI.Core
{
    using Vector2 = MinerWarsMath.Vector2;
    using Color = MinerWarsMath.Color;
    using Rectangle = MinerWarsMath.Rectangle;

    public class MyGuiFont
	{
		enum MyGlyphFlags
		{
			None = 0,
			ForceWhite = 1,		// force the drawing color for this glyph to be white.
		}

		//  Info for each glyph in the font - where to find the glyph image and other properties
		struct MyGlyphInfo
		{
			public ushort nBitmapID;
            public ushort pxLocX;
            public ushort pxLocY;
			public byte pxWidth;
			public byte pxHeight;
			public byte pxAdvanceWidth;
			public sbyte pxLeftSideBearing;
			public MyGlyphFlags nFlags;
		}

		//  Info for each font bitmap
		struct MyBitmapInfo
		{
			public string strFilename;
			public int nX, nY;
		}

        const char NEW_LINE = '\r';

        //  This is artificial spacing in between two characters (in pixels).
        //  Using it we can make spaces wider or narrower
        public int Spacing = 0;
        
		string m_strName;
		string m_strPath;
		Dictionary<int, MyBitmapInfo> m_dictBitmapID2BitmapInfo;
		Dictionary<int, MyTexture2D> m_dictBitmapID2Texture;
		Dictionary<char, MyGlyphInfo> m_dictUnicode2GlyphInfo;
		Dictionary<char, Dictionary<char, sbyte>> m_dictKern;
		int m_nBase = 0;
		int m_nHeight = 0;
		float m_fpDepth = 0.0f;

		//  Create a new font from the info in the specified font descriptor (XML) file
        public MyGuiFont(string strFontFilename)
		{
            MyMwcLog.WriteLine("MyGuiFont.MyGuiFont - START");
            MyMwcLog.IncreaseIndent();

            MyMwcLog.WriteLine("Font filename: " + strFontFilename, SysUtils.LoggingOptions.MISC_RENDER_ASSETS);
            
            Init(strFontFilename);
            Reset(MyMinerGame.Static.GraphicsDevice);

            MyMwcLog.WriteLine("Name: " + Name, SysUtils.LoggingOptions.MISC_RENDER_ASSETS);
            MyMwcLog.WriteLine("LineHeight: " + LineHeight, SysUtils.LoggingOptions.MISC_RENDER_ASSETS);
            MyMwcLog.WriteLine("Baseline: " + Baseline, SysUtils.LoggingOptions.MISC_RENDER_ASSETS);
            MyMwcLog.WriteLine("KernEnable: " + KernEnable, SysUtils.LoggingOptions.MISC_RENDER_ASSETS);

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyGuiFont.MyGuiFont - END");
        }

		void Init(string strFontFilename)
		{
			m_dictBitmapID2BitmapInfo = new Dictionary<int, MyBitmapInfo>();
			m_dictBitmapID2Texture = new Dictionary<int, MyTexture2D>();

			m_dictUnicode2GlyphInfo = new Dictionary<char, MyGlyphInfo>();
			m_dictKern = new Dictionary<char, Dictionary<char, sbyte>>();

			XmlDocument xd = new XmlDocument();
			
            string fontXmlFilename = MyMinerGame.Static.RootDirectory + "\\" + strFontFilename;

			//  load the XML file from wherever it is - filesystem or embedded
            if (System.IO.File.Exists(fontXmlFilename))
            {
                //  all files mentioned in the font descriptor file are relative to the parent directory of the font file.
                //  record the path to this directory.
                m_strPath = System.IO.Path.GetDirectoryName(strFontFilename);
                if (m_strPath != "") m_strPath += @"/";

                xd.Load(fontXmlFilename);   
            }
            else
            {
		        throw new Exception(String.Format("Unable to find font named '{0}'.", strFontFilename));
			}

			m_strName = "";

			LoadFontXML(xd.ChildNodes);

			//  if the font doesn't define a name, create one from the filename
			if (m_strName == "")
				m_strName = System.IO.Path.GetFileNameWithoutExtension(strFontFilename);

			//  add this font to the list of active fonts
		}

		void ConvertFilePath2EmbeddedPath(string strFilePath, out string strEmbeddedPath, out string strEmbeddedName)
		{
			Assembly a = Assembly.GetExecutingAssembly();
			// calc the resource path to this font (strip off <fontname>.xml)
			strEmbeddedPath = a.GetName().Name + ".";

			// strip directory nams from the filepath and add them to the embedded path
			string[] aPath = strFilePath.Split(new char[] { '/', '\\' });
			for (int i = 0; i < aPath.Length - 1; i++)
				strEmbeddedPath += aPath[i] + ".";
			strEmbeddedName = aPath[aPath.Length-1];
		}

		//  Reset the font when the device has changed
		void Reset(Device device)
		{
			foreach (KeyValuePair<int, MyBitmapInfo> kv in m_dictBitmapID2BitmapInfo)
			{
				MyTexture2D tex = MyTextureManager.GetTexture<MyTexture2D>(m_strPath + kv.Value.strFilename, null, LoadingMode.Immediate, TextureFlags.IgnoreQuality);

                m_dictBitmapID2Texture[kv.Key] = tex;
			}
		}

		//  The name of this font
		public string Name
		{
			get { return m_strName; }
		}

		//  Should we kern adjacent characters?
		bool m_fKern = true;

		//  Enable/disable kerning
		public bool KernEnable
		{
			get { return m_fKern; }
			set { m_fKern = value; }
		}

		//  Distance from top of font to the baseline
		public int Baseline
		{
			get { return m_nBase; }
		}

		//  Distance from top to bottom of the font
		public int LineHeight
		{
			get { return m_nHeight; }
		}

		//  The depth at which to draw the font
		public float Depth
		{
			get { return m_fpDepth; }
			set { m_fpDepth = value; }
		}

		//  Current pen position
		Vector2 m_vPen = new Vector2(0, 0);

		//  Current pen position
		public Vector2 Pen
		{
			get { return m_vPen; }
			set { m_vPen = value; }
		}

		//  Set the current pen position
		public void SetPen(int x, int y)
		{
			m_vPen = new Vector2(x, y);
		}

		//  Current color used for drawing text
		Color m_color = Color.White;

		//  Current color used for drawing text
		public Color TextColor
		{
			get { return m_color; }
			set { m_color = value; }
		}

		//  Draw the given string at (x,y).
		//  The text color is inherited from the last draw command (default=White).
		//  Returns: Width of string (in pixels)
        public float DrawString(int x, int y, StringBuilder text, float scale)
		{
			Vector2 v = new Vector2(x, y);
            return DrawString(v, m_color, text, scale);
		}

		//  Draw the given string at (x,y) using the specified color
		//  Returns: Width of string (in pixels)
        public float DrawString(int x, int y, Color color, StringBuilder text, float scale)
		{
			Vector2 v = new Vector2(x, y);
			return DrawString(v, color, text, scale);
		}

		//  Draw the given string using the specified color.
		//  The text drawing location is immediately after the last drawn text (default=0,0).
		//  Returns: Width of string (in pixels)
        public float DrawString(Color color, StringBuilder text, float scale)
		{
			return DrawString(m_vPen, color, text, scale);
		}

		//  Draw the given string at (x,y).
		//  The text drawing location is immediately after the last drawn text (default=0,0).
		//  The text color is inherited from the last draw command (default=White).
		//  Returns: Width of string (in pixels)
        public float DrawString(StringBuilder text, float scale)
		{
			return DrawString(m_vPen, m_color, text, scale);
		}

		//  Draw the given string at vOrigin using the specified color
		//  Returns: Width of string (in pixels)
        public float DrawString(Vector2 vAt, Color cText, StringBuilder text, float scale)
		{
			return DrawString_internal(vAt, cText, text, scale);
		}

		//  Calculate the width of the given string.
		//  Returns: Width and height (in pixels) of the string
        public Vector2 MeasureString(StringBuilder text, float scale)
		{
            scale *= MyGuiConstants.FONT_SCALE;
            float pxWidth = 0;
			char cLast = '\0';

            float maxPxWidth = 0;
            int lines = 1;
            for (int i = 0; i < text.Length; i++)
			{
                char c = text[i];

                //  New line
                if (c == NEW_LINE)
                {
                    lines++;
                    pxWidth = 0;
                    cLast = '\0';
                    continue;
                }

                if (!m_dictUnicode2GlyphInfo.ContainsKey(c))
				{
					continue;
				}

                MyGlyphInfo ginfo = m_dictUnicode2GlyphInfo[c];

                // if kerning is enabled, get the kern adjustment for this char pair
                if (m_fKern)
                {
                    pxWidth += CalcKern(cLast, c);
                    cLast = c;
                }

                //  update the string width
                pxWidth += ginfo.pxAdvanceWidth;

                //  Spacing
                if (i < (text.Length - 1)) pxWidth += Spacing;

                //  Because new line
                if (pxWidth > maxPxWidth) maxPxWidth = pxWidth;
			}

            return new Vector2(maxPxWidth * scale, lines * LineHeight * scale);
		}

		//  Private version of DrawString that expects the string to be formatted already
		//  Returns: Width of string (in pixels)
        float DrawString_internal(Vector2 vAtOriginal, Color cText, StringBuilder text, float scale)
		{
            scale *= MyGuiConstants.FONT_SCALE;
            Color currentColor = cText;
            Vector2 vOrigin = new Vector2(0,0);
			float pxWidth = 0;
			char cLast = '\0';

            Vector2 vAt = vAtOriginal;

            float spacingScaled = Spacing * scale;

            int line = 0;

			//  Draw each character in the string
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                if (c == NEW_LINE)
                {
                    vAt.X = vAtOriginal.X;
                    line++;
                    continue;
                }

                if (!m_dictUnicode2GlyphInfo.ContainsKey(c))
				{
					continue;
				}
            
                MyGlyphInfo ginfo = m_dictUnicode2GlyphInfo[c];

                //  If kerning is enabled, get the kern adjustment for this char pair
                if (m_fKern)
                {
                    int pxKern = CalcKern(cLast, c);
                    vAt.X += pxKern * scale;
                    pxWidth += pxKern * scale;
                    cLast = c;
                }

                //  This will fix vertical coordinate in case we use "gpad" - left/top blank space in every character
                vAt.Y = vAtOriginal.Y + (ginfo.pxLeftSideBearing + MyGuiConstants.FONT_TOP_SIDE_BEARING + line * LineHeight) * scale;

                //  Draw the glyph
                vAt.X += ginfo.pxLeftSideBearing * scale;
                if (ginfo.pxWidth != 0 && ginfo.pxHeight != 0)
                {
                    Rectangle rSource = new Rectangle(ginfo.pxLocX, ginfo.pxLocY, ginfo.pxWidth, ginfo.pxHeight);
                    Color color = (((ginfo.nFlags & MyGlyphFlags.ForceWhite) != 0) ? Color.White : currentColor);
                    MyGuiManager.DrawSpriteBatch(m_dictBitmapID2Texture[ginfo.nBitmapID], vAt, rSource, color, 0.0f, vOrigin, scale, SpriteEffects.None, m_fpDepth);
                }

                // update the string width and advance the pen to the next drawing position
                pxWidth += ginfo.pxAdvanceWidth * scale;
                vAt.X += (ginfo.pxAdvanceWidth - ginfo.pxLeftSideBearing) * scale;

                //  Spacing
                if (i < (text.Length - 1))
                {
                    pxWidth += spacingScaled;
                    vAt.X += spacingScaled;
                }
			}

			//  Record final pen position and color
			m_vPen = vAt;
			m_color = cText;

			return pxWidth;
		}

		//  Get the kern value for the given pair of characters
		//  Returns: Amount to kern (in pixels)
		int CalcKern(char chLeft, char chRight)
		{
			if (m_dictKern.ContainsKey(chLeft))
			{
				Dictionary<char, sbyte> kern2 = m_dictKern[chLeft];
				if (kern2.ContainsKey(chRight))
					return kern2[chRight];
			}
			return 0;
		}        	

		/// <summary>
		/// Load the font data from an XML font descriptor file
		/// </summary>
		/// <param name="xnl">XML node list containing the entire font descriptor file</param>
		void LoadFontXML(XmlNodeList xnl)
		{
			foreach (XmlNode xn in xnl)
			{
				if (xn.Name == "font")
				{
					m_strName = GetXMLAttribute(xn, "name");
					m_nBase = Int32.Parse(GetXMLAttribute(xn, "base"));
					m_nHeight = Int32.Parse(GetXMLAttribute(xn, "height"));

					LoadFontXML_font(xn.ChildNodes);
				}
			}
		}

		/// <summary>
		/// Load the data from the "font" node
		/// </summary>
		/// <param name="xnl">XML node list containing the "font" node's children</param>
		void LoadFontXML_font(XmlNodeList xnl)
		{
			foreach (XmlNode xn in xnl)
			{
				if (xn.Name == "bitmaps")
					LoadFontXML_bitmaps(xn.ChildNodes);
				if (xn.Name == "glyphs")
					LoadFontXML_glyphs(xn.ChildNodes);
				if (xn.Name == "kernpairs")
					LoadFontXML_kernpairs(xn.ChildNodes);
			}
		}

		/// <summary>
		/// Load the data from the "bitmaps" node
		/// </summary>
		/// <param name="xnl">XML node list containing the "bitmaps" node's children</param>
		void LoadFontXML_bitmaps(XmlNodeList xnl)
		{
			foreach (XmlNode xn in xnl)
			{
				if (xn.Name == "bitmap")
				{
					string strID = GetXMLAttribute(xn, "id");
					string strFilename = GetXMLAttribute(xn, "name");
					string strSize = GetXMLAttribute(xn, "size");
					string[] aSize = strSize.Split('x');
					
					// if the MyCustomContentManager is being used, then we may need to strip off the .png extension
					// to generate the correct asset name.
					if (strFilename.EndsWith(@".png"))
						strFilename = strFilename.Remove(strFilename.Length - 4, 4);

					MyBitmapInfo bminfo;
					bminfo.strFilename = strFilename;
					bminfo.nX = Int32.Parse(aSize[0]);
					bminfo.nY = Int32.Parse(aSize[1]);

					m_dictBitmapID2BitmapInfo[Int32.Parse(strID)] = bminfo;
				}
			}
		}

		/// <summary>
		/// Load the data from the "glyphs" node
		/// </summary>
		/// <param name="xnl">XML node list containing the "glyphs" node's children</param>
		void LoadFontXML_glyphs(XmlNodeList xnl)
		{
			foreach (XmlNode xn in xnl)
			{
				if (xn.Name == "glyph")
				{
					string strChar = GetXMLAttribute(xn, "ch");
					string strBitmapID = GetXMLAttribute(xn, "bm");
					string strLoc = GetXMLAttribute(xn, "loc");
					string strSize = GetXMLAttribute(xn, "size");
					string strAW = GetXMLAttribute(xn, "aw");
					string strLSB = GetXMLAttribute(xn, "lsb");
					string strForceWhite = GetXMLAttribute(xn, "forcewhite");

					if (strLoc == "")
						strLoc = GetXMLAttribute(xn, "origin");	// obsolete - use loc instead

					string[] aLoc = strLoc.Split(',');
					string[] aSize = strSize.Split('x');

					MyGlyphInfo ginfo = new MyGlyphInfo();
					ginfo.nBitmapID = UInt16.Parse(strBitmapID);
					ginfo.pxLocX = ushort.Parse(aLoc[0]);
                    ginfo.pxLocY = ushort.Parse(aLoc[1]);
					ginfo.pxWidth = Byte.Parse(aSize[0]);
					ginfo.pxHeight = Byte.Parse(aSize[1]);
					ginfo.pxAdvanceWidth = Byte.Parse(strAW);
					ginfo.pxLeftSideBearing = SByte.Parse(strLSB);
					ginfo.nFlags = 0;
					ginfo.nFlags |= (strForceWhite == "true" ? MyGlyphFlags.ForceWhite : MyGlyphFlags.None);

					m_dictUnicode2GlyphInfo[strChar[0]] = ginfo;
				}
			}
		}

		/// <summary>
		/// Load the data from the "kernpairs" node
		/// </summary>
		/// <param name="xnl">XML node list containing the "kernpairs" node's children</param>
		void LoadFontXML_kernpairs(XmlNodeList xnl)
		{
			foreach (XmlNode xn in xnl)
			{
				if (xn.Name == "kernpair")
				{
					string strLeft = GetXMLAttribute(xn, "left");
					string strRight = GetXMLAttribute(xn, "right");
					string strAdjust = GetXMLAttribute(xn, "adjust");

					char chLeft = strLeft[0];
					char chRight = strRight[0];

					// create a kern dict for the left char if needed
					if (!m_dictKern.ContainsKey(chLeft))
						m_dictKern[chLeft] = new Dictionary<char,sbyte>();

					// add the right char to the left char's kern dict
					Dictionary<char, sbyte> kern2 = m_dictKern[chLeft];
					kern2[chRight] = SByte.Parse(strAdjust);
				}
			}
		}

		/// <summary>
		/// Get the XML attribute value
		/// </summary>
		/// <param name="n">XML node</param>
		/// <param name="strAttr">Attribute name</param>
		/// <returns>Attribute value, or the empty string if the attribute doesn't exist</returns>
		static string GetXMLAttribute(XmlNode n, string strAttr)
		{
			XmlAttribute attr = n.Attributes.GetNamedItem(strAttr) as XmlAttribute;
			if (attr != null)
				return attr.Value;
			return "";
		}		
	}
}