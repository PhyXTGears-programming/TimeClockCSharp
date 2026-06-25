using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

/// <summary>
/// Users file format:
/// <code>
/// Some.Guy
/// SomeOther.Guy
/// King.Charles the 3rd
/// </code>
/// <para>One entry per line.</para>
/// <para>Values separated by a period.</para>
/// <para>First value is the first name and the second value is the last name which is separated by a period</para>
/// <para>The user file is used as a master lookup to all time files</para>
/// 
/// <code>
/// users.txt
/// times/
/// --Some.Guy.csv
/// --SomeOther.Guy.csv
/// --King.Charles the 3rd.csv
/// </code>
/// 
/// Time file format:
/// <code>
/// 2026-06-18 15:06,OUT
/// 2026-06-18 22:28,IN
/// 2026-06-18 22:30,*IN --> Double IN
/// 2026-06-18 22:40,OUT
/// 2026-06-18 22:44,*OUT -> Double OUT
/// 2026-06-18 19:12,IN
/// 2026-06-18 21:00,@OUT --> Automatically clocked OUT
/// </code>
/// <para>The time file is used as an append log.</para>
/// <para>One entry per line.</para>
/// <para>Values separated by comma.</para>
/// <para>First value is the date and time second value is the status which is separated by a comma</para>
/// 
/// <para>For more information about statuses, see: UserStatus.cs </para>
/// 
/// </summary>
public static class Serializer {

	// vars

