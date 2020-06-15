using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SecretGarden.Bos;
using SecretGarden.Dtos.PeopleDto;
using SecretGarden.Dtos.ReleaseInformationDto;
using SecretGarden.Model;
using SecretGarden.PeopleDto;

namespace SecretGarden.Controllers
{
    [Route("api/people")]
    public class PeopleController : Controller
    {
        private readonly BoProvider _boProvider;
        private NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        public PeopleController(BoProvider boProvider)
        {
            _boProvider = boProvider;
        }


        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="addPeopleDto"></param>
        [HttpPost("add_People")]
        public void AddPeople(AddPeopleDto addPeopleDto)
        {
           var adminUserBo= _boProvider.GetAdminUserBo();
            adminUserBo.CheckPeopleDto(addPeopleDto);
            adminUserBo.AddPeople(addPeopleDto);
            //adminUserBo.SetPayPassword()
        }

        /// <summary>
        /// 添加用户名和密码
        /// </summary>
        /// <param name="addLoginDto"></param>
        /// <returns></returns>
        [HttpPost("add_login")]
        public IActionResult AddLogin(AddLoginDto addLoginDto)
        {
            var adminUserBo = _boProvider.GetAdminUserBo();
            adminUserBo.CheckaddLoginDto(addLoginDto);
            var peopleBo = _boProvider.GetPeopleBo(addLoginDto.PeopleId);
            var result= peopleBo.SetPayPassword(addLoginDto);
            return Ok(result);
        }

        /// <summary>
        /// 通过邮箱发送重置密码需要用到的验证码
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost("Send_Email/{email}")]
        public IActionResult ResetPasswordByEmail([FromRoute] string email)
        {
            var adminUserBo = _boProvider.GetAdminUserBo();
            var result = adminUserBo.ResetPasswordByEmail(email);
            return Ok(result);
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="idCard"></param>
        /// <param name="VerificationCode"></param>
        /// <param name="password"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost("Reset_Password/{idCard}")]
        public IActionResult ResetPassword([FromRoute]string idCard, string VerificationCode,string password,string email)
        {
            var adminUserBo = _boProvider.GetAdminUserBo();
            var result = adminUserBo.ResetPassword(idCard, VerificationCode, password,email);
            return Ok(result);
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <returns></returns>
        [HttpPost("people_login")]
        public IActionResult Login(PeopleLoginDto peopleLoginDto)
        {
            var adminUserBo = _boProvider.GetAdminUserBo();
            var result = adminUserBo.Login(peopleLoginDto);
            return Ok(result);
        }

        /// <summary>
        /// 添加约会记录
        /// </summary>
        [HttpPost("add_Release")]
        public void AddRelease(ReleaseDto releaseDto)
        {
            //创建PeopleBo
            PeopleBo peopleBo = _boProvider.GetPeopleBo(releaseDto.PeopleId);
            peopleBo.CheckreleaseDto(releaseDto);
             //添加约会记录
            peopleBo.AddRelease(releaseDto);
        }

        /// <summary>
        /// 寻找有缘人
        /// </summary>
        /// <returns></returns>
        [HttpGet("Pairing_People")]
        public IActionResult Pairing()
        {
            //分组各个城市的人
            var adminUserBo = _boProvider.GetAdminUserBo();
            adminUserBo.GetPeopleByCity();
            return Ok();
        }


        
    }
}