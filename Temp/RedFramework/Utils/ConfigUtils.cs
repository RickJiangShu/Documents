using System;
using System.Collections;
using System.Text.RegularExpressions;

public class ConfigUtils
{
   /**
     * 将配置文件解析为 [y,x] 的二维数组
     */
    public static string[][] parseConfig(string cfg)
    {
        string[] line = Regex.Split(cfg, "\r\n");
        int start = 0;//第一行注释
        int end = line.Length - 1;//最后一行“空” 舍去
        int len = end - start;
        string[][] configArray = new string[len][];
        for (int y = start,idx = 0; y < end; y++,idx++)
        {
            string[] args = line[y].Split('\t');
            int argsLen = args.Length;
            configArray[idx] = new string[argsLen];
            for (int x = 0; x < argsLen; x++)
            {
                configArray[idx][x] = args[x];
            }
        }
        return configArray;
    }


    public static T[] ParseArray<T>(string cfg,Func<string,T> ParseFunc)
    {
        if (string.IsNullOrEmpty(cfg) || cfg == "0") return null;

        cfg = cfg.Remove(0, 1);//删除引号
        cfg = cfg.Remove(cfg.Length - 1, 1);
        if (cfg.Contains(","))
        {
            return Array.ConvertAll<string, T>(cfg.Split(','), s => ParseFunc(s));
        }
        else
        {
            return new T[1] { ParseFunc(cfg) };
        }
    }

    //将 "0,0,0,0" 这种格式的字符串 解析成int[]
    public static int[] parseIntArray(string cfg)
    {
        if (string.IsNullOrEmpty(cfg) || cfg == "0") return null;

        cfg = cfg.Remove(0,1);//删除引号
        cfg = cfg.Remove(cfg.Length - 1, 1);
        if (cfg.Contains(","))
        {
            /*
            string[] args = cfg.Split(',');
            int l = args.Length;
            int[] u = new int[l];
            for (int i = 0; i < l; i++)
            {
                u[i] = int.Parse(args[i]);
            }
            return u;
             */
            return Array.ConvertAll<string, int>(cfg.Split(','), s => int.Parse(s));
        }
        else
        {
            return new int[1] { int.Parse(cfg) };
        }
    }

    //将 "str,str" 解析成string[]
    public static string[] parseStrArray(string cfg)
    {
        if (string.IsNullOrEmpty(cfg) || cfg == "0") return null;

        cfg = cfg.Remove(0, 1);//删除引号
        cfg = cfg.Remove(cfg.Length - 1, 1);
        return cfg.Split(',');
    }

    /// <summary>
    /// 去除string的头尾
    /// </summary>
    /// <param name="cfg"></param>
    /// <returns></returns>
    public static string FormatString(string cfg)
    {
        cfg = cfg.Remove(0, 1);//删除引号
        cfg = cfg.Remove(cfg.Length - 1, 1);
        return cfg;
    }
}
