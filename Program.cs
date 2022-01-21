// See https://aka.ms/new-console-template for more information
using System.ComponentModel.DataAnnotations;

Console.WriteLine("Hello, World!");

List<AccessTokenProvider> accessTokenProviders = new();

for (int i = 0; i < 100; i++)
{
    Console.WriteLine(i);
    accessTokenProviders.Add(new());
}
await Task.Delay(TimeSpan.FromSeconds(5));

await Parallel.ForEachAsync(accessTokenProviders, async (accessTokenProvider, cancellationToken) => 
{
    await accessTokenProvider.M();
});

Console.ReadLine();


class AccessTokenProvider
{
    private readonly static SemaphoreSlim _semaphoreSlim = new(1, 1);
    private static string AccessToken = "";
    public async Task M()
    {
        if (string.IsNullOrWhiteSpace(AccessToken) is false)
        {
            Console.WriteLine($"Access token already exists, BeforeWaitAsync: {true}");
        }
        else
        {
            try
            {
                await _semaphoreSlim.WaitAsync();

                // DOUBLE CHECK: Prevent setting acess token again because of multi threads.
                if (string.IsNullOrWhiteSpace(AccessToken) is false)
                {
                    Console.WriteLine($"Access token already exists, BeforeWaitAsync: {false}");
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