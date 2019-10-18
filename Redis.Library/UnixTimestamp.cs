using System;
using System.Collections.Generic;
using System.Text;

namespace Redis.Library
{
    /// <summary>
    /// Author: Gordon
    /// Created: 2019-10-17
    /// Description: Timestamp and datetime conversion
    /// </summary>
    public class UnixTimestamp
    {
        /// <summary>
        /// 基準時間
        /// </summary>
        private static DateTime dtBase = new DateTime(1970, 1, 1).ToLocalTime();

        /// <summary>
        /// 時間戳末尾7位(補0或截斷)
        /// </summary>
        private const long TICK_BASE = 10000000;

        /// <summary>
        /// 從現在(調用此函數時刻)起若干秒以後那個時間點的時間戳
        /// </summary>
        /// <param name="secondsAfterNow">從現在起多少秒以後</param>
        /// <returns>Unix時間戳</returns>
        public static long GetUnixTimestamp(long secondsAfterNow)
        {
            DateTime dt = DateTime.Now.AddSeconds(secondsAfterNow).ToLocalTime();
            TimeSpan tsx = dt.Subtract(dtBase);
            return tsx.Ticks / TICK_BASE;
        }

        /// <summary>
        /// 日期時間轉換為時間戳
        /// </summary>
        /// <param name="dt">日期時間</param>
        /// <returns>時間戳</returns>
        public static long ConvertToTimestamp(DateTime dt)
        {
            TimeSpan tsx = dt.Subtract(dtBase);
            return tsx.Ticks / TICK_BASE;
        }

        /// <summary>
        /// 從UNIX時間戳轉換為DateTime
        /// </summary>
        /// <param name="timestamp">時間戳字符串</param>
        /// <returns>日期時間</returns>
        public static DateTime ConvertToDateTime(string timestamp)
        {
            long ticks = long.Parse(timestamp) * TICK_BASE;
            return dtBase.AddTicks(ticks);
        }

        /// <summary>
        /// 從UNIX時間戳轉換為DateTime
        /// </summary>
        /// <param name="timestamp">時間戳</param>
        /// <returns>日期時間</returns>
        public static DateTime ConvertToDateTime(long timestamp)
        {
            long ticks = timestamp * TICK_BASE;
            return dtBase.AddTicks(ticks);
        }

        /// <summary>
        /// 檢查Ctx是否過期，我們給當前時間加上一天來看看是否超過了過期時間
        /// 而不是直接比較是否超過了過期時間，是給這個文件最大1天的上傳持續時間
        /// </summary>
        /// <param name="expiredAt"></param>
        /// <returns></returns>
        public static bool IsContextExpired(long expiredAt)
        {
            if (expiredAt == 0)
            {
                return false;
            }
            bool expired = false;
            DateTime now = DateTime.Now.AddDays(1);
            long nowTs = ConvertToTimestamp(now);
            if (nowTs > expiredAt)
            {
                expired = true;
            }
            return expired;
        }

    }
}