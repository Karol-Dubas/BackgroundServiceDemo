Console.WriteLine("Click any key to start a background task");
Console.ReadKey(true);

var bt = new BackgroundTask(
    () => Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff")), 
    TimeSpan.FromSeconds(1));

bt.Start();

Console.WriteLine("Click any key to stop the background task");
Console.ReadKey(true);
await bt.Stop();
