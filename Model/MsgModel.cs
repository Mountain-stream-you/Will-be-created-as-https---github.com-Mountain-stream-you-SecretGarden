using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretGarden.Model
{
    public class MsgModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

        public int Status { get; set; }

        public string Content { get; set; }

        public string Remark { get; set; }
    }
}
