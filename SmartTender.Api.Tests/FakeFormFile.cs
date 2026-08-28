using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SmartTender.Api.Tests
{
	/// <summary>Minimal <see cref="IFormFile"/> backed by an in-memory byte array.</summary>
	internal sealed class FakeFormFile : IFormFile
	{
		private readonly byte[] _content;

		public FakeFormFile(byte[] content, string fileName)
		{
			_content = content;
			FileName = fileName;
		}

		public string ContentType { get; set; } = "application/octet-stream";
		public string ContentDisposition { get; set; }
		public IHeaderDictionary Headers { get; set; } = new HeaderDictionary();
		public long Length => _content.Length;
		public string Name => "file";
		public string FileName { get; }

		public Stream OpenReadStream() => new MemoryStream(_content, writable: false);
		public void CopyTo(Stream target) => target.Write(_content, 0, _content.Length);
		public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
			=> target.WriteAsync(_content, 0, _content.Length, cancellationToken);
	}
}
