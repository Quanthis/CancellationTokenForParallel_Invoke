using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

namespace CancellationTokenForParallel_Invoke
{
    class Program
    {
        static async Task<int> DoSomethingButTwice(CancellationToken cancellation)
        {
            return await Task.Run(() =>
             {
                 var po = new ParallelOptions { CancellationToken = cancellation };

                 Parallel.Invoke(po, () => Test1(cancellation), () => Test1(cancellation));
                 return 0;
             });
        }

        static void Test1(CancellationToken token)
        {
            for(int i = 0; i <= 10; i++)
            {
                WriteLine($"Was cancellation requested: {token.IsCancellationRequested}");
                Thread.Sleep(1000);
            }
        }

        static async Task Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            var ct = cts.Token;

            DoSomethingButTwice(ct);

            for(int i = 0; i < 10; i++)
            {
                WriteLine($"i == {i}");
                Thread.Sleep(1000);

                if(i == 5)
                {
                    WriteLine("Cancellation request sent. ");
                    cts.Cancel();
                    break;
                }
            }

            ReadKey();
        }
    }
}
