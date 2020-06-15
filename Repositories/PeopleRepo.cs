using Castle.Core.Internal;
using Newtonsoft.Json;
using PointsMall.Common;
using SecretGarden.Bos;
using SecretGarden.Dtos.PeopleDto;
using SecretGarden.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretGarden.Repositories
{
    public class PeopleRepo
    {
        public SecretGardenContext _context;
        public PeopleRepo(SecretGardenContext pointsMallContext)
        {
            _context = pointsMallContext;
        }

        /// <summary>
        /// 添加人员
        /// </summary>
        /// <param name="people"></param>
        internal void AddPeople(People people)
        {
            _context.Peoples.Add(people);
            _context.SaveChanges();
        }

        internal People GetPeopleById(int id)
        {
            var people = _context.Peoples.Find(id);
            return people;
        }

        /// <summary>
        /// 添加约会记录
        /// </summary>
        /// <param name="release"></param>
        internal void addRelease(ReleaseInformation release)
        {
            _context.ReleaseInformations.Add(release);
            _context.SaveChanges();
        }

        /// <summary>
        /// 通过身份证获取用户
        /// </summary>
        /// <param name="idCard"></param>
        /// <returns></returns>
        internal People GetPeopleBoByIdCard(string idCard)
        {
          var people=  _context.Peoples.Where(m => m.PeopleIdNumber == idCard).FirstOrDefault();
            return people;
        }

        /// <summary>
        /// 找出各个城市的人
        /// </summary>
        internal IEnumerable<IGrouping<string,ReleaseInformation>>  GetPeopleByCity()
        {
            var result = _context.ReleaseInformations.ToList().GroupBy(m => m.Place);
            return result;

           
            
          //  return arrayList;
        }

        /// <summary>
        /// 设置密码和用户名
        /// </summary>
        /// <param name="people"></param>
        internal bool SetPayPassword(People people)
        {
            _context.Peoples.Update(people);
            _context.SaveChanges();
            return true;
        }

        /// <summary>
        /// 通过邮箱重置密码
        /// </summary>
        /// <param name="peopleBo"></param>
        /// <returns></returns>
        internal bool ResetPassword(PeopleBo peopleBo)
        {
            _context.Update(peopleBo.People);
            _context.SaveChanges();
            return true;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="peopleLoginDto"></param>
        /// <returns></returns>
        internal object Login(PeopleLoginDto peopleLoginDto)
        {
            var result= _context.Peoples.Any(m=>m.NetName==peopleLoginDto.NetName && m.Password==peopleLoginDto.Password);
        if(result)
            {
                return true;
            }
            else
            {
                return JsonConvert.SerializeObject(new ResultMsgDto() { Code = 500, Msg = $"用户名或密码输入不正确" });
            }
        }
    }
}
