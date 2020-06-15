using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PointsMall.Common
{
    public static class JsonAttributeHelper<E>
    {
        /// <summary>
        /// 获取JsonPropertyAttribute构造参数值
        /// </summary>
        /// <typeparam name="Result"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string DataMemberName<Result>(Expression<Func<E, Result>> expression)
        {
            var body = expression.Body as MemberExpression;
            var attr = body.Member.CustomAttributes.FirstOrDefault(t => t.AttributeType == typeof(JsonPropertyAttribute));
            if (attr == null)
                return "";

            if (attr.ConstructorArguments.Count > 0)
                return attr.ConstructorArguments.First().Value.ToString();
            else
            {
                var property = attr.NamedArguments.FirstOrDefault(t => t.MemberName == "PropertyName");
                return property == null ? "" : property.TypedValue.Value.ToString();
            }
        }

        ///// <summary>
        ///// 取消跟踪DbContext中所有被跟踪的实体
        ///// </summary>
        //public static void DetachAll(this DbContext dbContext)
        //{
        //    //循环遍历DbContext中所有被跟踪的实体
        //    while (true)
        //    {
        //        //每次循环获取DbContext中一个被跟踪的实体
        //        var currentEntry = dbContext.ChangeTracker.Entries().FirstOrDefault();

        //        //currentEntry不为null，就将其State设置为EntityState.Detached，即取消跟踪该实体
        //        if (currentEntry != null)
        //        {
        //            //设置实体State为EntityState.Detached，取消跟踪该实体，之后dbContext.ChangeTracker.Entries().Count()的值会减1
        //            currentEntry.State = EntityState.Detached;
        //        }                
        //        else
        //        {
        //            break;
        //        }
        //    }
        //}


    }
}
