using Microsoft.Extensions.Caching.Distributed;
using MimeKit;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using PointsMall.Common;
using SecretGarden.Dtos.PeopleDto;
using SecretGarden.Model;
using SecretGarden.Model.Enum;
using SecretGarden.PeopleDto;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SecretGarden.Bos
{
    public class AdminUserBo : BoBase
    {
        private NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        public AdminUserBo()
        {

        }


        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="addPeopleDto"></param>
        internal void AddPeople(AddPeopleDto addPeopleDto)
        {
            var people = _boProvider._mapper.Map<People>(addPeopleDto);
            _boProvider._peopleRepo.AddPeople(people);
        }

        /// <summary>
        /// 校验peopledto合法性
        /// </summary>
        /// <param name="addPeopleDto"></param>
        internal void CheckPeopleDto(AddPeopleDto addPeopleDto)
        {

            if (_boProvider._context.Peoples.Any(m => m.NetName == addPeopleDto.NetName))
                throw ExceptionHelper.InvalidArgumentException("该用户名已存在");
            var result = IDCardHelper.GetIsValid(addPeopleDto.PeopleIdNumber);
            if (result == false)
                throw ExceptionHelper.InvalidArgumentException($"请输入合法的身份证");
            if(_boProvider._context.Peoples.Any(m=>m.PeopleIdNumber==addPeopleDto.PeopleIdNumber))
                throw ExceptionHelper.InvalidArgumentException($"该用户已经注册了");
            var addr = new MailAddress(addPeopleDto.Email);
            if (addr.Address != addPeopleDto.Email)
                throw ExceptionHelper.InvalidArgumentException($"请输入正确的邮箱地址");

            var sex = IDCardHelper.GetGenderStr(addPeopleDto.PeopleIdNumber);
            if (int.Parse(sex) % 2 == 0)
            {
                sex = "女";
            }
            else
            {
                sex = "男";
            }
            if (sex != addPeopleDto.Sex)
                throw ExceptionHelper.InvalidArgumentException($"请输入正确的性别");
        }

        /// <summary>
        /// 检查邮箱
        /// </summary>
        /// <param name="email"></param>
        internal void checkEmail(string email)
        {
            var addr = new MailAddress(email);
          
        }

        /// <summary>
        /// 检查用户名是否唯一
        /// </summary>
        /// <param name="addLoginDto"></param>
        internal void CheckaddLoginDto(AddLoginDto addLoginDto)
        {
            if (_boProvider._context.Peoples.Any(m => m.NetName == addLoginDto.NetName))
                throw ExceptionHelper.InvalidArgumentException("该用户名已存在");
        }

        /// <summary>
        /// 通过邮箱重置密码
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        internal bool ResetPasswordByEmail(string email)
        {
            var key = $"{nameof(ResetPasswordByEmail)}{email}";
            var cacheStr = _boProvider._cache.GetString(key);
            //if(cacheStr.IsNullOrEmpty())
            //{
            //    var data = JsonConvert.DeserializeObject<string>(cacheStr);
            //}
            //获取6为随机验证码
            Random random = new Random();
            var VerificationCode = random.Next(1000000).ToString().PadLeft(6, '0');
            //邮件来源
            var mailFron = new MailboxAddress("朝花夕拾", "1073308628@qq.com");
            //替换邮件模板
            var emailContent = _boProvider._context.MsgModels.FirstOrDefault(m => m.Status == (int)MsgModelEnum.重置密码邮件通知)?.Content;
            emailContent = emailContent.Replace("{VerificationCode}", VerificationCode);
            var mailTo = new MailboxAddress("亲爱的用户", email);

            SendEmail(mailFron, mailTo, "重置邮件通知", emailContent).Wait();
            _logger.Debug($"邮件已发送给：{email}用户");
            SetCache(key, VerificationCode, 23, 59, 59);
            return true;

        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="peopleLoginDto"></param>
        /// <returns></returns>
        internal object Login(PeopleLoginDto peopleLoginDto)
        {
            if (peopleLoginDto.NetName.IsNullOrEmpty() || peopleLoginDto.Password.IsNullOrEmpty())
            {
                return JsonConvert.SerializeObject(new ResultMsgDto() { Code = 400, Msg = $"用户名和密码不能为空" });
            }
            else
            {
                var result = _boProvider._peopleRepo.Login(peopleLoginDto);
                return result;
            }

        }

        /// <summary>
        /// 通过邮箱重置密码
        /// </summary>
        /// <param name="verificationCode"></param>
        /// <param name="password"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        internal bool ResetPassword(string idCard,string verificationCode, string password,string email)
        {
            var key = $"{nameof(ResetPasswordByEmail)}{email}";
            var cacheStr = _boProvider._cache.GetString(key);
            if(cacheStr.IsNullOrEmpty())
            {
                throw ExceptionHelper.DataNotFoundException("验证码已经过期，请重新获取");
            }
            else
            {
                var data = JsonConvert.DeserializeObject<string>(cacheStr);
                if(data==verificationCode)
                {
                    
                    var peopleBo = _boProvider.GetPeopleBoByIdCard(idCard);
                    peopleBo.People.Password = password;
                    var result = _boProvider._peopleRepo.ResetPassword(peopleBo);
                    return result;
                }
                else
                {
                    throw ExceptionHelper.InvalidArgumentException("请输入正确的验证码");
                }
            }
        }

        private void SetCache<T>(string key, T value, int hours = 1, int minutes = 0, int seconds = 0)
        {
          _boProvider._cache.SetString(key, JsonConvert.SerializeObject(value), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = new TimeSpan(hours, minutes, seconds) });
        }



        /// <summary>
        /// 找出各个城市的人
        /// </summary>
        internal void GetPeopleByCity()
        {
          var result= _boProvider._peopleRepo.GetPeopleByCity();         
            foreach (var item in result)
            {
                var data = item.Where(m => 1 == 1).ToList();
                if (data.Count == 1)
                {
                    continue;
                }
                else
                {
                    var arrylist = new ArrayList();
                    foreach (var release in data)
                    {
                        arrylist.Add(release);
                    }
                    for (var i = arrylist.Count - 1; i >= 0; i--)
                    {
                        //首先随机取出一个人
                        int random1 = new Random().Next(0, arrylist.Count - 1);
                        var people1 = (ReleaseInformation)arrylist[random1];
                        //移除已经取出的人
                        arrylist.RemoveAt(random1);
                        //再次随机取出第二人
                        int random2 = new Random().Next(0, arrylist.Count - 1);
                        var people2 = (ReleaseInformation)arrylist[random2];
                        //人员1邮件来源
                        var mailFron = new MailboxAddress("朝花夕拾", "1073308628@qq.com");
                        //替换邮件模板
                        var emailContent = _boProvider._context.MsgModels.FirstOrDefault(m => m.Status == (int)MsgModelEnum.配对成功邮件通知)?.Content;
                        emailContent = emailContent.Replace("{PeopleName}", people1.People.NetName)
                            .Replace("{emailAdress}", people2.Email);
                        var mailTo= new MailboxAddress(people1.People.Name, people1.Email);
                        SendEmail(mailFron, mailTo, "配对成功邮件通知", emailContent).Wait();

                        //人员2邮件来源
                        var mailFron2 = new MailboxAddress("朝花夕拾", "1073308628@qq.com");
                        //替换邮件模板
                        var emailContent2 = _boProvider._context.MsgModels.FirstOrDefault(m => m.Status == (int)MsgModelEnum.配对成功邮件通知)?.Content;
                        emailContent2 = emailContent2.Replace("{PeopleName}", people2.People.NetName)
                            .Replace("{emailAdress}", people1.Email);
                        var mailTo2 = new MailboxAddress(people2.People.Name, people2.Email);
                        SendEmail(mailFron2, mailTo2, "配对成功邮件通知", emailContent2).Wait();
                        //配对成功则软删除掉这两条记录
                        people1.IsDeleted = true;
                        people2.IsDeleted = true;
                        _boProvider._context.SaveChanges();
                        //移除第二个人
                        arrylist.RemoveAt(random2);
                        //当记录不足2条是跳出循环
                        if (arrylist.Count <= 1)
                            break;
                    }

                }
            }
        }
    }


}
