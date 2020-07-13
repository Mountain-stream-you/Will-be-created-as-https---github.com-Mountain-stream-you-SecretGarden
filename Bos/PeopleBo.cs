using Microsoft.Extensions.Logging;
using NLog;
using PointsMall.Common;
using SecretGarden.Dtos.PeopleDto;
using SecretGarden.Dtos.ReleaseInformationDto;
using SecretGarden.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SecretGarden.Bos
{
    public class PeopleBo :BoBase
    {
        public People People { get; private set; }
        Logger _logger = LogManager.GetCurrentClassLogger();
        public PeopleBo(People people)
        {
            this.People = people;
        }

        /// <summary>
        /// 创建约会记录
        /// </summary>
        /// <param name="releaseDto"></param>
        internal bool AddRelease(ReleaseDto releaseDto)
        {
            var release= _boProvider._mapper.Map<ReleaseInformation>(releaseDto);
            //添加约会记录
           var result=  _boProvider._peopleRepo.addRelease(release);
            return result;
          //配对有缘人

        }

        /// <summary>
        /// 检查Dto是否合法
        /// </summary>
        /// <param name="releaseDto"></param>
        internal void CheckreleaseDto(ReleaseDto releaseDto)
        {
        if(releaseDto.Convention>releaseDto.FailureTime)
            {
                throw ExceptionHelper.InvalidDataException($"失效时间不能小于约定时间");
            }
        }

        /// <summary>
        /// 设置密码和用户名
        /// </summary>
        /// <param name="addLoginDto"></param>
        internal bool SetPayPassword(AddLoginDto addLoginDto)
        {
            var salt = CreateSalt();
            var pwd = BitConverter.ToString(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(addLoginDto.Password + salt))).Replace("-", "");
           
            People.Password = pwd;
            People.NetName = addLoginDto.NetName;
            People.Salt = salt;
           return _boProvider._peopleRepo.SetPayPassword(People);
        }
        /// <summary>
        /// 生成加密用的盐值
        /// </summary>
        /// <returns></returns>
        internal string CreateSalt()
        {
            byte[] randomBytes = new byte[16];
            RNGCryptoServiceProvider rngServiceProvider = new RNGCryptoServiceProvider();
            rngServiceProvider.GetBytes(randomBytes);
            var result = BitConverter.ToInt64(randomBytes, 0);
            var rng = Math.Abs(result).ToString().PadLeft(32, '0');  //求绝对值
            return rng;
        }
    }
}
