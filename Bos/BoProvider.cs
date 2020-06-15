using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using SecretGarden.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretGarden.Bos
{
    public class BoProvider
    {
        public SecretGardenContext _context;
        public IMapper _mapper;
        public PeopleRepo _peopleRepo;
        public IDistributedCache _cache { get; set; }
        public BoProvider(
            SecretGardenContext context
            , IMapper mapper
            , PeopleRepo peopleRepo
            , IDistributedCache cache)
        {
            _context = context;
            _mapper = mapper;
            _peopleRepo = peopleRepo;
            _cache = cache;
        }

        internal AdminUserBo GetAdminUserBo()
        {
            return new AdminUserBo() { 
            _boProvider=this
            };
        }

        internal PeopleBo GetPeopleBo(int id)
        {
            var people = _peopleRepo.GetPeopleById(id);
            return new PeopleBo(people)
            { 
         _boProvider=this
            };
        }

        internal PeopleBo GetPeopleBoByIdCard(string  idCard)
        {
            var people = _peopleRepo.GetPeopleBoByIdCard(idCard);
            return new PeopleBo(people)
            {
                _boProvider = this
            };
        }

    }
}
