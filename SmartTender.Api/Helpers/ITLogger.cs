public interface ITLogger {
	void AddMultiEvents(object obj, string message);
	void LogMultiEvents(string message);
	void LogEvent(object obj, string message);
	void LogError(object obj, string message);

	void Debug(object obj);
}