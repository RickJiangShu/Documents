using UnityEngine;
using System.Collections;

/// <summary>
/// 通用数学工具集
/// </summary>
public struct Mathr
{
    /// <summary>
    /// 清楚不用的0
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string TrimZero(string str)
    {
        return str.Trim('0');
    }

    #region 小数操作
    /// <summary>
    /// 获取浮点数整数部分
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public static int Decimal2Integer(decimal n)
    {
        return (int)n;
    }

    /// <summary>
    /// 获取浮点数小数部分
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public static decimal Decimal2Fraction(decimal n)
    {
        return n - Decimal2Integer(n);
    }

    /// <summary>
    /// 判断浮点数是否有小数部分
    /// </summary>
    /// <returns></returns>
    public static bool DecimalHasFraction(decimal n)
    {
        return Decimal2Integer(n) != n;
    }

    #endregion

    #region 基于二进制字符串的运算
    /// <summary>
    /// 给出两个二进制字符串，将两者数位对其
    /// </summary>
    /// <returns></returns>
    public static void BinaryPadLeft(ref string b1,ref string b2)
    {
        int l1 = b1.Length;
        int l2 = b2.Length;
        if (l1 < l2)
        {
            b1 = b1.PadLeft(l2, '0');
            l1 = l2;
        }
        else if (l2 < l1)
        {
            b2 = b2.PadLeft(l1, '0');
            l2 = l1;
        }
    }

    /// <summary>
    /// 对二进制字符串进行大小比对
    /// </summary>
    /// <returns>true -1 b1 > b2 0 b1 = b2 1 b1 < b2</returns>
    public static int BinaryCompare(string b1,string b2)
    {
        BinaryPadLeft(ref b1, ref b2);
        int l1 = b1.Length;
        for (int i = 0; i < l1; i++)
        {
            if (b1[i] != b2[i])
            {
                if (b1[i] == '1')
                    return -1;
                else
                    return 1;
            }
        }
        return 0;
    }

    /// <summary>
    /// 二进制加法
    /// </summary>
    /// <param name="b1"></param>
    /// <param name="b2"></param>
    /// <returns></returns>
    public static string AddBinaryString(string b1, string b2)
    {
        BinaryPadLeft(ref b1, ref b2);
        int l1 = b1.Length;
        char[] writeArray = new char[l1];
        char writeDigit = '0';
        char carryDigit = '0';
        int index = l1 - 1;
        for (; index >= 0; index--)
        {
            char k1 = b1[index];
            char k2 = b2[index];
            if (k1 == k2)
            {
                writeDigit = carryDigit;
                carryDigit = k1;
            }
            else
            {
                writeDigit = carryDigit == '0' ? '1' : '0';
            }
            writeArray[index] = writeDigit;
        }
        string output = new string(writeArray);
        if (carryDigit == '1')
            output = output.PadLeft(l1 + 1, '1');
        
        return output;
    }
    public static string AddBinaryString(params string[] args)
    {
        if (args.Length < 2)
            return "";

        string output = "0";
        for (int i = 0, l = args.Length; i < l; i++)
        {
            output = AddBinaryString(output, args[i]);
        }
        return output;
    }

    /// <summary>
    /// 二进制减法
    /// </summary>
    /// <param name="b1">被减数</param>
    /// <param name="b2">减数</param>
    /// <returns>差</returns>
    public static string MinusBinaryString(string b1, string b2)
    {
        return "";

        int compareResult = BinaryCompare(b1, b2);
        if (compareResult == 1)//减数 > 被减数
            return "";
        if (compareResult == 0)
            return "0";


    }

    #endregion

}
