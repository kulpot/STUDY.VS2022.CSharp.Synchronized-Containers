using System;

//ref link:https://www.youtube.com/watch?v=O-PyEy7m3oM&list=PLRwVmtr-pp06KcX24ycbC-KkmAISAFKV5&index=19
//


class MainClass
{
    //static Queue<int> numbers = new Queue<int>(); // Queue Structure -- requires knowledge in data structures
    static MySynchronizedQueue<int> numbers = new MySynchronizedQueue<int>();
    static Random rand = new Random(987); // output total 41
    const int NumThreads = 3;
    static int[] sums = new int[NumThreads]; // for total added array
    static void ProduceNumbers()    // Producing Method
    {
        for (int i = 0; i < 10; i++)
        {
            int numToEnqueue = rand.Next(10);
            //numbers.Enqueue(rand.Next(10));
            Console.WriteLine("Producing thread adding " + numToEnqueue + " to the queue.");
            //lock (numbers) // not needed cause of MySynchronizedQueue
            numbers.Enqueue(numToEnqueue);
            Thread.Sleep(rand.Next(1000));
        }
    }
    static void SumNumbers(object threadNumber)   // Consuming Method
    {   //---------poorman's method of synchronization technique---------- needs improvements
        DateTime startTime = DateTime.Now;
        int mySum = 0;
        while ((DateTime.Now - startTime).Seconds < 11)
        {
            int numToSum = -1;
            //lock (numbers) // lock statement - enable queue properly
            lock(numbers.SyncRoot)
            {
                if (numbers.Count != 0)
                {
                    numToSum = numbers.Dequeue();
                }
            }
            if (numToSum != -1)
            {
                mySum += numToSum;
                Console.WriteLine("Consuming thread #"
                    + threadNumber + " adding "
                    + numToSum + " to its total sum making "
                    + numToSum + " for the thread total.");
            }
        }
        sums[(int)threadNumber] = mySum;
    }
    static void Main()
    {
        var producingThread = new Thread(ProduceNumbers);
        producingThread.Start();
        Thread[] threads = new Thread[NumThreads];
        for (int i = 0; i < NumThreads; i++)
        {
            threads[i] = new Thread(SumNumbers);
            threads[i].Start(i);
        }
        for (int i = 0; i < NumThreads; i++)
            threads[i].Join();
        int totalSum = 0;
        for (int i = 0; i < NumThreads; i++)
            totalSum += sums[i];
        Console.WriteLine("Done adding. Total is " + totalSum);
    }
}

class MySynchronizedQueue<T>
//class MySynchronizedQueue<T> : Queue<T>   // override
{
    object baton = new object();    // baton is public
    Queue<T> theQ = new Queue<T>(); // theQ is private internal structure
    public void Enqueue(T item)
    {
        lock (baton)
            theQ.Enqueue(item);
    }
    public T Dequeue()
    {
        lock (baton)
            return theQ.Dequeue();
    }
    public int Count
    {
        get { lock (baton) return theQ.Count; }
    }
    public object SyncRoot  
    {
        get { return baton; }   // baton is public
    }
}