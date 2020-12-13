using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ARGBControl
{
	public interface IQueue<TQueueItem>
	{
		void Enqueue(TQueueItem item);
		Task<TQueueItem> DequeueAsync(CancellationToken cancellationToken = default);
	}
	
	public class InMemoryQueue<TQueueItem> : IQueue<TQueueItem>, IDisposable
	{
		private readonly ConcurrentQueue<TQueueItem> queueItems = new ConcurrentQueue<TQueueItem>();
		private readonly SemaphoreSlim dequeueBlock = new SemaphoreSlim(0);

		public void Enqueue(TQueueItem item)
		{
			this.queueItems.Enqueue(item);
			this.dequeueBlock.Release();
		}

		public async Task<TQueueItem> DequeueAsync(CancellationToken cancellationToken = default)
		{
			await this.dequeueBlock.WaitAsync(cancellationToken);
			this.queueItems.TryDequeue(out var queueItem);

			return queueItem;
		}

		public void Dispose() => this.dequeueBlock?.Dispose();
	}
}
