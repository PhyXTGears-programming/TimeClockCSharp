using Godot;
using System;

public enum UserGroup {
    STUDENT,
    ADULT,
    MENTOR
}

// Create a static class to house behavior of UserGroup
public static class UserGroupExtensions {
    public static string ToStringFancy(this UserGroup status) {
        switch(status) {
            case UserGroup.STUDENT:
                return "student";
            case UserGroup.ADULT:
                return "adult";
            case UserGroup.MENTOR:
                return "mentor";
        }

        return "";
    }

    public static UserGroup FromStringFancy(string from) {
        switch(from) {
            case "student":
                return UserGroup.STUDENT;
            case "adult":
                return UserGroup.ADULT;
            case "mentor":
                return UserGroup.MENTOR;
        }

        throw new FormatException($"Parse error: Unable to convert string {from} to a {nameof(UserGroup)} value");
    }
}