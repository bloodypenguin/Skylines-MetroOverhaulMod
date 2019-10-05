using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using static ColossalFramework.UI.UIButton;

namespace MetroOverhaul.UI
{
    class UIHelper
    {
        public static readonly Texture2D HeaderIconTexture;
        public static readonly Texture2D InfoToolTips;
        public static readonly Texture2D QuadMetroTracks;
        public static readonly Texture2D DualMetroTracks;
        public static readonly Texture2D SingleMetroTracks;
        public static readonly Texture2D ModernStyle;
        public static readonly Texture2D ClassicStyle;
        public static readonly Texture2D VanillaStyle;
        public static readonly Texture2D TwowayDirection;
        public static readonly Texture2D OnewayDirection;
        public static readonly Texture2D Checkbox;
        public static readonly Texture2D WideMedianPillar;
        public static readonly Texture2D WidePillar;
        public static readonly Texture2D NarrowMedianPillar;
        public static readonly Texture2D NarrowPillar;
        public static readonly Texture2D SteamTab;
        public static readonly Texture2D MetroTab;
        public static readonly Texture2D TrainTab;
        public static readonly Texture2D MetroTrackToggle;
        public static readonly Texture2D TrainTrackToggle;
        public static readonly Texture2D SidePlatformStationTrack;
        public static readonly Texture2D IslandPlatformStationTrack;
        public static readonly Texture2D SinglePlatformStationTrack;
        public static readonly Texture2D ExpressSidePlatformStationTrack;
        public static readonly Texture2D DualIslandPlatformStationTrack;

        static UIHelper()
        {
            HeaderIconTexture = LoadDllResource("Header-Icon.png", 40, 40);
            HeaderIconTexture.name = "MOM_MainHeaderIcon";
            InfoToolTips = LoadDllResource("InfoToolTips.png", 482, 134);
            QuadMetroTracks = LoadDllResource("Thumbs.Menu.QuadThumbnails.png", 545, 100);
            DualMetroTracks = LoadDllResource("Thumbs.Menu.DualThumbnails.png", 545, 100);
            SingleMetroTracks = LoadDllResource("Thumbs.Menu.SingleThumbnails.png", 545, 100);
            ModernStyle = LoadDllResource("Thumbs.Button.ModernThumbnails.png", 295, 52);
            ClassicStyle = LoadDllResource("Thumbs.Button.ClassicThumbnails.png", 295, 52);
            VanillaStyle = LoadDllResource("Thumbs.Button.VanillaThumbnails.png", 295, 52);
            TwowayDirection = LoadDllResource("Thumbs.Button.TwowayThumbnails.png", 180, 33);
            OnewayDirection = LoadDllResource("Thumbs.Button.OnewayThumbnails.png", 180, 33);
            Checkbox = LoadDllResource("Thumbs.CheckboxThumbnails.png", 95, 20);
            WideMedianPillar = LoadDllResource("Thumbs.Button.WideMedianPillarThumbnails.png", 250, 50);
            WidePillar = LoadDllResource("Thumbs.Button.WidePillarThumbnails.png", 250, 50);
            NarrowMedianPillar = LoadDllResource("Thumbs.Button.NarrowMedianPillarThumbnails.png", 250, 50);
            NarrowPillar = LoadDllResource("Thumbs.Button.NarrowPillarThumbnails.png", 250, 50);
            SteamTab = LoadDllResource("Thumbs.Tab.SteamTabThumbnails.png", 380, 23);
            MetroTab = LoadDllResource("Thumbs.Tab.MetroTabThumbnails.png", 380, 23);
            TrainTab = LoadDllResource("Thumbs.Tab.TrainTabThumbnails.png", 380, 23);
            MetroTrackToggle = LoadDllResource("Thumbs.Toggle.MetroTrackToggleThumbnails.png", 111, 37);
            TrainTrackToggle = LoadDllResource("Thumbs.Toggle.TrainTrackToggleThumbnails.png", 111, 37);
            SidePlatformStationTrack = LoadDllResource("Thumbs.Button.SidePlatformThumbnails.png", 395, 69);
            IslandPlatformStationTrack = LoadDllResource("Thumbs.Button.IslandThumbnails.png", 395, 69);
            SinglePlatformStationTrack = LoadDllResource("Thumbs.Button.SinglePlatformThumbnails.png", 395, 69);
            ExpressSidePlatformStationTrack = LoadDllResource("Thumbs.Button.ExpressSideThumbnails.png", 395, 69);
            DualIslandPlatformStationTrack = LoadDllResource("Thumbs.Button.DualIslandThumbnails.png", 395, 69);
        }

