using ApartmentManagementSystem.Consts.Permissions;
using System.Reflection;

namespace ApartmentManagementSystem.Identity
{
    public static class PermissionsHelper
    {
        public static void GetPermissions(this Dictionary<string, string> allPermissions, Type policy)
        {
            FieldInfo[] fields = policy.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (FieldInfo fi in fields)
            {
                allPermissions.Add(fi.GetValue(null).ToString(), policy.Name);
            }
        }

        public static void GetPermissionsVN(this List<PermissionInfo> allPermissions, Type policy)
        {
            FieldInfo[] fields = policy.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (FieldInfo fi in fields)
            {
                allPermissions.Add(GetPermissionInfo(fi.GetValue(null).ToString(), policy.Name));
            }
        }
        private static PermissionInfo GetPermissionInfo(string permission, string type)
        {
            var perrmissionInfo = new PermissionInfo();
            perrmissionInfo.DisplayName = MapPermissionDisplayName(permission);
            perrmissionInfo.Type = type;
            perrmissionInfo.Selected = false;
            perrmissionInfo.Name = permission;
            return perrmissionInfo;
        }

        public static string MapPermissionDisplayName(string permission)
        {
            if (permission.Equals(UserPermissions.ReadWrite))
            {
                return "Xem và chỉnh sửa tài khoản";
            }
            if (permission.Equals(UserPermissions.ReadWriteAll))
            {
                return "Xem và chỉnh sửa toàn bộ  tài khoản";
            }
            if (permission.Equals(UserPermissions.Read))
            {
                return "Xem tài khoản";
            }

            if (permission.Equals(RolePermissions.ReadWrite))
            {
                return "Xem và chỉnh sửa chức vụ";
            }
            if (permission.Equals(RolePermissions.ReadWriteAll))
            {
                return "Xem và chỉnh sửa toàn bộ chức vụ";
            }
            if (permission.Equals(RolePermissions.Read))
            {
                return "Xem  chức vụ";
            }
            return "";
        }
    }
    public class PermissionInfo
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool Selected { get; set; }
        public string Type { get; set; }
    }
}
