// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.Infrastructure.Tasks;

internal sealed class LimitedConcurrencyLevelTaskScheduler : TaskScheduler
{
    private static readonly AsyncLocal<bool> currentThreadIsProcessingItems = new AsyncLocal<bool>();
    private readonly LinkedList<Task> tasks = new LinkedList<Task>();
    private readonly int maxDegreeOfParallelism;
    private readonly CancellationTokenSource shutdownCts = new CancellationTokenSource();
    private int delegatesQueuedOrRunning;

    public override int MaximumConcurrencyLevel => maxDegreeOfParallelism;

    public LimitedConcurrencyLevelTaskScheduler(int maxDegreeOfParallelism)
    {
        Guard.GreaterEquals(maxDegreeOfParallelism, 1);

        this.maxDegreeOfParallelism = maxDegreeOfParallelism;
    }

    protected override void QueueTask(Task task)
    {
        lock (tasks)
        {
            tasks.AddLast(task);

            if (delegatesQueuedOrRunning < maxDegreeOfParallelism)
            {
                ++delegatesQueuedOrRunning;

                NotifyThreadPoolOfPendingWork();
            }
        }
    }

    private void NotifyThreadPoolOfPendingWork()
    {
        ThreadPool.UnsafeQueueUserWorkItem(_ =>
        {
            currentThreadIsProcessingItems.Value = true;
            try
            {
                while (!shutdownCts.Token.IsCancellationRequested)
                {
                    Task task;

                    lock (tasks)
                    {
                        // When there are no more items to be processed,
                        // note that we're done processing, and get out.
                        if (tasks.Count == 0)
                        {
                            --delegatesQueuedOrRunning;
                            break;
                        }

                        // Cannot be null because of previous check.
                        task = tasks.First!.Value;
                        tasks.RemoveFirst();
                    }

                    TryExecuteTask(task);
                }
            }
            finally
            {
                currentThreadIsProcessingItems.Value = false;
            }
        }, null);
    }

    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
    {
        // If this thread isn't already processing a task, we don't support inlining
        if (!currentThreadIsProcessingItems.Value)
        {
            return false;
        }

        // If the task was previously queued, remove it from the queue
        if (taskWasPreviouslyQueued)
        {
            // Try to run the next task from the tasks.
            if (TryDequeue(task))
            {
                return TryExecuteTask(task);
            }

            return false;
        }

        return TryExecuteTask(task);
    }

    protected override bool TryDequeue(Task task)
    {
        lock (tasks)
        {
            return tasks.Remove(task);
        }
    }

    public void Shutdown()
    {
        shutdownCts.Cancel();
    }

    protected override IEnumerable<Task> GetScheduledTasks()
    {
        var lockTaken = false;
        try
        {
            Monitor.TryEnter(tasks, ref lockTaken);

            if (lockTaken)
            {
                return tasks;
            }

            throw new NotSupportedException();
        }
        finally
        {
            if (lockTaken)
            {
                Monitor.Exit(tasks);
            }
        }
    }
}
