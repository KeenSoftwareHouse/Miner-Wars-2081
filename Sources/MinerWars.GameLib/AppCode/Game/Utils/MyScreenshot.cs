using System;
using System.IO;
using System.Collections.Generic;
//using MinerWarsMath.Graphics;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using System.Drawing;

using SharpDX.Direct3D9;
using SharpDX;
using SharpDX.Toolkit;

//  Screenshot object survives only one DRAW after created. We delete it immediatelly. So if 'm_screenshot'
//  is not null we know we have to take screenshot and set it to null.
//  All files are saved under same date/time names

namespace MinerWars.AppCode.Game.Utils
{
    class MyScreenshot
    {
        readonly string m_folder;
        readonly string m_datetimePrefix;
        float m_sizeMultiplier = 1.0f;

        public MyScreenshot(float sizeMultiplier)
        {
            MyMwcLog.WriteLine("MyScreenshot.Constructor() - START");
            MyMwcLog.IncreaseIndent();

            System.Diagnostics.Debug.Assert(sizeMultiplier > 0.0f);
            m_sizeMultiplier = sizeMultiplier;

            m_folder = Path.Combine(MyFileSystemUtils.GetApplicationUserDataFolder(), "Screenshots\\");
            m_datetimePrefix = MyValueFormatter.GetFormatedDateTimeForFilename(DateTime.Now);

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyScreenshot.Constructor() - END");
        }

        public string GetFilename(string name)
        {
            return Path.Combine(m_folder, "MinerWars_" + MyEnumsToStrings.CameraDirection[(int)MyCamera.ActualCameraDirection] + "_" + m_datetimePrefix + "_" + name);
        }

        public static void SaveScreenshotUserFolder(Texture texture2D, string name)
        {
            var datetimePrefix = MyValueFormatter.GetFormatedDateTimeForFilename(DateTime.Now);
            var folder = Path.Combine(MyFileSystemUtils.GetApplicationUserDataFolder(), "Screenshots\\");
            var path = Path.Combine(folder, "MinerWars_" + MyEnumsToStrings.CameraDirection[(int)MyCamera.ActualCameraDirection] + "_" + datetimePrefix + "_" + name);
            SaveScreenshot(texture2D, path);
        }

        public static void SaveScreenshot(Texture texture2D, string file)
        {      
            MyMwcLog.WriteLine("MyScreenshot.SaveTexture2D() - START");
            MyMwcLog.IncreaseIndent();

            Texture systemTex =  new Texture(MinerWars.AppCode.App.MyMinerGame.Static.GraphicsDevice, texture2D.GetLevelDescription(0).Width, texture2D.GetLevelDescription(0).Height, 0, Usage.None, Format.A8R8G8B8, Pool.SystemMemory);


            Surface sourceSurface = texture2D.GetSurfaceLevel(0);
            Surface destSurface = systemTex.GetSurfaceLevel(0);
            MinerWars.AppCode.App.MyMinerGame.Static.GraphicsDevice.GetRenderTargetData(sourceSurface, destSurface);
            sourceSurface.Dispose();
            destSurface.Dispose();

            texture2D = systemTex;

            try
            {
                MyMwcLog.WriteLine("File: " + file);

                MyFileSystemUtils.CreateFolderForFile(file);

                Stack<SharpDX.Rectangle> tiles = new Stack<SharpDX.Rectangle>();

                int tileWidth = texture2D.GetLevelDescription(0).Width;
                int tileHeight = texture2D.GetLevelDescription(0).Height;

                while (tileWidth > 3200)
                {
                    tileWidth /= 2;
                    tileHeight /= 2;
                }

                int widthOffset = 0;
                int heightOffset = 0;

                while (widthOffset < texture2D.GetLevelDescription(0).Width)
                {
                    while (heightOffset < texture2D.GetLevelDescription(0).Height)
                    {
                        tiles.Push(new SharpDX.Rectangle(widthOffset, heightOffset, tileWidth, tileHeight));
                        heightOffset += tileHeight;
                    }

                    heightOffset = 0;
                    widthOffset += tileWidth;
                }

                int sc = 0;
                while (tiles.Count > 0)
                {
                    SharpDX.Rectangle rect = tiles.Pop();

                    byte[] data = new byte[rect.Width * rect.Height * 4];
                    SharpDX.Rectangle rect2 = new SharpDX.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
                    //texture2D.GetData<byte>(0, rect2, data, 0, data.Length);
                    DataStream ds;
                    texture2D.LockRectangle(0, rect2, LockFlags.None, out ds); 

                    ds.Read(data, 0, data.Length);
                            /*
                    for (int i = 0; i < data.Length; i += 4)
                    {
                        //Swap ARGB <-> RGBA
                        byte b = data[i + 0];
                        byte g = data[i + 1];
                        byte r = data[i + 2];
                        byte a = data[i + 3];
                        data[i + 0] = r;  //Blue
                        data[i + 1] = g; //Green
                        data[i + 2] = b; //Red
                        data[i + 3] = a; //Alpha
                    }         */

                    ds.Seek(0, SeekOrigin.Begin);
                    ds.WriteRange(data);

                    texture2D.UnlockRectangle(0);

                    string filename = file.Replace(".png", "_" + sc.ToString("##00") + ".png");
                    using (Stream stream = File.Create(filename))
                    {        
                        System.Drawing.Bitmap image = new System.Drawing.Bitmap(rect.Width, rect.Height);

                        System.Drawing.Imaging.BitmapData imageData = image.LockBits(new System.Drawing.Rectangle(0,0,rect.Width, rect.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        System.Runtime.InteropServices.Marshal.Copy(data, 0, imageData.Scan0, data.Length);

                        image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

                        image.UnlockBits(imageData);
                        image.Dispose();
                               
                        //texture2D.SaveAsPng(stream, texture2D.Width, texture2D.Height);
                        //BaseTexture.ToStream(texture2D, ImageFileFormat.Png);
                    }

                    sc++;
                    GC.Collect();
                }
            }
            catch (Exception exc)
            {
                //  Write exception to log, but continue as if nothing wrong happened
                MyMwcLog.WriteLine(exc);
            }

            texture2D.Dispose();

            //BaseTexture.ToFile(texture2D, "c:\\test.png", ImageFileFormat.Png);

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyScreenshot.SaveTexture2D() - END");   
        }

        //  Failure while saving will not crash the game, only log an exception into log file
        public void SaveTexture2D(Texture texture2D, string name)
        {
            SaveScreenshot(texture2D, GetFilename(name + ".png"));
        }

        public float SizeMultiplier
        {
            get { return m_sizeMultiplier; }
        }
    }
}
