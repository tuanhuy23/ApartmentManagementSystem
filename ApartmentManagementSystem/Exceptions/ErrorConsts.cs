namespace ApartmentManagementSystem.Exceptions
{
    public static class ErrorMessageConsts
    {
        public static string UserNotFound = "Use is not found";
        public static string UserNameOrPasswordNotInCorrect = "UserName or Password is not incorrect";
        public static string RefreshTokenInvalid = "Refresh token is invalid";
        public static string ConfirmNewPasswordNotInCorrect = "Confirm new password not incorrect";
        public static string OldPasswordNotInCorrect = "Old password not incorrect";
        public static string ErrorWhenChangePassword = "Error when change password";
        public static string ErrorWhenCreateUser = "Error when create user";
        public static string ErrorWhenUpdateUser = "Error when update user";
        public static string ErrorWhenDeleteUser = "Error when delete user";
        public static string RoleNotFound = "Role not found";
        public static string RoleIdIsRequire = "Role Id is require";
        public static string NoPermissionAccessApartmentBuilding = "No permission access Apartment Building";
        public static string ApartmentNotFoud = "Apartment not found";
        public static string BillingCycleIsSet = "Billing cycle is set";
        public static string FeeTierIsRequired = "Fee tier is required";
        public static string FeeTypeNotFound = "Fee type not found";
        public static string FeeRateConfigNotfound = "Fee rate config not found";
        public static string CurentUtilityReadingDateNotEarlierPrevious = "The current utility value entry date cannot be earlier than the previous utility value entry date";
        public static string FeeTypeNotConfig = "Fee has not been set up yet";
        public static string BillingCycleSettingIsNotFound = "Billing cycle setting is not found";
        public static string BillingCycleInvalidFormat = "Billing cycle invalid format. Expected format is YYYY-MM";
        public static string FeeTypeIsRequired = "Fee type is required";
        public static string UtilityReadingDataIsRequired = "Utility reading data is required";
        public static string FeeNoticeHasBeenExisted = "The fee invoice for this billing period already exists";
        public static string FeeNoticeIsNotDue = "The fee invoice is not yet due.";
    }

    public static class ErrorCodeConsts
    {
        public static string UserNotFound = "USER_NOT_FOUND";
        public static string UserNameOrPasswordNotInCorrect = "USERNAME_OR_PASSWORD_NOT_IN_CORRECT";
        public static string RefreshTokenInvalid = "REFRESH_TOKEN_INVALID";
        public static string ConfirmNewPasswordNotInCorrect = "CONFIRM_NEW_PASSWORD_NOT_IN_CORRECT";
        public static string OldPasswordNotInCorrect = "OLD_PASSWORD_NOT_IN_CORRECT";
        public static string ErrorWhenChangePassword = "ERROR_WHEN_CHANGE_PASSWORD";
        public static string ErrorWhenCreateUser = "ERROR_WHEN_CREATE_USER";
        public static string ErrorWhenUpdateUser = "ERROR_WHEN_UPDATE_USER";
        public static string ErrorWhenDeleteUser = "ERROR_WHEN_DELETE_USER";
        public static string RoleNotFound = "ROLE_NOT_FOUND";
        public static string RoleIdIsRequire = "ROLE_ID_IS_REQUIRE";
        public static string NoPermissionAccessApartmentBuilding = "NO_PERMISSION_ACCESS_APARTMENT_BUILDING";
        public static string ApartmentNotFoud = "APARTMENT_NOT_FOUND";
        public static string BillingCycleIsSet = "BILLING_CYCLE_IS_SET";
        public static string BillingCycleInvalidFormat = "BILLING_CYCLE_INVALID_FORMAT";
        public static string BillingCycleSettingIsNotFound = "BILLING_CYCLE_IS_SET";
        public static string FeeTypeNotFound = "FEETYPE_NOT_FOUND";
        public static string FeeTypeNotConfig = "FEETYPE_NOT_CONFIG";
        public static string FeeTierIsRequired = "FEETIER_IS_REQUIRED";
        public static string FeeRateConfigNotfound = "FEERATE_CONFIG_NOTFOUND";
        public static string CurentUtilityReadingDateNotEarlierPrevious = "CURENT_UTILITY_READING_DATE_NOT_EARLIER_PREVIOUS";
        public static string UtilityReadingDataIsRequired = "UTILITYREADING_IS_REQUIRED";
        public static string FeeNoticeHasBeenExisted = "FEENOTICE_HAS_BEEN_EXISTED";
        public static string FeeNoticeIsNotDue = "FEENOTICE_IS_NOT_DUE";
    }
    
}
