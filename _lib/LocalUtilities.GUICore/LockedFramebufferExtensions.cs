using LocalUtilities.General;
using Avalonia.Platform;
using System;

namespace LocalUtilities.GUICore;

public static class LockedFramebufferExtensions
{
    public static int GetBytesPerPixel(this PixelFormat pixelFormat)
    {
        if (PixelFormat.Rgb565.Equals(pixelFormat))
            return 2;
        if (PixelFormat.Rgba8888.Equals(pixelFormat))
            return 4;
        if (PixelFormat.Bgra8888.Equals(pixelFormat))
            return 4;

        throw new ArgumentOutOfRangeException(nameof(pixelFormat), pixelFormat, null);
    }

    public static Span<byte> GetPixels(this ILockedFramebuffer framebuffer)
    {
        unsafe
        {
            return new Span<byte>((byte*)framebuffer.Address, framebuffer.RowBytes * framebuffer.Size.Height);
        }
    }

    public static Span<byte> GetPixel(this ILockedFramebuffer framebuffer, int x, int y)
    {
        unsafe
        {
            var bytesPerPixel = framebuffer.Format.GetBytesPerPixel();
            var zero = (byte*)framebuffer.Address;
            var offset = framebuffer.RowBytes * y + bytesPerPixel * x;
            return new Span<byte>(zero + offset, bytesPerPixel);
        }
    }

    public static void SetPixel(this ILockedFramebuffer framebuffer, int x, int y, Color color)
    {
        var pixel = framebuffer.GetPixel(x, y);

        var alpha = color.A / 255.0;

        var frameBufferFormat = framebuffer.Format;

        if (PixelFormat.Rgb565.Equals(frameBufferFormat))
        {
            var value = (((color.R & 0b11111000) << 8) + ((color.G & 0b11111100) << 3) + (color.B >> 3));
            pixel[0] = (byte)value;
            pixel[1] = (byte)(value >> 8);
        }
        else if (PixelFormat.Rgba8888.Equals(frameBufferFormat))
        {
            pixel[0] = (byte)(color.R * alpha);
            pixel[1] = (byte)(color.G * alpha);
            pixel[2] = (byte)(color.B * alpha);
            pixel[3] = color.A;
        }
        else if (PixelFormat.Bgra8888.Equals(frameBufferFormat))
        {
            pixel[0] = (byte)(color.B * alpha);
            pixel[1] = (byte)(color.G * alpha);
            pixel[2] = (byte)(color.R * alpha);
            pixel[3] = color.A;
        }
        else
        {
            throw new ArgumentOutOfRangeException();
        }

    }
}
