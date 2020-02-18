using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CancellationTokenForParallel_Invoke
{
    class Program
    {
        static void DoActionNTimes(Action action, CancellationToken token)
        {
            Action[] actions = Enumerable.Repeat(action, 2).ToArray();
            try
            {
                Parallel.Invoke(new ParallelOptions { CancellationToken = token }, actions);
            }
            catch(OperationCanceledException)
            {
                Console.WriteLine("Operation was cancelled.");
            }
        }

        static void DoActionNTimes(CancellationToken token)
        {
            Action action = new Action(() =>
            {
                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine($"Action state: {i}...");
                }
            });
            Action[] actions = Enumerable.Repeat(action, 3).ToArray();
            try
            {
                Parallel.Invoke(new ParallelOptions { CancellationToken = token }, actions);
            }
            catch(OperationCanceledException)
            {
                Console.WriteLine("Operation was cancelled");
            }
        }

        static void Main(string[] args)
        {
            Action ac = new Action(() => Console.WriteLine("action..."));
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;
            for (int i = 0; i < 4; i++)
            {
                DoActionNTimes(ac, token);
                if (i == 2)
                {
                    cts.Cancel();
                }
            }

            Console.WriteLine("\n");

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            for (int i = 0; i < 4; i++)
            {
                DoActionNTimes(cancellationToken);
                if(i == 2)
                {
                    cancellationTokenSource.Cancel();
                }
            }

            Console.ReadKey();
        }
    }
}
