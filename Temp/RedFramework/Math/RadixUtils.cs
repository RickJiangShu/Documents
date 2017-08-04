using System;
using System.Collections;

/// <summary>
/// 进位计数制工具
/// </summary>
public class RadixUtils
{
    /// <summary>
    /// 十进制数字转换为任意进制格式
    /// </summary>
    /// <returns></returns>
    public static string Decimal2Radix(decimal d,byte b = 2,byte p = 64)
    {
        int i = Mathr.Decimal2Integer(d);
        decimal f = Mathr.Decimal2Fraction(d);
        return Integer2Radix(i, b) + "." + Fraction2Radix(f, b);
    }

    /// <summary>
    /// 任意进制转换为十进制数字
    /// </summary>
    /// <param name="digit"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static decimal Radix2Decimal(string digit, byte b = 2)
    {
        if (string.IsNullOrEmpty(digit))
            return 0;

        int length = digit.Length;
        decimal output = Sign2Decimal(digit[length - 1]);
        string remainingString = digit.Substring(0, length - 1);
        decimal valueOfRemaining = Radix2Decimal(remainingString, b);
        output += b * valueOfRemaining;
        return output;
    }

    /// <summary>
    /// 二进制转换为八进制
    /// </summary>
    /// <returns></returns>
    public static string Binary2Octal(string b)
    {
        b = Mathr.TrimZero(b);
        string[] s = b.Split('.');
        string i = s[0];
        
        string output = "";
        int il = (int)Math.Ceiling(i.Length / 3f) * 3;
        i = i.PadLeft(il, '0');
        for (int ii = 0; ii < il; ii += 3)
        {
            output += SignBinary2Octal(i.Substring(ii, 3));
        }
        if (s.Length > 1)
        {
            output += ".";
            string f = s[1];
            int fl = (int)Math.Ceiling(f.Length / 3f) * 3;
            f = f.PadRight(fl,'0');
            for (int fi = 0; fi < fl; fi += 3)
            {
                output += SignBinary2Octal(f.Substring(fi, 3));
            }
        }
        return output;
    }

    /// <summary>
    /// 八进制转换为二进制
    /// </summary>
    /// <returns></returns>
    public static string Octal2Binary(string o)
    {
        o = Mathr.TrimZero(o);
        string output = "";
        for (int i = 0, l = o.Length; i < l; i++)
        {
            output += SignOctal2Binary(o[i]);
        }
        output = Mathr.TrimZero(output);
        return output;
    }

    /// <summary>
    /// 二进制转换为十六进制
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    public static string Binary2Hex(string b)
    {
        b = Mathr.TrimZero(b);
        string[] s = b.Split('.');
        string i = s[0];

        string output = "";
        int il = (int)Math.Ceiling(i.Length / 4f) * 4;
        i = i.PadLeft(il, '0');
        for (int ii = 0; ii < il; ii += 4)
        {
            output += SignBinary2Hex(i.Substring(ii, 4));
        }
        if (s.Length > 1)
        {
            output += ".";
            string f = s[1];
            int fl = (int)Math.Ceiling(f.Length / 4f) * 4;
            f = f.PadRight(fl, '0');
            for (int fi = 0; fi < fl; fi += 4)
            {
                output += SignBinary2Hex(f.Substring(fi, 4));
            }
        }
        return output;
    }

    /// <summary>
    /// 十六进制转换为二进制
    /// </summary>
    /// <returns></returns>
    public static string Hex2Binary(string o)
    {
        o = Mathr.TrimZero(o);
        string output = "";
        for (int i = 0, l = o.Length; i < l; i++)
        {
            output += SignHex2Binary(o[i]);
        }
        output = Mathr.TrimZero(output);
        return output;
    }

    #region 符号转换
    /// <summary>
    /// 数字转换为进制符
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public static string Decimal2Sign(int number)
    {
        switch (number)
        {
            case 10:
                return "A";
            case 11:
                return "B";
            case 12:
                return "C";
            case 13:
                return "D";
            case 14:
                return "E";
            case 15:
                return "F";
            default:
                return number.ToString();
        }
    }
    /// <summary>
    /// 进制符转换为数字
    /// </summary>
    /// <param name="hex"></param>
    /// <returns></returns>
    public static decimal Sign2Decimal(char hex)
    {
        switch (hex)
        {
            case 'A':
                return 10;
            case 'B':
                return 11;
            case 'C':
                return 12;
            case 'D':
                return 13;
            case 'E':
                return 14;
            case 'F':
                return 15;
            default:
                return hex - '0';
        }
    }
    public static decimal Sign2Decimal(string hex)
    {
        return Sign2Decimal(hex[0]);
    }

    private static string SignBinary2Octal(string b)
    {
        switch (b)
        {
            case "000":
                return "0";
            case "001":
                return "1";
            case "010":
                return "2";
            case "011":
                return "3";
            case "100":
                return "4";
            case "101":
                return "5";
            case "110":
                return "6";
            case "111":
                return "7";
            default:
                return b;
        }
    }
    private static string SignOctal2Binary(char o)
    {
        switch (o)
        {
            case '0':
                return "000";
            case '1':
                return "001";
            case '2':
                return "010";
            case '3':
                return "011";
            case '4':
                return "100";
            case '5':
                return "101";
            case '6':
                return "110";
            case '7':
                return "111";
            default:
                return o.ToString();
        }
    }
    private static string SignBinary2Hex(string b)
    {
        switch (b)
        {
            case "0000":
                return "0";
            case "0001":
                return "1";
            case "0010":
                return "2";
            case "0011":
                return "3";
            case "0100":
                return "4";
            case "0101":
                return "5";
            case "0110":
                return "6";
            case "0111":
                return "7";
            case "1000":
                return "8";
            case "1001":
                return "9";
            case "1010":
                return "A";
            case "1011":
                return "B";
            case "1100":
                return "C";
            case "1101":
                return "D";
            case "1110":
                return "E";
            case "1111":
                return "F";
            default:
                return b;
        }
    }
    private static string SignHex2Binary(char o)
    {
        switch (o)
        {
            case '0':
                return "0000";
            case '1':
                return "0001";
            case '2':
                return "0010";
            case '3':
                return "0011";
            case '4':
                return "0100";
            case '5':
                return "0101";
            case '6':
                return "0110";
            case '7':
                return "0111";
            case '8':
                return "1000";
            case '9':
                return "1001";
            case 'A':
                return "1010";
            case 'B':
                return "1011";
            case 'C':
                return "1100";
            case 'D':
                return "1101";
            case 'E':
                return "1110";
            case 'F':
                return "1111";
            default:
                return o.ToString();
        }
    }
    #endregion

   

    /// <summary>
    /// 十进制整型部分转换
    /// </summary>
    private static string Integer2Radix(int i, byte b)
    {
        if (i < b)
        {
            return Decimal2Sign(i);
        }
        else
        {
            int remainder = i % b;
            int reducedNumber = (i - remainder) / b;
            string restOfString = Integer2Radix(reducedNumber, b);
            return restOfString + Decimal2Sign(remainder);
        }
    }

    /// <summary>
    /// 十进制分数部分转换
    /// </summary>
    /// <returns></returns>
    private static string Fraction2Radix(decimal f, byte b ,byte p = 64)
    {
        string output = "";
        int count = 0;
        do
        {
            decimal result = f * b;
            int i = Mathr.Decimal2Integer(result);
            f = Mathr.Decimal2Fraction(result);
            output += i;
            count++;
        }
        while (f != 0 && count < p);
        return output;
    }
    

    
}