        private static Texture2D LoadDllResource(string resourceName, int width, int height)
        {
            try
            {
                var myAssembly = Assembly.GetExecutingAssembly();
                var myStream = myAssembly.GetManifestResourceStream("MetroOverhaul.Resources.UI." + resourceName);

                var texture = new Texture2D(width, height, TextureFormat.ARGB32, false);

                texture.LoadImage(ReadToEnd(myStream));

                return texture;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e.StackTrace.ToString());
                return null;
            }
        }
        static byte[] ReadToEnd(Stream stream)
        {
            var originalPosition = stream.Position;
            stream.Position = 0;

            try
            {
                var readBuffer = new byte[4096];

                var totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead != readBuffer.Length)
                        continue;

                    var nextByte = stream.ReadByte();
                    if (nextByte == -1)
                        continue;

                    var temp = new byte[readBuffer.Length * 2];
                    Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                    Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                    readBuffer = temp;
                    totalBytesRead++;
                }

                var buffer = readBuffer;
                if (readBuffer.Length == totalBytesRead)
                    return buffer;

                buffer = new byte[totalBytesRead];
                Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                return buffer;
            }
            catch (Exception e)
            {
                Debug.LogError(e.StackTrace.ToString());
                return null;
            }
            finally
            {
                stream.Position = originalPosition;
            }
        }
        public static UITextureAtlas GenerateLinearAtlas(string name, Texture2D texture, int numSprites = 5)
        {
            string[] ts = null;

            if (numSprites == 5 || numSprites == 10)
            {
                ts = new[] { "", "Disabled", "Focused", "Hovered", "Pressed" };
            }
            else if (numSprites == 3)
            {
                ts = new[] { "", "Hovered", "Pressed" };
            }
            List<string> spriteNames = new List<string>();
            if (numSprites > 5)
            {
                foreach (var value in ts)
                {
                    spriteNames.Add(name + "Bg" + value.ToString());
                }
                foreach (var value in ts)
                {
                    spriteNames.Add(name + "Fg" + value.ToString());
                }
            }
            else
            {
                foreach (var value in ts)
                {
                    spriteNames.Add(name + "Bg" + value.ToString());
                }
            }

            return GenerateLinearAtlas(name, texture, numSprites, spriteNames.ToArray());
        }
        public static UITextureAtlas GenerateLinearAtlas(string name, Texture2D texture, int numSprites, string[] spriteNames)
        {
            return Generate2DAtlas(name, texture, numSprites, 1, spriteNames);
        }

        public static UITextureAtlas Generate2DAtlas(string name, Texture2D texture, int numX, int numY, string[] spriteNames)
        {
            if (spriteNames.Length != numX * numY)
            {
                throw new ArgumentException($"Number of sprite name does not match dimensions (expected {numX} x {numY}, was {spriteNames.Length})");
            }

            UITextureAtlas atlas = ScriptableObject.CreateInstance<UITextureAtlas>();
            atlas.padding = 0;
            atlas.name = name;

            var shader = Shader.Find("UI/Default UI Shader");
            if (shader != null)
                atlas.material = new Material(shader);
            atlas.material.mainTexture = texture;

            int spriteWidth = Mathf.RoundToInt((float)texture.width / (float)numX);
            int spriteHeight = Mathf.RoundToInt((float)texture.height / (float)numY);

            int k = 0;
            for (int i = 0; i < numX; ++i)
            {
                float x = (float)i / (float)numX;
                for (int j = 0; j < numY; ++j)
                {
                    float y = (float)j / (float)numY;

                    var sprite = new UITextureAtlas.SpriteInfo
                    {
                        name = spriteNames[k],
                        region = new Rect(x, y, (float)spriteWidth / (float)texture.width, (float)spriteHeight / (float)texture.height)
                    };

                    var spriteTexture = new Texture2D(spriteWidth, spriteHeight);
                    spriteTexture.SetPixels(texture.GetPixels((int)((float)texture.width * sprite.region.x), (int)((float)texture.height * sprite.region.y), spriteWidth, spriteHeight));
                    sprite.texture = spriteTexture;

                    atlas.AddSprite(sprite);

                    ++k;
                }
            }

            return atlas;
        }
    }
}
