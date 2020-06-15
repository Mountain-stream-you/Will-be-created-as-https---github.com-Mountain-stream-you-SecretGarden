using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SecretGarden.Model
{
    public class ReleaseInformation :BaseModel
    {
        /// <summary>
        /// 人员Id
        /// </summary>
        public int PeopleId { get; set; }
    
        /// <summary>
        /// 关联人员表
        /// </summary>
        [ForeignKey("PeopleId")]
       public virtual People People { get; set; }

        /// <summary>
        /// 约定时间
        /// </summary>
        [StringLength(20)]
        [Column(TypeName = "DateTime")]
        public DateTime Convention { get; set; }
    
        /// <summary>
        /// 约定地点
        /// </summary>
        public string Place { get; set; }

        /// <summary>
        /// 失效时间
        /// </summary>
        [Column(TypeName ="DateTime")]
        public DateTime FailureTime { get; set; }

        /// <summary>
        /// 备用邮箱
        /// </summary>
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// 留言
        /// </summary>
        public string LeaveMessage { get; set; }
    }
}
