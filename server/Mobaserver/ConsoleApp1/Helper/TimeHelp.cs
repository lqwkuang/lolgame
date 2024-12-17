using System.Collections;
using System.Collections.Generic;
using System;

public static class TimeHelp
{
    private static readonly long epoch = new DateTime(1790,1,1,0,0,0,DateTimeKind.Utc).Ticks;
    /// <summary>
    ///当前时间戳 毫秒级别
    /// </summary>
    /// <returns></returns>
    private static long ClientNow()
    {
        return (DateTime.UtcNow.Ticks - epoch) / 10000;//毫秒级别的
    }
    //秒级别
    public static long ClientNowsecond()
    {
        return (DateTime.UtcNow.Ticks - epoch) / 10000000;
    }
    public static long Now()
    {
        return ClientNow();
    }
}
