using System.Collections;
using System;

public class TimeUtils
{
    /// <summary>
    /// 获取1970.01.01到现在的时间戳（秒）
    /// </summary>
    /// <returns></returns>
    public static long Timestamp()
    {
        return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
    }
}
