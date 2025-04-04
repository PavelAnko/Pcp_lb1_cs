using System;
using System.Threading;

class Program
{
    static int threadCount = 20;
    static double step = 0.5;
    static ManualResetEvent[] stopEvents = Array.Empty<ManualResetEvent>();
    static ManualResetEvent startSignal = new ManualResetEvent(false); // для одночасного (синхроного) запуску 

    static void Main()
    {
        stopEvents = new ManualResetEvent[threadCount];
        Thread[] threads = new Thread[threadCount];
        Random rand = new Random();

        for (int i = 0; i < threadCount; i++)
        {
            stopEvents[i] = new ManualResetEvent(false);  // ініціалізація кожного потоку 
            int index = i;
            int delay = rand.Next(3000, 10000); 

            threads[i] = new Thread(() => CalculateSequence(index, step, stopEvents[index], delay));
            threads[i].Start();
        }
        startSignal.Set(); 
    }

    static void CalculateSequence(int id, double step, ManualResetEvent stopEvent, int workTime)
    {
        startSignal.WaitOne(); // чекає поки не буде дозволено старт

        double sum = 0;
        int count = 0;
        double current = 0;

        int elapsed = 0;
        int interval = 10;  // імітація навантаження або ж затримки

        while (elapsed < workTime)
        {
            sum += current;
            current += step;
            count++;
            Thread.Sleep(interval);
            elapsed += interval;
        }

        Console.WriteLine($"[Потік {id + 1}] Завершився після {workTime} мс. Сума: {sum}, Елементів: {count}");
        // stopEvent.Set();
    }
}
