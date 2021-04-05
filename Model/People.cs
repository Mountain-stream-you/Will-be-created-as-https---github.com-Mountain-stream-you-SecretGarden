
using SecretGarden.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SecretGarden.Model 
{
    public class People : BaseModel
    {
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 网名
        /// </summary>
        public string NetName { get; set; }

        /// <summary>
        /// 性别类型
        /// </summary>
        [Required]
        [Column(TypeName = "enum('男','女')")]
        public string Sex { get; set; }

      //  public GenderEnum SexEnum { get => System.Enum.Parse<GenderEnum>(Sex); }

        /// <summary>
        /// 登录密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public string Age { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        [Required]
        [StringLength(20)]
        [Column(TypeName = "enum('身份证')")]
        public string IdType{ get; set; }

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
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime BirthDay { get; set; }

        /// <summary>
        /// 加密的用盐值
        /// </summary>
        public string Salt { get; set; }
    }
}
