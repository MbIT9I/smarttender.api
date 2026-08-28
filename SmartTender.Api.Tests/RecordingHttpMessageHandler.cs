using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SmartTender.Api.Tests
{
	/// <summary>
	/// Test double for <see cref="HttpMessageHandler"/>. Returns a pre-programmed sequence of
	/// responses and captures a snapshot of every outgoing request (the real message object is
	/// disposed by <see cref="HttpClient"/> once it returns, so everything is read here).
	/// </summary>
	internal sealed class RecordingHttpMessageHandler : HttpMessageHandler
	{
		private readonly Queue<Func<CapturedRequest, HttpResponseMessage>> _responders;

		public RecordingHttpMessageHandler(params HttpStatusCode[] statuses)
		{
			_responders = new Queue<Func<CapturedRequest, HttpResponseMessage>>();
			foreach (var status in statuses)
			{
				var captured = status;
				_responders.Enqueue(_ => new HttpResponseMessage(captured)
				{
					Content = new StringContent("{}")
				});
			}
		}

		public RecordingHttpMessageHandler(params Func<CapturedRequest, HttpResponseMessage>[] responders)
		{
			_responders = new Queue<Func<CapturedRequest, HttpResponseMessage>>(responders);
		}

		public List<CapturedRequest> Requests { get; } = new List<CapturedRequest>();

		// Raw message references, kept for identity assertions (the objects are disposed by HttpClient).
		public List<HttpRequestMessage> RawRequests { get; } = new List<HttpRequestMessage>();

		public static Func<CapturedRequest, HttpResponseMessage> Respond(HttpStatusCode status, string json = "{}")
		{
			return _ => new HttpResponseMessage(status) { Content = new StringContent(json) };
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			string body = null;
			var isMultipart = false;
			if (request.Content != null)
			{
				isMultipart = request.Content is MultipartFormDataContent;
				var bytes = await request.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
				body = System.Text.Encoding.UTF8.GetString(bytes);
			}

			var snapshot = new CapturedRequest
			{
				Method = request.Method,
				RequestUri = request.RequestUri,
				Authorization = request.Headers.Authorization?.ToString(),
				Culture = SingleHeader(request, "culture"),
				OrganizationItId = SingleHeader(request, "OrganizationItId"),
				Body = body,
				IsMultipart = isMultipart,
				ContentLength = request.Content?.Headers?.ContentLength
			};
			Requests.Add(snapshot);
			RawRequests.Add(request);

			if (_responders.Count == 0)
				throw new InvalidOperationException("RecordingHttpMessageHandler received more requests than programmed responses.");

			return _responders.Dequeue()(snapshot);
		}

		private static string SingleHeader(HttpRequestMessage request, string name)
		{
			return request.Headers.TryGetValues(name, out var values)
				? string.Join(",", values)
				: null;
		}

		internal sealed class CapturedRequest
		{
			public HttpMethod Method { get; set; }
			public Uri RequestUri { get; set; }
			public string Authorization { get; set; }
			public string Culture { get; set; }
			public string OrganizationItId { get; set; }
			public string Body { get; set; }
			public bool IsMultipart { get; set; }
			public long? ContentLength { get; set; }
		}
	}
}
