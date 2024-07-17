namespace AltitudeMapGenerator.Test;

/// <summary>
/// 摘要：
///    跨线程访问UI通用组件,在此类外的Try Catch语句不能捕获委托中的错误。
/// </summary>
static class ControlExtensions
{
    /// <summary>
    /// 同步执行 注：外层Try Catch语句不能捕获Code委托中的错误
    /// </summary>
    static public void UIThreadInvoke(this Control control, Action Code)
    {
        try
        {
            if (control.InvokeRequired)
            {
                control.Invoke(Code);
                return;
            }
            Code.Invoke();
        }
        catch
        {
            /*仅捕获、不处理！*/
        }
    }

    /// <summary>
    /// 异步执行 注：外层Try Catch语句不能捕获Code委托中的错误
    /// </summary>
    static public void UIThreadBeginInvoke(this Control control, Action Code)
    {
        if (control.InvokeRequired)
        {
            control.BeginInvoke(Code);
            return;
        }
        Code.Invoke();
    }
}

