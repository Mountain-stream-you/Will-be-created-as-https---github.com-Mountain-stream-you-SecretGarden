using AutoMapper;
using System.Collections.Generic;

namespace PointsMall.Common
{
    public static class PageHelper
    {
        public static FyuPageResult<List<TDto>> FyuToPageDto<TModel, TDto>(this PageResult<List<TModel>> pageModel, IMapper mapper)
        {
            FyuPageResult<List<TDto>> pageDto = new FyuPageResult<List<TDto>>()
            {
                Code = pageModel.Code,
                Count = pageModel.Count,
                PageIndex = pageModel.PageIndex,
                PageSize = pageModel.PageSize,
                TotalNumber = pageModel.Count,
                TotalPage = (pageModel.Count + pageModel.PageSize - 1) / pageModel.PageSize,
                Data = mapper.Map<List<TDto>>(pageModel.Data)
            };
            return pageDto;
        }

        public static PageResult<List<TDto>> ToPageDto<TModel, TDto>(this PageResult<List<TModel>> pageModel, IMapper mapper)
        {
            PageResult<List<TDto>> pageDto = new PageResult<List<TDto>>()
            {
                Code = pageModel.Code,
                Count = pageModel.Count,
                Msg = pageModel.Msg,
                PageIndex = pageModel.PageIndex,
                PageSize = pageModel.PageSize,
                Data = mapper.Map<List<TDto>>(pageModel.Data)
            };
            return pageDto;
        }

    }
}
