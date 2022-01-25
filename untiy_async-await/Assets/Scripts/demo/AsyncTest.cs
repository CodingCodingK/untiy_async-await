# define mode3
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

// Test Mode
// mode1 协程
// mode2 错误示例
// mode3 await
public class AsyncTest : MonoBehaviour
{
    public Text text1;

    // Start is called before the first frame update
    void Start()
    {

        
#if mode1
        StartCoroutine(DoSth());
#elif mode2
        DoSth_Await1();
#elif mode3
        DoSth_Await2();
#endif

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator DoSth_Coroutine()
    {
        text1.text = Thread.CurrentThread.ManagedThreadId + " thread: Start! " + DateTime.Now + "\n";
        Debug.Log(Thread.CurrentThread.ManagedThreadId + " thread: Waiting 2 second... " + DateTime.Now);
        yield return new WaitForSeconds(2.0f);
        text1.text += Thread.CurrentThread.ManagedThreadId + " thread: Done! " + DateTime.Now;
        Debug.Log(Thread.CurrentThread.ManagedThreadId + " thread: Done! "+ DateTime.Now);
    }

    async Task<int> DoSth_Await1()
    {
        // 线程内出错需要再次被调度才会抛出来。所以需要显式抛出。
        await Task.Run(
            () =>
            {
                try
                {
                    // 虽然不知道为什么用非gui程修改成功了这一次，但是后续gui线程不响应了。
                    text1.text = Thread.CurrentThread.ManagedThreadId + " thread: Start! " + DateTime.Now + "\n";
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    throw;
                }
            });
        
        Debug.Log(Thread.CurrentThread.ManagedThreadId + " thread: Waiting 2 second... " + DateTime.Now);
        text1.text += Thread.CurrentThread.ManagedThreadId + " thread: Done! " + DateTime.Now;
        Debug.Log(Thread.CurrentThread.ManagedThreadId + " thread: Done! " + DateTime.Now);
        
        return 1;
    }
    
    async Task<int> DoSth_Await2()
    {
        Debug.Log(Thread.CurrentThread.ManagedThreadId + " thread: Waiting 2 second... " + DateTime.Now);
        var assetBundle = await GetAssetBundle("www.my-server.com/myfile");
        text1.text += Thread.CurrentThread.ManagedThreadId + " thread: Done! " + DateTime.Now;
        Debug.Log(Thread.CurrentThread.ManagedThreadId + " thread: Done! " + DateTime.Now);
        
        return 1;
    }
    
    async Task<AssetBundle> GetAssetBundle(string url)
    {
        return (await new WWW(url)).assetBundle;
    }
    IEnumerator CustomCoroutineAsync()
    {
        //text1.text = Thread.CurrentThread.ManagedThreadId + " thread: Start! " + DateTime.Now;
        yield return new WaitForSeconds(2.0f);
    }
}

public static class AwaitExtensions
{
    public static TaskAwaiter GetAwaiter(this TimeSpan timeSpan)
    {
        Debug.Log(Thread.CurrentThread.ManagedThreadId + " thread: GetAwaiter! " + DateTime.Now);
        return Task.Delay(timeSpan).GetAwaiter();
    }
    
    // public static TaskAwaiter GetAwaiter(this IEnumerator et)
    // {
    //     Debug.Log(Thread.CurrentThread.ManagedThreadId + " thread: GetAwaiter! " + DateTime.Now);
    //     return et.GetAwaiter();
    // }
}
