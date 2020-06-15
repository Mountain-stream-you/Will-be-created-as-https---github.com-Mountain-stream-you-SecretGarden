using PointsMall.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PointsMall.Common
{
    public class IDCardHelper
    {
        /// <summary>
        /// 解析身份证件号信息
        /// </summary>
        /// <param name="idCardNo">证件号号码</param>
        /// <returns></returns>
        public static IDCardInfo GetIDCardInfo(string idCardNo)
        {
            if (string.IsNullOrEmpty(idCardNo) || (idCardNo.Length != 15 && idCardNo.Length != 18))
                throw ExceptionHelper.InvalidArgumentException("身份证件号为空或不合法");

            IDCardInfo entity = new IDCardInfo();
            string strSex = string.Empty;
            if (idCardNo.Length == 18)//处理18位的身份证号码从号码中得到生日和性别代码
            {
                entity.Birthday = idCardNo.Substring(6, 4) + "-" + idCardNo.Substring(10, 2) + "-" + idCardNo.Substring(12, 2);
                strSex = idCardNo.Substring(14, 3);
            }
            if (idCardNo.Length == 15)
            {
                entity.Birthday = "19" + idCardNo.Substring(6, 2) + "-" + idCardNo.Substring(8, 2) + "-" + idCardNo.Substring(10, 2);
                strSex = idCardNo.Substring(12, 3);
            }
            entity.Age = CalculateAge(entity.Birthday);//根据生日计算年龄

            if (int.Parse(strSex) % 2 == 0)//性别代码为偶数是女性奇数为男性
                entity.Sex = "女";
            else
                entity.Sex = "男";
            return entity;
        }
        /// <summary>
        /// 根据出生日期，计算精确的年龄
        /// </summary>
        /// <param name="birthday">生日</param>
        /// <returns></returns>
        public static int CalculateAge(string birthday)
        {
            DateTime birthDate = DateTime.Parse(birthday);
            DateTime nowDateTime = DateTime.Now;
            int age = nowDateTime.Year - birthDate.Year;
            //再考虑月、天的因素
            if (nowDateTime.Month < birthDate.Month || (nowDateTime.Month == birthDate.Month && nowDateTime.Day < birthDate.Day))
                age--;
            return age;
        }


        /// <summary>
        /// 获取出生日期
        /// </summary>
        /// <param name="idCardNo"></param>
        /// <returns></returns>
        public static string GetBirthDay(string idCardNo)
        {
            if (string.IsNullOrEmpty(idCardNo) || (idCardNo.Length != 15 && idCardNo.Length != 18))
                throw ExceptionHelper.InvalidArgumentException("身份证件号为空或不合法");

            if (idCardNo.Length == 18)//处理18位的身份证号码从号码中得到生日
            {
                return idCardNo.Substring(6, 4) + "-" + idCardNo.Substring(10, 2) + "-" + idCardNo.Substring(12, 2);
            }
            else 
            {
                return "19" + idCardNo.Substring(6, 2) + "-" + idCardNo.Substring(8, 2) + "-" + idCardNo.Substring(10, 2);                
            }
        }

        /// <summary>
        /// 获取性别
        /// </summary>
        /// <param name="idCardNo"></param>
        /// <returns></returns>
        public static string GetGenderStr(string idCardNo)
        {
            var strGender = string.Empty;
            if (string.IsNullOrEmpty(idCardNo) || (idCardNo.Length != 15 && idCardNo.Length != 18))
                throw ExceptionHelper.InvalidArgumentException("身份证件号为空或不合法");

            if (idCardNo.Length == 18)//处理18位的身份证号码从号码中得到生日和性别代码
            {
                strGender = idCardNo.Substring(14, 3);                
            }
            else
            {
                strGender = idCardNo.Substring(12, 3);
            }
            return strGender;
        }

        /// <summary>
        /// 校验身份证是否有效
        /// </summary>
        /// <param name="idCardNo"></param>
        /// <returns></returns>
        public static bool GetIsValid(string idCardNo) {
            if (idCardNo.Length == 18)
            {
                return CheckIDCard18(idCardNo);
            }
            else if (idCardNo.Length == 15)
            {
                return CheckIDCard15(idCardNo);
            }
            else
            {
                return false;
            }
        }


        /// <summary>  
        /// 18位身份证号码验证  
        /// </summary>  
        public static  bool CheckIDCard18(string idNumber)
        {
            long n = 0;
            if (long.TryParse(idNumber.Remove(17), out n) == false
                || n < Math.Pow(10, 16) || long.TryParse(idNumber.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证  
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(idNumber.Remove(2)) == -1)
            {
                return false;//省份验证  
            }
            string birth = idNumber.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证  
            }
            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = idNumber.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != idNumber.Substring(17, 1).ToLower())
            {
                return false;//校验码验证  
            }
            return true;//符合GB11643-1999标准  
        }

        /// <summary>  
        /// 15位身份证号码验证  
        /// </summary>  
        public static bool CheckIDCard15(string idNumber)
        {
            long n = 0;
            if (long.TryParse(idNumber, out n) == false || n < Math.Pow(10, 14))
            {
                return false;//数字验证  
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(idNumber.Remove(2)) == -1)
            {
                return false;//省份验证  
            }
            string birth = idNumber.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证  
            }
            return true;
        }
    }
    /// <summary>
    /// 定义识别信息实体
    /// </summary>
    public class IDCardInfo
    {
        /// <summary>
        /// 生日
        /// </summary>
        public string Birthday { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }
    }
}
