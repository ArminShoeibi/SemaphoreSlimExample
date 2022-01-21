// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

List<Task> tasks = new();
for (int i = 0; i < 100; i++)
{
    Console.WriteLine(i);
    AccessTokenProvider accessTokenProvider = new();
    tasks.Add(accessTokenProvider.M(i));
}
await Task.WhenAll(tasks);

Console.ReadLine();


class AccessTokenProvider
{
    private readonly static SemaphoreSlim _semaphoreSlim = new(1, 1);
    private static string AccessToken = "";
    public async Task M(int index)
    {
        if (string.IsNullOrWhiteSpace(AccessToken) is false)
        {
            Console.WriteLine($"Access token already exists.{index}, BeforeWaitAsync: {true}");
        }
        else
        {
            try
            {
                await _semaphoreSlim.WaitAsync();

                // DOUBLE CHECK: Prevent setting acess token again because of multi threads.
                if (string.IsNullOrWhiteSpace(AccessToken) is false)
                {
                    Console.WriteLine($"Access token already exists. Index: {index}, BeforeWaitAsync: {false}");
                    return;
                }

                Console.WriteLine("Setting Access Token into Memory Cache. (Just one time every 45 minutes)");
                AccessToken = Guid.NewGuid().ToString("N");
                return;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }


    }
}