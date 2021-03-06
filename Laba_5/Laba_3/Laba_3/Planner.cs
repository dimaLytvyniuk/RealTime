﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace Laba_3
{
    public class Planner
    {
        static ConcurrentQueue<PlanObject> queue = new ConcurrentQueue<PlanObject>();
        static List<PlanObject> listCompletedTasks = new List<PlanObject>();
        static Dictionary<double, PlanObject> dictionaryCanceledTasks = new Dictionary<double, PlanObject>();
        static List<PlanObject> listWaitedTasks = new List<PlanObject>();
        static List<int> sizeOfQueue = new List<int>();

        static DateTime TimeStart;

        public static string MakePlan(Chart firstChart)
        {
            var N = 1000;
            var minimumInterval = 10;
            var maximumInterval = 20;
            var minimumDeadline = 20;
            var maximumDeadline = 30;

            TimeStart = DateTime.Now;

            var random = new Random();
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
            CancellationToken token = cancelTokenSource.Token;

            var taskWorker = Task.Run(() => RoundRobinWork(token));

            var taskMain = Task.Run( async () =>
            {
                for (var i = 0; i < N; i++)
                {
                    var newTask = new PlanObject
                    {
                        TimeIn = DateTime.Now,
                        Task = new Task(() => Functions.CreateCalculation(random.Next(600, 1000) * 2)),
                        Deadline = DateTime.Now + TimeSpan.FromMilliseconds(random.Next(minimumDeadline, maximumDeadline))
                    };
                    queue.Enqueue(newTask);
                    await Task.Delay(random.Next(minimumInterval, maximumInterval));
                }

                cancelTokenSource.CancelAfter(5000);
            });

            taskMain.Wait();
            taskWorker.Wait();

            var firstValues = listCompletedTasks
                .Select(x => ((int)((x.TimeOut - x.TimeIn).TotalMilliseconds)))
                .OrderBy(x => x)
                .GroupBy(x => x);

            var firstXAxi = firstValues.Select(x => x.Key.ToString()).ToArray();
            var firstYAxi = firstValues.Select(x => x.Count()).ToArray();

            //GraphHelper.PrintGraph(firstChart, firstXAxi, new[] { firstYAxi });
            PrintCanceledTasks(firstChart);

            var averageSizeOfQueue = sizeOfQueue.Average();
            var averageTimeFromInToOut = listCompletedTasks
                .Select(x => ((int)((x.TimeOut - x.TimeIn).TotalMilliseconds)))
                .Average();
            var countOfCanceled = dictionaryCanceledTasks.Count;
            var percent = countOfCanceled / N;

            return $"{averageSizeOfQueue} {averageTimeFromInToOut} {countOfCanceled} {percent}";
        }

        public static void RoundRobinWork(CancellationToken cancellationToken)
        {
            var tasks = new Task<PlanObject>[]
            {
                Task.Run<PlanObject>(async () => await WaitTask()),
                Task.Run<PlanObject>(async () => await WaitTask()),
                Task.Run<PlanObject>(async () => await WaitTask()),
                Task.Run<PlanObject>(async () => await WaitTask())
            };

            while(true)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var index = Task.WaitAny(tasks);
                var finishedObject = tasks[index].Result;
                finishedObject.TimeOut = DateTime.Now;
                tasks[index] = GetNextTask();

                if (finishedObject.IsWaitObject)
                    listWaitedTasks.Add(finishedObject);
                else if (finishedObject.IsCanceled)
                    dictionaryCanceledTasks.Add((finishedObject.TimeOut - TimeStart).TotalMilliseconds, finishedObject);
                else
                    listCompletedTasks.Add(finishedObject);
            }

            return;
        }

        public static PlanObject WorkWithTask(PlanObject planObject, DateTime timeIn)
        {
            planObject.Task.Start();
            planObject.IsCanceled = planObject.Task.Wait((int)(planObject.Deadline - timeIn).TotalMilliseconds);

            return planObject;
        }

        public static async Task<PlanObject> WaitTask()
        {
            await Task.Delay(20);
            return new PlanObject { IsWaitObject = true };
        }

        private static Task<PlanObject> GetNextTask()
        {
            sizeOfQueue.Add(queue.Count);
            if (queue.TryDequeue(out PlanObject planObject))
            {
                var timeIn = DateTime.Now;
                if (planObject.Deadline > timeIn)
                    return Task.Run(() => WorkWithTask(planObject, timeIn));
                else
                {
                    planObject.TimeOut = DateTime.Now;
                    dictionaryCanceledTasks.Add((planObject.TimeOut - TimeStart).TotalMilliseconds,planObject);
                    return GetNextTask();
                }
            }
            else
                return Task.Run(async () => await WaitTask());
        }
        
        private static void PrintCanceledTasks(Chart chart)
        {
            var values = dictionaryCanceledTasks.
                GroupBy(x => (int)(x.Key / 1000));

            var xAxi = values.Select(x => x.Key.ToString()).ToArray();

            var yAxi = values.Select(x => x.Count()).ToArray();

            GraphHelper.PrintGraph(chart, xAxi, new[] { yAxi });
        }
    }
}
