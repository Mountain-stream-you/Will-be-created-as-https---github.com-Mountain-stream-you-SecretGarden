using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PointsMall.Common
{
    public static class RandomHelper
    {
        /// <summary>
        /// 获得三位的随机数
        /// </summary>
        /// <returns></returns>
        public static string GetNumRandom(int digit = 3)
        {
            Random ro = new Random((int)DateTime.Now.Ticks);
            int iResult;
            int iDown = Convert.ToInt32("1".PadRight(digit, '0'));
            int iUp = Convert.ToInt32("9".PadRight(digit, '9'));
            iResult = ro.Next(iDown, iUp);
            return iResult.ToString().Trim();
        }

        /// <summary>
        /// 转换金额保留两位小数三位分隔
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        public static string ConvertPrice(decimal price)
        {
            string resultPrice = "0.00";
            if (price > 0)
                resultPrice = string.Format("{0:N}", price);

            return resultPrice;
        }
    }
}