	public static readonly string DATA_PATH = Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "frc-timeclock"
	);
	public static readonly string USERS_FILE_PATH = Path.Combine(
		DATA_PATH, "users.txt"
	);
	public static readonly string TIMES_FILE_DIR = Path.Combine(
		DATA_PATH, "times"
	);

    public static readonly string NEW_LINE = Environment.NewLine;

    // E.g 2026-06-18 15:26
    public const string DATE_TIME_FORMAT = "yyyy-MM-dd HH:mm";

	public const char DELIMITER = ',';

	// Functions

	public static void initDataFiles() {
		Directory.CreateDirectory(DATA_PATH);
        Directory.CreateDirectory(TIMES_FILE_DIR);

		if (!File.Exists(USERS_FILE_PATH)) {
			using FileStream fileStream = File.Create(USERS_FILE_PATH);
		}
	}

    public static UserStatus readStatus(string userName) {
        string path = getTimeFile(userName);

        string lastLine = readLastLine(path);

        getStatusEntry(lastLine, out UserStatus status);

        return status;
    }

    public static TimeSpan readTime(string userName) {
        string path = getTimeFile(userName);

        string[] lines = File.ReadAllLines(path);

        // Stores the accumulated time read from the append log
        TimeSpan accumulatedTime = TimeSpan.Zero;

        // Gather all data into a stack
        Stack<bool> clockInOutStack = new Stack<bool>();

        for (int i = 0; i < lines.Length; i++) {
            string line = lines[i];

            getStatusEntry(line, out UserStatus status);

            clockInOutStack.Push(status.isClockedIn());
        }

        // Rebuild the stack so Pop() returns entries in file order (top to bottom)
        clockInOutStack = new Stack<bool>(clockInOutStack);

        // Flags

        // insideClockInIndex == -1 --> latch is open
        // insideClockInIndex != -1 --> latch is closed
        int insideClockInIndex = -1;
        DateTime clockInStart = DateTime.Now;

        // Index pointer to line number
        int index = 0;

        // Algorithm:
        // Start from the top and work our way down to the bottom.
        // Descend the stack until we come across a IN status and then open a latch and set the time,
        // Make sure to NOT let other IN statuses overwrite these flags. This prevents an edge case like this:
        
        // 2026-06-18 15:06,OUT
        // 2026-06-18 22:28,IN
        // 2026-06-18 22:30,*IN <-- Without the check above we would overwrite the time causing a loss of time. (*IN is a double clock in) 
        // 2026-06-18 22:40,OUT

        // Once we hit an OUT status, we close the latch and figure out the time between the OUT status and IN status,
        // And add it to the accumulated time
        // At the end of the loop if the latch is still open we add the time between the IN status start and NOW

        // Repeat until the stack is empty
        while (clockInOutStack.Count > 0) {
            bool isClockedIn = clockInOutStack.Pop();

            // Dear Maintainer, DO NOT change this check to:
            // if (isClockedIn && insideClockInIndex == -1)
            // This as the sideaffect of making an IN status recorded while the latch is open to be interpreted as an OUT status
            // Due to the else if tacked on the end
            if (isClockedIn) {
                if (insideClockInIndex == -1) {
                    // Open the latch

                    insideClockInIndex = index;

                    string line = lines[index];

                    // Record time
                    getTimeStampEntry(line, out DateTime timeStamp);

                    clockInStart = timeStamp;
                }
            }
            else if (insideClockInIndex != -1) {
                // Close the latch

                string line = lines[index];

                getTimeStampEntry(line, out DateTime timeStamp);

                // Accumulate time
                accumulatedTime += timeStamp - clockInStart;

                // And... close the latch
                insideClockInIndex = -1;
            }

            index ++;
        }

        // If the latch is still open...
        if (insideClockInIndex != -1) {
            accumulatedTime += DateTime.Now - clockInStart;
        }

        return accumulatedTime;
    }

	public static void appendEntry(
		string userName,
		UserStatus userStatus,
		DateTime timeStamp
	) {
		string path = getTimeFile(userName);

        // E.g 2026-06-18 15:26
        string formattedTime = timeStamp.ToString(DATE_TIME_FORMAT);

		string log = 
            formattedTime + DELIMITER + userStatus.ToStringFancy() + NEW_LINE;

		File.AppendAllText(path, log);
	}

    public static string[] allUsers() {
        string usersText = File.ReadAllText(USERS_FILE_PATH);

        if (string.IsNullOrWhiteSpace(usersText)) {
            return Array.Empty<string>();
        }
        
        return usersText.Split(NEW_LINE);
    }

    private static string getTimeFile(string userName) {
        if (string.IsNullOrEmpty(userName)) {
            throw new FormatException($"Parse error: user name: '{userName}' is incorrect format");
        }
        
        return Path.Combine(TIMES_FILE_DIR, userName + ".csv");
    }

    private static void getEntries(string line, out DateTime timeStamp, out UserStatus userStatus) {
        getStatusEntry(line, out userStatus);
        getTimeStampEntry(line, out timeStamp);
    }

    private static void getStatusEntry(string line, out UserStatus userStatus) {
        string[] entry = line.Split(DELIMITER);

        if (entry.Length != 2) {
            throw new FormatException($"Parse error: too many or little values found when parsing status");
        }

        // Second entry is the status
        string statusString = entry[1];

        userStatus = UserStatusExtensions.FromStringFancy(statusString);
    }

    private static void getTimeStampEntry(string line, out DateTime timeStamp) {
        string[] entry = line.Split(DELIMITER);

        if (entry.Length != 2) {
            throw new FormatException($"Parse error: too many or little values found when parsing time stamp");
        }

        // First entry is the date time
        string dateString = entry[0];

        timeStamp = DateTime.Parse(dateString);
    }

    private static string readLastLine(string path) {
        using FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        
        long seeker = fs.Length - 1;

        // Skip any trailing newlines/carriage returns at the very end of the file
        while (seeker >= 0) {
            fs.Position = seeker;
            int byteRead = fs.ReadByte();

            if (byteRead != '\n' && byteRead != '\r') break;
            
            seeker--;
        }

        // If the file only contained newlines, return empty
        if (seeker < 0) return string.Empty;

        long endOfLastLine = seeker + 1;

        // Seek backward to find the start of the last line
        while (seeker >= 0) {
            fs.Position = seeker;
            int byteRead = fs.ReadByte();
            
            // break on LF (\n) or CR (\r) to catch all OS formats
            if (byteRead == '\n' || byteRead == '\r') break;
            
            seeker--;
        }

        // Extract and decode the bytes
        long startOfLastLine = seeker + 1;
        int lineLength = (int)(endOfLastLine - startOfLastLine);
        
        if (lineLength <= 0) return string.Empty;

        byte[] buffer = new byte[lineLength];
        fs.Position = startOfLastLine;
        fs.Read(buffer, 0, lineLength);
        
        return Encoding.UTF8.GetString(buffer);
    }

}
