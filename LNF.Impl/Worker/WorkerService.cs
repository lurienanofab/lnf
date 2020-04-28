using LNF.Worker;
using System;
using System.Messaging;

namespace LNF.Impl.Worker
{
    public class WorkerService : IWorkerService
    {
        private const string QUEUE_PATH = @".\private$\osw";

        public string Execute(WorkerRequest req)
        {
            if (string.IsNullOrEmpty(req.Command))
                throw new Exception($"The Command property cannot be empty.");

            var msgq = GetMessageQueue();
            var message = GetMessage(req);
            msgq.Send(message);

            return $"Enqueued command {req.Command} at {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        }

        private MessageQueue GetMessageQueue()
        {
            if (MessageQueue.Exists(QUEUE_PATH))
                return new MessageQueue(QUEUE_PATH);
            else
                throw new Exception($"Queue not found: {QUEUE_PATH}");
        }

        private Message GetMessage(WorkerRequest body)
        {
            var result = new Message(body)
            {
                Formatter = new XmlMessageFormatter(new[] { typeof(WorkerRequest) })
            };

            return result;
        }
    }
}
