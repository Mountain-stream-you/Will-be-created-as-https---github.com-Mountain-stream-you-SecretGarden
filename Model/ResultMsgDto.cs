using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretGarden.Model
{
    public class ResultMsgDto
    {
        /// <summary>
        /// 编码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        ///  返回信息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 总条数
        /// </summary>
        public int Count { get; set; }
    }
}
