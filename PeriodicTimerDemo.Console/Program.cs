using PeriodicTimerDemo.Console;

Console.WriteLine("Click any key to start repeating background task");
Console.ReadKey(true);

var bt = new BackgroundTask(TimeSpan.FromSeconds(1));
bt.Start();

Console.WriteLine("Click any key to stop repeating background task");
Console.ReadKey(true);
await bt.Stop();

