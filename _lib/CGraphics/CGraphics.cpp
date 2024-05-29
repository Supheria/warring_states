#include "CGraphics.h"

//HINSTANCE hInst;//保存实例句柄

void DrawGraph(int width, int height, HDC hdc)
{
    //HINSTANCE hInst = 0;
    HBITMAP hSun = (HBITMAP)LoadImage(0, "1.jpg", IMAGE_BITMAP, width, height, LR_LOADFROMFILE | LR_CREATEDIBSECTION);
    //HDC hMemDc = CreateCompatibleDC(hdc);//获取内存设备描述表句柄，使得  位图能在内存中保存下来
    SelectObject(hdc, hSun);//选择位图对象，送入内存设备描述表；
    //BitBlt(hdc, 0, 0, width, height, hSun, 0, 0, SRCCOPY);//把位图从内存复制到窗口
    //DeleteDC(hMemDc);//删除设备内存描述表中的位图
    //return 1;
}

void Render(HDC hdc, int width, int height)//画面渲染
{
    //HDC hDC = CreateCompatibleDC(hdc);		//获得系统绘图设备

    //HDC memDC = CreateCompatibleDC(0);	//创建辅助绘图设备

    //HBITMAP bmpBack = CreateCompatibleBitmap(hdc, width, height);//创建掩码位图（画布）
    //SelectObject(hdc, bmpBack);	//将画布贴到绘图设备上

    HPEN penBack = CreatePen(PS_SOLID, 1, RGB(255, 0, 255));//创建画笔
    SelectObject(hdc, penBack);    //将画笔选到绘图设备上

    HBRUSH brushBack = CreateSolidBrush(RGB(255, 255, 255));//创建画刷
    SelectObject(hdc, brushBack);  //将画刷选到绘图设备上

    //擦除背景
    RECT rcClient = RECT{ 0, 0, width, height };
    HBRUSH brushTemp = (HBRUSH)GetStockObject(WHITE_BRUSH);//获得库存物体，白色画刷。
    FillRect(hdc, &rcClient, brushTemp);//填充客户区域。
    //    
    HBRUSH brushObj = CreateSolidBrush(RGB(0, 255, 0));//创建物体画刷
    //绘制维网格，矩形画法。
    int dw = 30;
    int rows = width / dw;
    int cols = height / dw;
    for (int r = 0; r < rows; ++r)
    {
        for (int c = 0; c < cols; ++c)
        {
            if (r == c)
            {
                SelectObject(hdc, brushObj);
            }
            else
            {
                SelectObject(hdc, brushBack);
            }
            Rectangle(hdc, c * dw, r * dw, (c + 1) * dw, (r + 1) * dw);
        }
    }

    DeleteObject(brushObj);
    //
    //SelectObject(hdc, bmpBack);//选择位图对象，送入内存设备描述表；
    //BitBlt(hDC, 0, 0, g_nWidth, g_nHeight, memDC, 0, 0, SRCCOPY);//复制到系统设备上显示
    //BitBlt(hdc, 0, 0, width, height, memDC, 0, 0, SRCCOPY);//把位图从内存复制到窗口
    DeleteObject(penBack);  //释放画笔资源
    DeleteObject(brushBack);//释放画刷资源
    //DeleteObject(bmpBack);  //释放位图资源
    //DeleteDC(memDC);	    //释放辅助绘图设备
    //ReleaseDC(hdc, hDC);	//归还系统绘图设备
    //Sleep(1);
}
