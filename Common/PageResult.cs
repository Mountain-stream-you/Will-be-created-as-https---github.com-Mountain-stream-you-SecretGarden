namespace PointsMall.Common
{
    /// 属性名统一为小写，避免Json序列化后属性首字母还是大写，与Table要的格式不匹配
    /// <summary>
    /// 返回的页数据
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class PageResult<TData>
    {
        /// <summary>
        /// 查询结果状态码 : LayUI Table 默认要求的参数名与值，成功默认为：0;成功不用设置
        /// </summary>
        public int Code { get; set; } = 0;

        /// <summary>
        /// 结果信息
        /// </summary>
        public string Msg { get; set; } = string.Empty;

        /// <summary>
        /// 总录数: LayUI Table 默认要求的参数名
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 每页显示的条数
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 获取或设置当前页码的数据
        /// </summary>
        public TData Data { get; set; }
    }

    public class FyuPageResult<TData>  : PageResult<TData>
    {
          /// <summary>
         /// 总条数
        /// </summary>
        public int TotalNumber { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPage { get; set; }
    }
}
