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
        public static List<PermissionInfo> GetPermissionInfos()
        {
            var result = new List<PermissionInfo>()
            {
                GetPermissionInfo(UserPermissions.ReadWrite, "Permission", "User"),
                GetPermissionInfo(UserPermissions.Read, "Permission","User"),
                GetPermissionInfo(ApartmentPermissions.ReadWrite, "Permission","Apartment"),
                GetPermissionInfo(ApartmentPermissions.Read, "Permission","Apartment"),
                GetPermissionInfo(FeeConfigurationPermissions.ReadWrite, "Permission","Fee Configuration"),
                GetPermissionInfo(FeeConfigurationPermissions.Read, "Permission","Fee Configuration"),
                GetPermissionInfo(FeeNoticePermissions.ReadWrite, "Permission","Fee Notice"),
                GetPermissionInfo(FeeNoticePermissions.Read, "Permission","Fee Notice"),
                GetPermissionInfo(NotificationPermissions.ReadWrite, "Permission","Announcement"),
                GetPermissionInfo(NotificationPermissions.Read, "Permission","Announcement"),
                GetPermissionInfo(RolePermissions.ReadWrite, "Permission", "Role"),
                GetPermissionInfo(RolePermissions.Read, "Permission","Role")
            };
            return result;
        }
        public static PermissionInfo GetPermissionInfo(string permission, string type, string groupName = "")
        {
            var perrmissionInfo = new PermissionInfo();
            perrmissionInfo.DisplayName = MapPermissionDisplayName(permission);
            perrmissionInfo.Type = type;
            perrmissionInfo.Selected = false;
            perrmissionInfo.Name = permission;
            perrmissionInfo.GroupName = groupName;
            return perrmissionInfo;
        }

        private static string MapPermissionDisplayName(string permission)
        {
            if (permission.Equals(UserPermissions.ReadWrite))
            {
                return "Manage Users"; 
            }
            if (permission.Equals(UserPermissions.Read))
            {
                return "View Users";
            }

            if (permission.Equals(RolePermissions.ReadWrite))
            {
                return "Manage Roles";
            }
            if (permission.Equals(RolePermissions.Read))
            {
                return "View Roles";
            }

            if (permission.Equals(ApartmentPermissions.ReadWrite))
            {
                return "Manage Apartments";
            }
            if (permission.Equals(ApartmentPermissions.Read))
            {
                return "View Apartments";
            }

            if (permission.Equals(FeeConfigurationPermissions.ReadWrite))
            {
                return "Configure Fees"; 
            }
            if (permission.Equals(FeeConfigurationPermissions.Read))
            {
                return "View Fee Configuration";
            }

            if (permission.Equals(FeeNoticePermissions.ReadWrite))
            {
                return "Manage Fee Notices";
            }
            if (permission.Equals(FeeNoticePermissions.Read))
            {
                return "View Fee Notices";
            }

            if (permission.Equals(NotificationPermissions.ReadWrite))
            {
                return "Manage Notifications";
            }
            if (permission.Equals(NotificationPermissions.Read))
            {
                return "View Notifications";
            }

            if (permission.Equals(RequestPermissions.ReadWrite))
            {
                return "Manage Requests";
            }
            if (permission.Equals(RequestPermissions.Read))
            {
                return "View Requests";
            }

            return "Unknown Permission";
        }
    }
    public class PermissionInfo
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool Selected { get; set; }
        public string Type { get; set; }
        public string GroupName { get; set; }
    }
}
