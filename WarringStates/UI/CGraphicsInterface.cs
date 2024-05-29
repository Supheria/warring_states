using System.Runtime.InteropServices;

namespace WarringStates.UI;

public static partial class CGraphicsInterface
{
    private const string dllPath = @"CGraphics.dll";

    [LibraryImport(dllPath, EntryPoint = "DrawGraph")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    private static partial void DrawGraph(IntPtr mhwnd, int width, int height);

    public static void DrawGraph(this Graphics g, int width, int height)
    {
        var hdc = g.GetHdc();
        DrawGraph(hdc, width, height);
        g.ReleaseHdc(hdc);
    }

    [LibraryImport(dllPath, EntryPoint = "Render")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    private static unsafe partial void Render(IntPtr mhwnd, int width, int height);

    public static void Render(this Graphics g, int width, int height)
    {
        var hdc = g.GetHdc();
        Render(hdc, width, height);
        g.ReleaseHdc(hdc);
    }
}
