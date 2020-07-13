using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretGarden.Dtos.PeopleDto
{
    public class ResetPasswordsDto
    {
        /// <summary>
        /// 证件号码
        /// </summary>
        public string IdCard { get; set; }

        /// <summary>
        /// 用户邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string VerificationCode { get; set; }

        /// <summary>
        /// 重置后的密码
        /// </summary>
        public string Newpassword { get; set; }
    }
}
