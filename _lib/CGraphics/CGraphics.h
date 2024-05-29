#pragma once

#include <Windows.h>

extern "C" _declspec(dllexport) void DrawGraph(int width, int height, HDC hdc);

extern "C" _declspec(dllexport) void Render(HDC hdc, int width, int height);
