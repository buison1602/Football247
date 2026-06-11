using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Enum
{
    public enum EnumSenderTemplate
    {
        /// <summary>
        /// Dùng khi người dùng quên mật khẩu và cần gửi email để đặt lại mật khẩu mới.
        /// </summary>
        SendNewPassword,

        /// <summary>
        /// Dùng khi người dùng bật nhật thông báo qua email 
        /// </summary>
        SendNewArticleNotification,
    }
}
