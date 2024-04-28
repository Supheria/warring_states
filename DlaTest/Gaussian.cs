using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DlaTest;

public static class Gaussian
{
    private static void clip_array(int min, int max, ref int[] array) //截断
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] > max)
                array[i] = max;
            if (array[i] < min)
                array[i] = min;
        }
    }

    public static System.Drawing.Bitmap Convolution_calculation( this System.Drawing.Bitmap bitmap,
double[,] kernel = null)//卷积运算
    {
        /*Paras:
            bitmap: 进行卷积运算的Bitmap
            kernel: 卷积核数组
        */

        System.Drawing.Bitmap new_bitmap = new System.Drawing.Bitmap(bitmap);
        //将bitmap复制给将要作为结果输出的new_bitmap

        int kernel_length; //意为卷积核单侧的长度

        if (kernel.GetLength(0) != kernel.GetLength(1) || kernel == null)
        { //若卷积核长宽不相等或不存在卷积核则报错
            MessageBox.Show("The  convolution kernel is wrong!", "ERROR!");
            return null;
        }

        if (kernel.GetLength(0) % 2 == 1) //奇数
        {//卷积核为奇数则给卷积核单侧长度赋值
            kernel_length = (kernel.GetLength(0) - 1) / 2;
        }
        else
        {//若卷积核不为奇数则报错
            MessageBox.Show("Kernel_size shoule be singular!", "ERROR!");
            return null;
        }

        int[] val = new int[3];
        //长度为3的数组，因为BMP图像有三个通道，所以我们需要三个数来存储点乘的值

        for (int x = kernel_length; x + kernel_length < bitmap.Width; x++)
            for (int y = kernel_length; y + kernel_length < bitmap.Height; y++)
            { //注意理解这里的xy循环起始值，就是为了防止卷积核超出图像外

                val[0] = 0;//在每一次卷积核移动时，记得清空上一次卷积计算的结果
                val[1] = 0;
                val[2] = 0;

                for (int j = -kernel_length; j <= kernel_length; j++)
                {
                    for (int i = -kernel_length; i <= kernel_length; i++)
                    {    //在卷积核内的循环

                        val[0] += (int)((double)bitmap.GetPixel(x + i, y + j).R * kernel[j + kernel_length, i + kernel_length]);//R通道的点乘结果

                        val[1] += (int)((double)bitmap.GetPixel(x + i, y + j).G * kernel[j + kernel_length, i + kernel_length]);//G通道的点乘结果

                        val[2] += (int)((double)bitmap.GetPixel(x + i, y + j).B * kernel[j + kernel_length, i + kernel_length]);//B通道的点乘结果

                    }
                }

                clip_array(0, 255, ref val);//这里的clip_array函数其实就是一个截断，遍历这个数组并保证像素值在0-255之间，数组的实现我放在下边。

                new_bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(val[0], val[1], val[2]));//获得当前卷积结果后设置新的像素值
            }


        return new_bitmap; //返回卷积运算后的结果
    }
}
