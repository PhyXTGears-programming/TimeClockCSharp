using System;

public enum UserStatus {
    IN,
    DOUBLE_IN,
    OUT,
    DOUBLE_OUT,
    AUTO_OUT
}

// Create a static class to house behavior of UserStatus
public static class UserStatusExtensions {
    public static bool isClockedIn(this UserStatus status) {
        switch(status) {
            case UserStatus.IN:
                return true;
            case UserStatus.DOUBLE_IN:
                return true;
            case UserStatus.OUT:
                return false;
            case UserStatus.DOUBLE_OUT:
                return false;
            case UserStatus.AUTO_OUT:
                return false;
        }

        return false;
    }

    public static string ToStringFancy(this UserStatus status) {
        switch(status) {
            case UserStatus.IN:
                return "IN";
            case UserStatus.DOUBLE_IN:
                return "*IN";
            case UserStatus.OUT:
                return "OUT";
            case UserStatus.DOUBLE_OUT:
                return "*OUT";
            case UserStatus.AUTO_OUT:
                return "@OUT";
        }

        return "";
    }

    public static UserStatus FromStringFancy(string from) {
        switch(from) {
            case "IN":
                return UserStatus.IN;
            case "*IN":
                return UserStatus.DOUBLE_IN;
            case "OUT":
                return UserStatus.OUT ;
            case "*OUT":
                return UserStatus.DOUBLE_OUT;
            case "@OUT":
                return UserStatus.AUTO_OUT;
        }

        throw new FormatException($"Parse error: Unable to convert string {from} to a {nameof(UserStatus)} value");
    }
}