using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PointsMall.Common
{
    public static class ExcelHelper
    {

        /// <summary>
        /// 下载excel模板
        /// </summary>
        /// <returns></returns>
        public static FileStream DownloadTemplate(IWebHostEnvironment _environment)
        {
            var tempPath = _environment.WebRootPath + string.Format("/upload/{0}/{1}", "template", "ImportPeoplesInfoTemplate.xlsx");
            //读取excel模板文件
            FileInfo file = new FileInfo(tempPath);
            return file.OpenRead();
        }

  

     


    }
}
