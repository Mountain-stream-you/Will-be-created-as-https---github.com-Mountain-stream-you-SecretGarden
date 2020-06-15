
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PointsMall.Common
{

    public static class EnumHelper
    {
        public static List<KeyValuePair<string, string>> EnumListFromDescriptions<T>(bool IsHasAll = false, int? keyOfAll = null, string valueOfAll = "--所有--")
        {
            return EnumListFromDescriptions<T>(null, IsHasAll, keyOfAll, valueOfAll);
        }
        /// <summary>
        /// 取枚举的自定义属性的描述值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fliter"></param>
        /// <param name="IsHasAll"></param>
        /// <param name="keyOfAll"></param>
        /// <param name="valueOfAll"></param>
        /// <returns></returns>
        public static List<KeyValuePair<string, string>> EnumListFromDescriptions<T>(T[] fliter = null, bool IsHasAll = false, int? keyOfAll = null, string valueOfAll = "--所有--")
        {
            return EnumList(fliter, true, IsHasAll, keyOfAll, valueOfAll);
        }

        public static List<KeyValuePair<string, string>> EnumListFromEnumName<T>(bool IsHasAll = false, int? keyOfAll = null, string valueOfAll = "--所有--")
        {
            return EnumListFromEnumName<T>(null, IsHasAll, keyOfAll, valueOfAll);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fliter">要过滤掉的枚举数组</param>
        /// <param name="IsHasAll"></param>
        /// <param name="keyOfAll"></param>
        /// <param name="valueOfAll"></param>
        /// <returns></returns>
        public static List<KeyValuePair<string, string>> EnumListFromEnumName<T>(T[] fliter = null, bool IsHasAll = false, int? keyOfAll = null, string valueOfAll = "--所有--")
        {
            return EnumList(fliter, false, IsHasAll, keyOfAll, valueOfAll);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fliter">要过虑的枚举数组</param>
        /// <param name="isDescription"></param>
        /// <param name="IsHasAll"></param>
        /// <param name="keyOfAll"></param>
        /// <param name="valueOfAll"></param>
        /// <returns></returns>
        private static List<KeyValuePair<string, string>> EnumList<T>(T[] fliter, bool isDescription, bool IsHasAll, int? keyOfAll, string valueOfAll)
        {
            List<KeyValuePair<string, string>> listEnum = new List<KeyValuePair<string, string>>();
            Type enumType = typeof(T);
            if (!enumType.IsEnum)
            {
                return listEnum;
            }
            if (IsHasAll) //判断是否添加默认选项,默认项排前面           
            {
                listEnum.Add(new KeyValuePair<string, string>(keyOfAll.HasValue ? keyOfAll.Value.ToString() : "", valueOfAll));
            }
            var EnumTypeValueList = enumType.GetEnumValues(); //获取枚举字段数组            
            foreach (var item in EnumTypeValueList)
            {
                if (fliter != null && fliter.Contains((T)item))
                    continue;
                string description = string.Empty;
                if (!isDescription) //不取描述信息自定义参数
                    description = item.ToString();  //描述不存在取字段名称                
                else
                {
                    object[] arr = enumType.GetField(item.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), true); //获取属性字段数组                
                    if (arr != null && arr.Length > 0)
                        description = ((DescriptionAttribute)arr[0]).Description;   //属性描述                
                    else
                        description = item.ToString();  //描述不存在取字段名称                
                }
                listEnum.Add(new KeyValuePair<string, string>(description, description));  //不用枚举的value值作为字典key值的原因从枚举例子能看出来，其实这边应该判断他的值不存在，默认取字段名称           
            }
            return listEnum;
        }

        /// <summary>
        /// 将字符串转换为枚举类型值
        /// </summary>
        /// <typeparam name="E">枚举类型</typeparam>
        /// <param name="enumName">字符串</param>
        /// <returns>枚举值</returns>
        public static E ConvertToEnum<E>(string enumName)
        {
            if (!string.IsNullOrWhiteSpace(enumName))
            {
                return (E)Enum.Parse(typeof(E), enumName);
            }

            return default(E);
        }


        /// <summary>
        /// 获取枚举类型下拉数据
        /// </summary>
        /// <param name="t"></param>        
        /// <returns></returns>
        public static IReadOnlyList<dynamic> EnumDropDownList(Type t)
        {
            //获取枚举类型值
            List<dynamic> list = new List<dynamic>();            
            foreach (var item in Enum.GetValues(t))
            {
                var model = new
                {
                    value = Enum.Parse(t, item.ToString()),
                    text = item.ToString()
                };
                list.Add(model);
            }
            return list;
        }
    }

    /// <summary>
    /// 备注特性
    /// </summary>
    public class DescriptionAttribute : Attribute
    {
        public DescriptionAttribute(string description)
        {
            Description = description;
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string Description { get; internal set; }
    }

    /// <summary>
    /// 枚举扩展类
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// 获取枚举的备注信息
        /// </summary>
        /// <param name="em"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum em)
        {
            Type type = em.GetType();
            FieldInfo fd = type.GetField(em.ToString());
            if (fd == null)
                return string.Empty;
            object[] attrs = fd.GetCustomAttributes(typeof(DescriptionAttribute), false);
            string name = string.Empty;
            foreach (DescriptionAttribute attr in attrs)
            {
                name = attr.Description;
            }
            return name;
        }
    }


}
