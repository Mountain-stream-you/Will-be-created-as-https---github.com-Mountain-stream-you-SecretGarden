using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SecretGarden.Dtos.ReleaseInformationDto
{
    public class ReleaseDto
    {
        /// <summary>
        /// 创建人Id
        /// </summary>
        public int PeopleId { get; set; }


        public Guid Guid { get; set; } =Guid.NewGuid();

        /// <summary>
        /// 约定时间
        /// </summary>
        public DateTime Convention { get; set; }
    
        /// <summary>
        /// 约定地点
        /// </summary>
        public string Place { get; set; }

        /// <summary>
        ///失效时间 
        /// </summary>
        [Column(TypeName = "DateTime")]
        public DateTime FailureTime { get; set; }

        /// <summary>
        /// 联系邮箱
        /// </summary>
        [EmailAddress()]
        public String Email { get; set; }

        /// <summary>
        /// 留言
        /// </summary>
        public string leaveMessage { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        //[Column(TypeName = "DateTime")]
        //public DateTime CreateTime { get; set; } = DateTime.Now;
    }

}
