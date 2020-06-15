using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PointsMall.Common
{
    public static class ArgumentHelper
    {
        public static Regex rx = new Regex(@"^[1]([3-9])[0-9]{9}$");
        #region String Check

        public static string ToBase64(this string str)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(str));
        }

        public static void CheckNullOrEmpty(this string str, string errMsg = "")
        {
            if (string.IsNullOrEmpty(str))
                throw ExceptionHelper.InvalidArgumentException(errMsg);
        }

        /// <summary>
        /// 字符串数组不能为空
        /// </summary>
        /// <param name="ls"></param>
        /// <param name="errMsg"></param>
        public static void CheckStringListNull(this List<string> ls, string errMsg = "")
        {
            if (ls == null || ls.Count <= 0)
                throw ExceptionHelper.InvalidArgumentException(errMsg);
        }

        /// <summary>
        /// 判断字符串是否Null 或 空
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 是否有值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsExistAndNotEmpty(this string str)
        {
            if (str.IsNullOrEmpty())
                return false;
            return true;
        }

        public static bool IsDecimalPrice(this string priceStr)
        {
            if (decimal.TryParse(priceStr, out decimal price) == false)
                return false;
            return price.IsDecimalPrice();
        }

        #endregion

        #region Object Check or Is

        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        public static void CheckNull(this Object obj, string errMsg = "")
        {
            if (null == obj)
                throw ExceptionHelper.InvalidArgumentException(errMsg);
        }

        public static bool IsNull(this Object obj)
        {
            return null == obj;
        }

        public static bool IsExist(this Object obj)
        {
            return null != obj;
        }
        #endregion


        #region DateTime Check 

        /// <summary>
        /// 转为 2020-03-17 12:12:01 格式的日期时间字符串
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToChineseDateTimeString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// /// 转为 2020-03-17 格式的日期字符串
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToChineseDateString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 检查是否无效的时间,如0001-01-01 00:00:00
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="errMsg"></param>
        public static void CheckValidDateTime(this DateTime dateTime, string errMsg = "")
        {
            if (dateTime == default(DateTime))
                throw ExceptionHelper.InvalidArgumentException(errMsg);
        }

        /// <summary>
        /// 检查开始时间与结束时间是否是有效时间,且开始时间小于结束时间
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="errMsg"></param>
        public static void CheckBeginTimeBeforeEndTime(this DateTime beginTime, DateTime endTime, string errMsg = "")
        {
            beginTime.CheckValidDateTime(nameof(beginTime));
            endTime.CheckValidDateTime(nameof(endTime));
            if (beginTime >= endTime)
                throw ExceptionHelper.InvalidArgumentException(errMsg);
        }


        /// <summary>
        /// 检查开始时间与结束时间是否是有效时间,异常情况:开始时间> 结束时间
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="IsCanIncludeEqual"></param>
        /// <param name="errMsg"></param>
        public static void CheckBeginTimeOrNullBeforeEndTimeOrNull(this DateTime? beginTime, DateTime? endTime, bool IsCanIncludeEqual = true, string errMsg = "")
        {
            if (beginTime.HasValue == false && endTime.HasValue == false)
                return;

            if (beginTime.HasValue && endTime.HasValue == false)
                throw ExceptionHelper.InvalidArgumentException($"{nameof(endTime)} 无有效日期");
            if (beginTime.HasValue == false && endTime.HasValue)
                throw ExceptionHelper.InvalidArgumentException($"{nameof(beginTime)} 无有效日期");

            beginTime.Value.CheckValidDateTime(nameof(beginTime));
            endTime.Value.CheckValidDateTime(nameof(endTime));

            if (beginTime > endTime)
                throw ExceptionHelper.InvalidArgumentException(errMsg);

            else if (IsCanIncludeEqual == false && beginTime == endTime)
                throw ExceptionHelper.InvalidArgumentException(errMsg);
        }

        #endregion


        #region int and decimal Check Or Is
        /// <summary>
        /// 判断数字是否为0
        /// </summary>
        public static bool IsZero(this int count) => count == 0;


        /// <summary>
        /// Id int 参数不能为0
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="errMsg"></param>
        public static void CheckIntZero(this int Id, string errMsg = "")
        {
            if (Id <= 0)
                throw ExceptionHelper.InvalidArgumentException(errMsg);
        }

        /// <summary>
        /// Id int 参数不能小于0
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="errMsg"></param>
        public static void CheckIntLessThanZero(this int Id, string errMsg = "")
        {
            if (Id < 0)
                throw ExceptionHelper.InvalidArgumentException(errMsg + "不能小于0");
        }

        /// <summary>
        /// Decimal price  参数不能为0
        /// </summary>
        /// <param name="price"></param>
        /// <param name="errMsg"></param>
        public static void CheckDecimalZero(this decimal price, string errMsg = "")
        {
            if (price <= 0)
                throw ExceptionHelper.InvalidArgumentException(errMsg);
        }

        /// <summary>
        /// 检查小数点不能超过两位
        /// </summary>
        /// <param name="price"></param>
        /// <param name="errMsg"></param>
        public static void CheckLessThan2DecimalPlaces(this decimal price, string errMsg = "")
        {
            var strPrice = price.ToString();
            var startIndex = strPrice.IndexOf(".");
            if (startIndex == -1) //没有小数点的整数,直接返回
                return;
            int length = strPrice.Substring(strPrice.IndexOf(".")).Length - 1;
            if (length > 2)
                throw ExceptionHelper.InvalidArgumentException(errMsg + "小数位不能超过2位");
        }
        /// <summary>
        /// 价格检查须大于0: 1:不能小于等于0, 2:小数点后不能超过两位,
        /// </summary>
        /// <param name="price"></param>
        /// <param name="errMsg"></param>
        public static void CheckDecimalPrice(this decimal price, string errMsg = "")
        {
            price.CheckDecimalZero(errMsg);
            price.CheckLessThan2DecimalPlaces(errMsg);
        }

        public static void CheckDecimalMultiple(this decimal price,int multiple, string errMsg = "")
        {
           if(price% multiple!=0)
                throw ExceptionHelper.InvalidArgumentException($"必须是{multiple}的倍数");
        }

        /// <summary>
        /// 检查折扣必须在0~1之间
        /// </summary>
        /// <param name="discountRate"></param>
        /// <param name="errMsg"></param>
        public static void CheckDecimal_0To1(this decimal discountRate, string errMsg = "")
        {
            if (discountRate <= 0 || discountRate > 1)
                throw ExceptionHelper.InvalidArgumentException(errMsg + "必须在0~1之间");
        }

        /// <summary>
        /// 判断是否为有效的金额数字
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        public static bool IsDecimalPrice(this decimal price)
        {
            if (price <= 0)
                return false;

            var strPrice = price.ToString();
            var startIndex = strPrice.IndexOf(".");
            if (startIndex == -1) //没有小数点的整数,直接返回
                return true;
            int length = strPrice.Substring(strPrice.IndexOf(".")).Length - 1;
            if (length > 2)
                return false;
            return true;
        }
        #endregion

        #region List Check , IsNullOrEmpty
        /// <summary>
        /// 数组不能为空
        /// </summary>
        /// <param name="ls"></param>
        /// <param name="errMsg"></param>
        public static void CheckNullOrEmpty<T>(this List<T> ls, string errMsg = "")
        {
            if (ls == null || ls.Count <= 0)
                throw ExceptionHelper.InvalidArgumentException(errMsg);
        }

        /// <summary>
        /// 数组是否为Null 或 0
        /// </summary>
        /// <param name="value"></param>
        public static bool IsNullOrEmpty<T>(this ICollection<T> value)
        {
            return (value == null || value.Count <= 0);
        }

        #endregion

        #region MobilePhone Check
        /// <summary>
        /// 检查手机号码是否有效
        /// </summary>
        /// <param name="tel">电话号码</param>
        /// <param name="errMsg"></param>
        public static void CheckTelephoneIsValid(this string tel, string errMsg = "")
        {
            var result = tel.GetTelephoneValid();
            if (!result)
                throw ExceptionHelper.InvalidArgumentException(errMsg + "手机号码格式不合法");
        }

        /// <summary>
        /// 获取手机号码有效值
        /// </summary>
        /// <param name="tel">手机号码</param>
        public static bool GetTelephoneValid(this string tel)
        {
            //电信手机号正则
            string regexStr_DianXin = @"^1[35789][01379]\d{8}$";
            Regex regex_DianXin = new Regex(regexStr_DianXin);
            //联通手机号码正则
            string regexStr_LianTong = @"^1[34578][012456]\d{8}$";
            Regex regex_LianTong = new Regex(regexStr_LianTong);
            //移动手机号码正则
            string regexStr_YiDong = @"^(134[012345678]\d{7}|1[345678][012356789]\d{8})$";
            Regex regex_YiDong = new Regex(regexStr_YiDong);
            //验证手机号
            bool isTrue = regex_DianXin.IsMatch(tel) || regex_LianTong.IsMatch(tel) || regex_YiDong.IsMatch(tel);
            return isTrue;
        }
        #endregion


        /// <summary>
        /// 验证文件是否为Excel
        /// </summary>
        /// <param name="fileExtension">文件后缀</param>
        /// <param name="errMsg"></param>
        public static void CheckExcelFile(this string fileExtension, string errMsg = "")
        {
            if (fileExtension != ".xlsx" && fileExtension != ".xls")
                throw ExceptionHelper.InvalidOperationException($"导入的文件非Excel文件，文件类型错误");
        }

        //public static void CheckLogisticMsgNull(this List<LogisticMsg> LogisticMsg, string errMsg = "")
        //{
        //    if (LogisticMsg == null || LogisticMsg.Count == 0)
        //        throw ExceptionHelper.InvalidArgumentException(errMsg);
        //}

        public static string CheckPeopleName(this string str, string ErrorMsg)
        {
            string errorMsg = string.Empty;
            if (string.IsNullOrWhiteSpace(str))
            {
                errorMsg = ErrorMsg;
            }
            return errorMsg;
        }
        public static string CheckTelephone(this string str, string ErrorMsg)
        {
            string errorMsg = string.Empty;
            if (string.IsNullOrWhiteSpace(str) || !rx.IsMatch(str))
            {
                errorMsg = ErrorMsg;
            }
            return errorMsg;
        }

        public static string CheckPeopleIdNum(this string str, string ErrorMsg)
        {
            string errorMsg = string.Empty;
            if (string.IsNullOrWhiteSpace(str) || str.Length != 18 || !IDCardHelper.CheckIDCard18(str))
            {
                errorMsg = ErrorMsg;
            }
            return errorMsg;
        }
    }
}
