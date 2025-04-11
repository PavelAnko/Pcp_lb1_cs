using System;
using System.Collections.Generic;
using System.Threading;

namespace Company
{
    class ControllerThread
    {
        private readonly ManualResetEvent[] stopEvents;
        private readonly int[] stopTimes;

        public ControllerThread(ManualResetEvent[] stopEvents, int[] stopTimes)
        {
            this.stopEvents = stopEvents;
            this.stopTimes = stopTimes;
        }

        public void Start()
        {
            for (int i = 0; i < stopEvents.Length; i++)
            {
                int index = i;
                int delay = stopTimes[i];
                new Thread(() =>
                {
                    Thread.Sleep(delay);
                    stopEvents[index].Set();
                }).Start();
            }
        }
    }

    class Program
    {
        static int threadCount = 8;
        static double step = 0.5;
        static ManualResetEvent[] stopEvents = new ManualResetEvent[threadCount];
        static ManualResetEvent startSignal = new ManualResetEvent(false);
        static int[] workTimes = new int[threadCount];

        static void Main()
        {
            Thread[] threads = new Thread[threadCount];
            Random rand = new Random();

            for (int i = 0; i < threadCount; i++)
            {
                stopEvents[i] = new ManualResetEvent(false);
                workTimes[i] = rand.Next(3000, 10000);

                int index = i;
                ManualResetEvent stopEvent = stopEvents[i];

                threads[i] = new Thread(() => CalculateSequence(index, step, stopEvent));
                threads[i].Start();
            }

            startSignal.Set();

            ControllerThread controller = new ControllerThread(stopEvents, workTimes);
            controller.Start();
        }

        static void CalculateSequence(int id, double step, ManualResetEvent stopEvent)
        {
            startSignal.WaitOne();

            double sum = 0;
            int count = 0;
            double current = 0;

            while (!stopEvent.WaitOne(0))
            {
                sum += current;
                current += step;
                count++;
            }

            Console.WriteLine($"[Потік {id + 1}] Завершився. Сума: {sum}, Елементів: {count}");
        }
    }
}
