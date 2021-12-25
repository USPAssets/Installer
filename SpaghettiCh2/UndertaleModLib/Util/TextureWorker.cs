using System;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using UndertaleModLib.Models;

namespace UndertaleModLib.Util
{
    public class TextureWorker
    {
        private Dictionary<UndertaleEmbeddedTexture, Image<Rgba32>> embeddedDictionary = new Dictionary<UndertaleEmbeddedTexture, Image<Rgba32>>();

        // Cleans up all the images when usage of this worker is finished.
        // Should be called when a TextureWorker will never be used again.
        public void Cleanup()
        {
            foreach (Image<Rgba32> img in embeddedDictionary.Values)
                img.Dispose();
            embeddedDictionary.Clear();
        }

        public Image<Rgba32> GetEmbeddedTexture(UndertaleEmbeddedTexture embeddedTexture)
        {
            lock (embeddedDictionary)
            {
                if (!embeddedDictionary.ContainsKey(embeddedTexture))
                    embeddedDictionary[embeddedTexture] = GetImageFromByteArray(embeddedTexture.TextureData.TextureBlob);
                return embeddedDictionary[embeddedTexture];
            }
        }

        public void ExportAsPNG(UndertaleTexturePageItem texPageItem, string FullPath, string imageName = null, bool includePadding = false)
        {
            SaveImageToFile(FullPath, GetTextureFor(texPageItem, imageName ?? Path.GetFileNameWithoutExtension(FullPath), includePadding));
        }

        public Image<Rgba32> GetTextureFor(UndertaleTexturePageItem texPageItem, string imageName, bool includePadding = false)
        {
            int exportWidth = texPageItem.BoundingWidth; // sprite.Width
            int exportHeight = texPageItem.BoundingHeight; // sprite.Height
            var embeddedImage = GetEmbeddedTexture(texPageItem.TexturePage);

            // Sanity checks.
            if (includePadding && ((texPageItem.TargetWidth > exportWidth) || (texPageItem.TargetHeight > exportHeight)))
                throw new InvalidDataException(imageName + "'s texture is larger than its bounding box!");

            // Create a bitmap representing that part of the texture page.
            Image<Rgba32> resultImage = null;
            lock (embeddedImage)
            {
                try
                {
                    // new Rectangle(texPageItem.SourceX, texPageItem.SourceY, texPageItem.SourceWidth, texPageItem.SourceHeight)
                    resultImage = new Image<Rgba32>(texPageItem.SourceWidth, texPageItem.SourceHeight);
                    resultImage.Mutate(x => x.DrawImage(embeddedImage, new Point(texPageItem.SourceX, texPageItem.SourceY), 1.0f));
                }
                catch (OutOfMemoryException)
                {
                    throw new OutOfMemoryException(imageName + "'s texture is abnormal. 'Source Position/Size' boxes 3 & 4 on texture page may be bigger than the sprite itself or it's set to '0'.");
                }
            }

            // Resize the image, if necessary.
            if ((texPageItem.SourceWidth != texPageItem.TargetWidth) || (texPageItem.SourceHeight != texPageItem.TargetHeight))
                resultImage = ResizeImage(resultImage, texPageItem.TargetWidth, texPageItem.TargetHeight);

            // Put it in the final holder image.
            Image<Rgba32> returnImage = resultImage;
            if (includePadding)
            {
                returnImage = new Image<Rgba32>(exportWidth, exportHeight);
                returnImage.Mutate(x => x.DrawImage(resultImage, new Point(texPageItem.SourceX, texPageItem.SourceY), 1.0f));
            }

            return returnImage;
        }

        public static Image<Rgba32> ReadImageFromFile(string filePath)
        {
            return Image.Load<Rgba32>(filePath);
        }

        // Grabbed from https://stackoverflow.com/questions/3801275/how-to-convert-image-to-byte-array/16576471#16576471
        public static Image<Rgba32> GetImageFromByteArray(byte[] byteArray)
        {
            return Image.Load<Rgba32>(byteArray);
        }

