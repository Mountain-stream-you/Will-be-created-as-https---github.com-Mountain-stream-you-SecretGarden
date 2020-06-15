using AutoMapper;
using NLog.Targets;
using SecretGarden.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretGarden.Dtos.ReleaseInformationDto
{
    public class ReleaseProfile :Profile
    {
        public ReleaseProfile()
        {
            CreateMap<ReleaseDto, ReleaseInformation>();
              //  .ForMember(Target=>Target.Guid,(map)=>map.MapFrom(soure=>new Guid()))
              
        }
    }
}
