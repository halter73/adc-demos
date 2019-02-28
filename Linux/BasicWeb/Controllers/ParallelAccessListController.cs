using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Talk1Samples.Controllers
{
    public class ParallelAccessListController : Controller
    {
        [HttpGet("/parallel-list")]
        public async Task<IEnumerable<int>> Parallel()
        {
            var list = new List<int>();

            var tasks = new Task[10];

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = GetNumberAsync(list, i);
            }

            await Task.WhenAll(tasks);

            return list;
        }

        [HttpGet("/parallel-list-sync")]
        public async Task<IEnumerable<int>> ParallelSync()
        {
            var list = new List<int>();

            var tasks = new Task[10];

            var context = new OneAtAtTimeSyncContext();

            try
            {
                SynchronizationContext.SetSynchronizationContext(context);

                for (int i = 0; i < tasks.Length; i++)
                {
                    tasks[i] = GetNumberAsync(list, i);
                }

                GetNumberAsync(list, i).Wait();

                await Task.WhenAll(tasks);
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(null);
            }

            return list;
        }

        private async Task GetNumberAsync(List<int> results, int number)
        {
            await Task.Delay(300).ConfigureAwait(false);

            results.Add(number);

            foreach (var result in results)
            {
                Thread.Sleep(10);
            }
        }

        private class OneAtAtTimeSyncContext : SynchronizationContext
        {
            private Task _task = Task.CompletedTask;
            private object lockObj = new object();

            public override void Post(SendOrPostCallback d, object state)
            {
                lock (lockObj)
                {
                    _task = _task.ContinueWith(_ =>
                    {
                        d(state);
                    });
                }
            }
        }
    }
}