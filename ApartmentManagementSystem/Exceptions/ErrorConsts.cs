namespace ApartmentManagementSystem.Exceptions
{
    public static class ErrorMessageConsts
    {
        public static string UserNotFound = "User not found";
        public static string UserNameOrPasswordIncorrect = "Username or password is incorrect";
        public static string RefreshTokenInvalid = "Refresh token is invalid";
        public static string ConfirmNewPasswordIncorrect = "Confirmation password is incorrect";
        public static string OldPasswordIncorrect = "Old password is incorrect";
        public static string ErrorChangingPassword = "An error occurred while changing the password";
        public static string ErrorCreatingUser = "An error occurred while creating the user";
        public static string ErrorUpdatingUser = "An error occurred while updating the user";
        public static string ErrorDeletingUser = "An error occurred while deleting the user";
        public static string RoleNotFound = "Role not found";
        public static string RoleIdIsRequired = "Role ID is required";
        public static string NoPermissionAccessApartmentBuilding = "You do not have permission to access this apartment building";
        public static string ApartmentNotFound = "Apartment not found";
        public static string BillingCycleAlreadySet = "Billing cycle has already been set";
        public static string FeeTierIsRequired = "Fee tier is required";
        public static string FeeTypeNotFound = "Fee type not found";
        public static string FeeRateConfigNotFound = "Fee rate configuration not found";
        public static string CurrentUtilityReadingDateCannotBeEarlier = "The current utility reading date cannot be earlier than the previous reading date";
        public static string FeeTypeNotConfigured = "Fee type has not been configured";
        public static string BillingCycleSettingIsNotFound = "Billing cycle setting not found";
        public static string BillingCycleInvalidFormat = "Billing cycle invalid format. Expected format is YYYY-MM";
        public static string FeeTypeIsRequired = "Fee type is required";
        public static string UtilityReadingDataIsRequired = "Utility reading data is required";
        public static string FeeDetailIsRequired = "Fee detail data is required";
        public static string FeeNoticeAlreadyExists = "A fee notice for this billing cycle already exists";
        public static string FeeNoticeNotDue = "The fee notice is not yet due";
        public static string FeeNoticeNotFound = "Fee notice not found";
        public static string FeeNoticeIdIsRequired = "Fee notice ID is required";
        public static string FeeNoticeCannotBeModified = "The fee notice cannot be modified based on its current status";
    }

    public static class ErrorCodeConsts
    {
        public static string UserNotFound = "USER_NOT_FOUND";
        public static string UserNameOrPasswordIncorrect = "USERNAME_OR_PASSWORD_INCORRECT";
        public static string RefreshTokenInvalid = "REFRESH_TOKEN_INVALID";
        public static string ConfirmNewPasswordIncorrect = "CONFIRM_NEW_PASSWORD_INCORRECT";
        public static string OldPasswordIncorrect = "OLD_PASSWORD_INCORRECT";
        public static string ErrorChangingPassword = "ERROR_CHANGING_PASSWORD";
        public static string ErrorCreatingUser = "ERROR_CREATING_USER";
        public static string ErrorUpdatingUser = "ERROR_UPDATING_USER";
        public static string ErrorDeletingUser = "ERROR_DELETING_USER";
        public static string RoleNotFound = "ROLE_NOT_FOUND";
        public static string RoleIdIsRequired = "ROLE_ID_IS_REQUIRED";
        public static string NoPermissionAccessApartmentBuilding = "NO_PERMISSION_ACCESS_APARTMENT_BUILDING";
        public static string ApartmentNotFound = "APARTMENT_NOT_FOUND";
        public static string BillingCycleAlreadySet = "BILLING_CYCLE_ALREADY_SET";
        public static string BillingCycleInvalidFormat = "BILLING_CYCLE_INVALID_FORMAT";
        public static string BillingCycleSettingIsNotFound = "BILLING_CYCLE_SETTING_NOT_FOUND"; 
        public static string FeeTypeNotFound = "FEE_TYPE_NOT_FOUND";
        public static string FeeTypeNotConfigured = "FEE_TYPE_NOT_CONFIGURED";
        public static string FeeTierIsRequired = "FEE_TIER_IS_REQUIRED";
        public static string FeeRateConfigNotFound = "FEE_RATE_CONFIG_NOT_FOUND";
        public static string CurrentUtilityReadingDateCannotBeEarlier = "CURRENT_UTILITY_READING_DATE_CANNOT_BE_EARLIER";
        public static string UtilityReadingDataIsRequired = "UTILITY_READING_DATA_IS_REQUIRED";
        public static string FeeDetailIsRequired = "FEE_DETAIL_IS_REQUIRED";
        public static string FeeNoticeAlreadyExists = "FEE_NOTICE_ALREADY_EXISTS";
        public static string FeeNoticeNotDue = "FEE_NOTICE_NOT_DUE";
        public static string FeeNoticeNotFound = "FEE_NOTICE_NOT_FOUND";
        public static string FeeNoticeIdIsRequired = "FEE_NOTICE_ID_IS_REQUIRED";
        public static string FeeNoticeCannotBeModified = "FEE_NOTICE_CANNOT_BE_MODIFIED";
    }
}