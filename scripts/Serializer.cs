using System;
using System.IO;

public static class Serializer {

	// vars

	public static readonly string DATA_PATH = Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
	);
	public static readonly string USERS_FILE_PATH = Path.Combine(
		DATA_PATH, "frc-timeclock"
	);
	public static readonly string TIMES_FILE_PATH = Path.Combine(
		DATA_PATH, "frc-timeclock", "times"
	);

    public static readonly string NEW_LINE = Environment.NewLine;

	public const char DELIMITER = ',';

	// functions

	public static void initDataFiles() {
		Directory.CreateDirectory(USERS_FILE_PATH);

		if (!File.Exists(USERS_FILE_PATH)) {
			File.Create(USERS_FILE_PATH);
		}
	}

	public static void appendTime(
		string userName,
		UserStatus userStatus,
		DateTime time
	) {
		string path = Path.Combine(TIMES_FILE_PATH, userName);
		string log = time.ToLongDateString() + DELIMITER + userStatus.ToStringFancy();

		if (!File.Exists(path)) {
			File.Create(path);
		}

		File.AppendAllText(path, log);
	}
}
