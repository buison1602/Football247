namespace Football247.Authorization
{
    public static class Permissions
    {
        // Nhóm quyền cho Article 
        public static class Articles
        {
            public const string View = "Permissions.Articles.View";
            public const string Create = "Permissions.Articles.Create";
            public const string Edit = "Permissions.Articles.Edit";
            public const string Delete = "Permissions.Articles.Delete";
            // Quyền đặc biệt
            public const string DeleteAny = "Permissions.Articles.DeleteAny"; // Admin xóa bất kỳ bài nào
        }


        // Nhóm quyền cho User 
        public static class Users
        {
            public const string View = "Permissions.Users.View";
            public const string Create = "Permissions.Users.Create";
            public const string Edit = "Permissions.Users.Edit";
            public const string Delete = "Permissions.Users.Delete";
        }


        // Nhóm quyền cho Comment 
        public static class Comments
        {
            public const string View = "Permissions.Comments.View";
            public const string Create = "Permissions.Comments.Create";
            public const string Delete = "Permissions.Comments.Delete";
        }

        public static class Categories
        {
            public const string View = "Permissions.Categories.View";
            public const string Create = "Permissions.Categories.Create";
            public const string Edit = "Permissions.Categories.Edit";
            public const string Delete = "Permissions.Categories.Delete";
        }

        public static class Tags
        {
            public const string View = "Permissions.Tags.View";
            public const string Create = "Permissions.Tags.Create";
            public const string Edit = "Permissions.Tags.Edit";
            public const string Delete = "Permissions.Tags.Delete";
        }


        // dùng kỹ thuật Reflection lấy ra danh sách tất cả các quyền đã định nghĩa
        public static List<string> GetAllPermissions()
        {
            var allPermissions = new List<string>();
            var modules = typeof(Permissions).GetNestedTypes();

            foreach (var module in modules)
            {
                var fields = module.GetFields(System.Reflection.BindingFlags.Public |
                                              System.Reflection.BindingFlags.Static |
                                              System.Reflection.BindingFlags.FlattenHierarchy);

                foreach (var field in fields)
                {
                    var propValue = field.GetValue(null);
                    if (propValue is string permissionString)
                        allPermissions.Add(permissionString);
                }
            }
            return allPermissions;
        }
    }
}