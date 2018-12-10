using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LNF.Service
{
    /// <summary>
    /// Limits execution of long running processes to one at a time.
    /// </summary>
    public class JobManager
    {
        private static JobManager _Current;

        private IList<string> _log;
        private BlockingCollection<ServiceJob> _queue;

        public event EventHandler<JobException> JobError;

        public JobManager()
        {
            _log = new List<string>
            {
                string.Format("[{0:yyyy-MM-dd HH:mm:ss}] Queue started.", DateTime.Now)
            };

            _queue = new BlockingCollection<ServiceJob>();

            Task.Run(async () =>
            {
                while (true)
                {
                    ServiceJob job = null;

                    try
                    {
                        job = _queue.Take();

                        if (job != null)
                        {
                            await Pop(job);
                        }
                    }
                    catch (Exception ex)
                    {
                        OnJobError(job, ex);
                    }
                }
            });
        }

        public static JobManager Current
        {
            get
            {
                if (_Current == null)
                {
                    _Current = new JobManager();
                }

                return _Current;
            }
        }

        public void Push(ServiceJob job)
        {
            _queue.Add(job);
        }

        public IEnumerable<string> GetLog()
        {
            return _log.AsEnumerable();
        }

        protected virtual void OnJobError(ServiceJob job, Exception ex)
        {
            JobError?.Invoke(this, new JobException(job, ex));
        }

        private async Task<bool> Pop(ServiceJob job)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();

            _log.Add($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Started job ID {job.ID} ({job.Name}).");

            if (job.Action != null)
            {
                bool result = await job.Action();
                sw.Stop();
                _log.Add($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Completed job ID {job.ID} ({job.Name}) in {sw.Elapsed.TotalSeconds:0.00} seconds. Result: {result}");
                return result;
            }
            else
            {
                _log.Add($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Unable to process job. {job.Name} does not have an action to execute.");
                throw new InvalidOperationException($"Unable to process job. {job.Name} does not have an action to execute.");
            }
        }
    }

    public class JobException : Exception
    {
        public ServiceJob Job { get; }
        public Exception Exception { get; }

        internal JobException(ServiceJob job, Exception ex)
        {
            Job = job;
            Exception = ex;
        }
    }
}
