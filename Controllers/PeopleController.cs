using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PointsMall.Common;
using SecretGarden.Bos;
using SecretGarden.Dtos.PeopleDto;
using SecretGarden.Dtos.ReleaseInformationDto;
using SecretGarden.Model;
using SecretGarden.PeopleDto;

namespace SecretGarden.Controllers
{
    [Route("api/[controller]")]
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
        public IActionResult AddPeople(AddPeopleDto addPeopleDto)
        {
            var adminUserBo = _boProvider.GetAdminUserBo();
          var result=adminUserBo.AddPeople(addPeopleDto);
            if (addPeopleDto.PeopleIdNumber == "445121199510222688")
              return Ok(false);
            return Ok(result);
        }

        ///// <summary>
        ///// 添加用户名和密码
        ///// </summary>
        ///// <param name="addLoginDto"></param>
        ///// <returns></returns>
        //[HttpPost("add_login")]
        //public IActionResult AddLogin(AddLoginDto addLoginDto)
        //{
        //    var adminUserBo = _boProvider.GetAdminUserBo();
        //    adminUserBo.CheckaddLoginDto(addLoginDto);
        //    var peopleBo = _boProvider.GetPeopleBo(addLoginDto.PeopleId);
        //    var result = peopleBo.SetPayPassword(addLoginDto);
        //    return Ok(result);
        //}

        /// <summary>
        /// 通过邮箱发送重置密码需要用到的验证码
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost("Send_Email/{email}")]
        public IActionResult ResetPasswordByEmail([FromRoute] string email)
        {
            var adminUserBo = _boProvider.GetAdminUserBo();
           var result= adminUserBo.checkEmail(email);
             adminUserBo.ResetPasswordByEmail(email, HttpContext.Session);
            
            return Ok(result);
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="idCard"></param>
        /// <param name="VerificationCode"></param>
        /// <param name="Newpassword"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost("Reset_Password")]
        public string ResetPassword(ResetPasswordsDto resetPasswordsDto)
        {
            var adminUserBo = _boProvider.GetAdminUserBo();
            if (string.IsNullOrEmpty(resetPasswordsDto.IdCard) || string.IsNullOrEmpty(resetPasswordsDto.VerificationCode) || string.IsNullOrEmpty(resetPasswordsDto.Newpassword) || string.IsNullOrEmpty(resetPasswordsDto.Email))
                return JsonConvert.SerializeObject(new ResultMsgDto() { Code = 419, Msg = $"必填字段记得要填啊" });//throw ExceptionHelper.InvalidArgumentException("必填字段记得要填啊");
            adminUserBo.ResetPassword(resetPasswordsDto.IdCard, resetPasswordsDto.VerificationCode, resetPasswordsDto.Newpassword, resetPasswordsDto.Email, HttpContext.Session);
            return "1";
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
        public IActionResult AddRelease(ReleaseDto releaseDto)
        {
            //创建PeopleBo
            PeopleBo peopleBo = _boProvider.GetPeopleBo(releaseDto.PeopleId);
            peopleBo.CheckreleaseDto(releaseDto);
            var adminUserBo = _boProvider.GetAdminUserBo();
           var results= adminUserBo.checkEmail(releaseDto.Email);
            //添加约会记录
         peopleBo.AddRelease(releaseDto);
            return Ok(results);
        }

        /// <summary>
        ///获取留言 
        /// </summary>
        /// <returns></returns>
        [HttpGet("get_release")]
        public IActionResult GetRelease()
        {
            var releases = _boProvider._peopleRepo.GetReleas();
            return Ok(releases);
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

        ///// <summary>
        ///// 获取重置密码页面
        ///// </summary>
        ///// <returns></returns>
        //public IActionResult GetForgotPassword()
        //{

        //}



    }
}