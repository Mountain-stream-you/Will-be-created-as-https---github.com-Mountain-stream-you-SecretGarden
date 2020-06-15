using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretGarden.Dtos.PeopleDto
{
    public class PeopleLoginDto
    {
        /// <summary>
        /// 网名
        /// </summary>
        public string NetName { get; set; }
        
        /// <summary>
        /// 登录密码
        /// </summary>
        public string Password { get; set; }
        
        ///// <summary>
        ///// 真实姓名
        ///// </summary>
        //public string PeopleName { get; set; }
        
        ///// <summary>
        ///// 本人身份证
        ///// </summary>
        //public string IdCard { get; set; }
    }
}
