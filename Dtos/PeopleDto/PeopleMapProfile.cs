using AutoMapper;
using SecretGarden.Model;
using SecretGarden.PeopleDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretGarden.Dtos.PeopleDto
{
    public class PeopleMapProfile :Profile
    {
        public PeopleMapProfile()
        {
            CreateMap<AddPeopleDto, People>()
              .ForMember(Target => Target.CreateTime, (map) => map.MapFrom(soure => DateTime.Now));
        }
    }

 
}
