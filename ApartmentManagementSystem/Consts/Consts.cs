namespace ApartmentManagementSystem.Consts
{
    public static class Consts
    {
        public const string ReadPermissionName = "Read";
        public const string ReadWritePermissionName = "ReadWrite";
           public const string ReadWriteAllPermissionName = "ReadWriteAll";
        public const string PublishApprovePermissionName = "PublishApprove";
    }
    public static class RoleDefaulConsts
    {
        public const string SupperAdmin = "SupperAdmin";
        public const string Management = "Management";
        public const string Resident = "Resident";
    }
    public static class StatusConsts
    {
        public const string Active = "Active";
        public const string InActive = "InActive";       
        public const string UnPublish = "UNPUBLISH";
        public const string Publish = "PUBLISH";
        public const string New = "NEW";
        public const string InProgress = "PROCESSING";
        public const string Completed = "COMPLETED";
        public const string Canceled = "CANCELED";
    }
    public static class ActionType
    {
        public const string Create = "CREATE";
        public const string StatusChange = "STATUS_CHANGE";
        public const string Comment = "COMMENT";
        public const string Assign = "ASSIGN";
    }
    public static class FeeNoticeStatus
    {
        public const string Issued = "ISSUED";
        public const string Paid = "PAID";
        public const string Canceled = "CANCELED";
        public const string UnPaid = "UNPAID";
    }
    public static class FileType
    {
        public const string Doc = "Doc";
        public const string Media = "MEDIA";
    }
}