        // This should perform a high quality resize.
        // Grabbed from https://stackoverflow.com/questions/1922040/how-to-resize-an-image-c-sharp
        public static Image<Rgba32> ResizeImage(Image<Rgba32> image, int width, int height)
        {
            if (image.Width == width && image.Height == height)
            {
                return image.Clone();
            }

            var resizeOptions = new ResizeOptions()
            {
                Mode = ResizeMode.Stretch,
                Size = new Size(width, height),
                Position = AnchorPositionMode.TopLeft,
                PremultiplyAlpha = false,
                Sampler = KnownResamplers.Bicubic
            };

            var destImage = image.Clone(x => x.Resize(resizeOptions));

            return destImage;
        }

        public static byte[] ReadMaskData(string filePath)
        {
            Image<Rgba32> image = ReadImageFromFile(filePath);
            List<byte> bytes = new List<byte>();

            var enableColor = Color.White.ToPixel<Rgba32>();
            for (int y = 0; y < image.Height; y++)
            {
                for (int xByte = 0; xByte < (image.Width + 7) / 8; xByte++)
                {
                    byte fullByte = 0x00;
                    int pxStart = (xByte * 8);
                    int pxEnd = Math.Min(pxStart + 8, (int) image.Width);

                    for (int x = pxStart; x < pxEnd; x++)
                        if (image[x, y] == enableColor) // Don't use Color == OtherColor, it doesn't seem to give us the type of equals comparison we want here.
                            fullByte |= (byte)(0b1 << (7 - (x - pxStart)));

                    bytes.Add(fullByte);
                }
            }

            image.Dispose();
            return bytes.ToArray();
        }

        public static byte[] ReadTextureBlob(string filePath)
        {
            //Image.FromFile(filePath).Dispose(); // Make sure the file is valid image.
            return File.ReadAllBytes(filePath);
        }

        public static void SaveEmptyPNG(string FullPath, int width, int height)
        {
            var blackPix = Color.Black.ToPixel<Rgba32>();
            var blackImage = new Image<Rgba32>(width, height, blackPix);
            SaveImageToFile(FullPath, blackImage);
        }

        public static Image<Rgba32> GetCollisionMaskImage(UndertaleSprite sprite, UndertaleSprite.MaskEntry mask)
        {
            byte[] maskData = mask.Data;
            var bitmap = new Image<Rgba32>((int)sprite.Width, (int)sprite.Height); // Ugh. I want to use 1bpp, but for some BS reason C# doesn't allow SetPixel in that mode.

            var white = Color.White.ToPixel<Rgba32>();
            var black = Color.Black.ToPixel<Rgba32>();

            for (int y = 0; y < sprite.Height; y++)
            {
                int rowStart = y * (int)((sprite.Width + 7) / 8);
                for (int x = 0; x < sprite.Width; x++)
                {
                    byte temp = maskData[rowStart + (x / 8)];
                    bool pixelBit = (temp & (0b1 << (7 - (x % 8)))) != 0b0;
                    bitmap[x, y] = pixelBit ? white : black;
                }
            }

            return bitmap;
        }

        public static void ExportCollisionMaskPNG(UndertaleSprite sprite, UndertaleSprite.MaskEntry mask, string fullPath)
        {
            SaveImageToFile(fullPath, GetCollisionMaskImage(sprite, mask));
        }

        public static byte[] GetImageBytes(Image<Rgba32> image, bool disposeImage = true)
        {
            using (var ms = new MemoryStream())
            {
                image.SaveAsPng(ms);
                byte[] result = ms.ToArray();
                if (disposeImage)
                    image.Dispose();
                return result;
            }
        }

        public static void SaveImageToFile(string FullPath, Image<Rgba32> image, bool disposeImage = true)
        {
            image.SaveAsPng(FullPath);
            if (disposeImage)
                image.Dispose();
        }
    }
}
