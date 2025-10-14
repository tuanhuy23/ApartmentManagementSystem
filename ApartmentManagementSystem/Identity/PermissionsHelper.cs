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
                return "Read and write user";
            }
            if (permission.Equals(UserPermissions.Read))
            {
                return "Read user";
            }

            if (permission.Equals(RolePermissions.ReadWrite))
            {
                return "Read and write role";
            }
            if (permission.Equals(RolePermissions.Read))
            {
                return "Read role";
            }

            if (permission.Equals(ApartmentBuildingPermissions.ReadWrite))
            {
                return "Read and write apartment building";
            }
            if (permission.Equals(ApartmentBuildingPermissions.Read))
            {
                return "Read apartment building";
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
