using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretGarden.Dtos.PeopleDto
{
    public class AddLoginDto
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public int PeopleId { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 网名
        /// </summary>
        public string NetName { get; set; }
    }
}
