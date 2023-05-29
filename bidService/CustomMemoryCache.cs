using System.Runtime.Caching;
using Microsoft.Extensions.Caching.Memory;

public class CustomMemoryCache : IMemoryCache
{
    public static CustomMemoryCache CustomMemoryCach {get; private set;} = new CustomMemoryCache();
    public  System.Runtime.Caching.MemoryCache MemoryCache {get; private set;}

    public CustomMemoryCache()
    {
        CustomMemoryCach = this;
        MemoryCache = new System.Runtime.Caching.MemoryCache("CustomMemoryCache");
    }

    public ICacheEntry CreateEntry(object key)
    {
        throw new NotImplementedException();
    }
    
    public void Dispose()
    {
        throw new NotImplementedException();
    }
    public void Fisk(){
        Console.WriteLine("Fiskehandler");
    }

    public void Remove(object key)
    {
        throw new NotImplementedException();
    }

    public bool TryGetValue(object key, out object? value)
    {
        throw new NotImplementedException();
    }

    // Implement the members of IMemoryCache using the underlying MemoryCache instance
    // ...



    // Implement the remaining members of IMemoryCache
    // ...
}
