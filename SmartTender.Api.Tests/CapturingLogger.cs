using System.Collections.Generic;

namespace SmartTender.Api.Tests
{
	/// <summary>Collects everything the wrapper logs so tests can assert on it.</summary>
	internal sealed class CapturingLogger : ITLogger
	{
		public List<string> Errors { get; } = new List<string>();
		public List<object> Debugs { get; } = new List<object>();

		public void AddMultiEvents(object obj, string message) { }
		public void LogMultiEvents(string message) { }
		public void LogEvent(object obj, string message) { }
		public void LogError(object obj, string message) => Errors.Add(message);
		public void Debug(object obj) => Debugs.Add(obj);
	}
}
