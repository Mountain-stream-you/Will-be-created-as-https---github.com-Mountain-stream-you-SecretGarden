using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SecretGarden.PeopleDto
{
    public class AddPeopleDto
    {
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// 用户名
        /// </summary>
        public string NetName { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        public string PassWord { get; set; }


        /// <summary>
        /// 性别类型
        /// </summary>
        [Required]
        public string Sex { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public string Age { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        [Required]
        public string IdType { get; set; } = "身份证";

        /// <summary>
        /// 证件号码
        /// </summary>
        public string PeopleIdNumber { get; set; }

        /// <summary>
        /// 电话号码
        /// </summary>
        [Phone]
        [StringLength(20)]
        public string Phone { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime BirthDay { get; set; }
    }
}
