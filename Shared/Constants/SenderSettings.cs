using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Constants
{
    public class SenderSettings
    {
        public const string HostName = "F247";
        public const string SendOtpSubject = "Đổi mật khẩu";
        public const string TemplateFileName = "Resources//{0}.html";
        public const string OtpValidMinute = "{0} phút";
        public const string OtpValidDay = "{0} ngày";
        public const string SendPassword = "Mật khẩu mới";
    }
}
