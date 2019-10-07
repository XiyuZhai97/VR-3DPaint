using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;
using wvr.render.thread;
using wvr.render.utils;

public class RenderThreadSyncObjectTest : MonoBehaviour
{
    public class TestMessage : Message
    {
        public int textureId;
    }

    static CustomSampler sampler;
    void Start()
    {
        sampler = CustomSampler.Create("NTj");

        int C = randomList.Length;
        for (int i = 0; i < C; i++)
            randomList[i] = Random.Range(0, 20);

        setTexture = new RenderThreadSyncObject(Receive);

        senderThread = new Thread(SenderThread);
        senderThread.Start();
        receiverThread = new Thread(ReceiverThread);
        receiverThread.Start();
    }

    private void OnDisable()
    {
        run = false;
    }

    private static RenderThreadSyncObject setTexture;

    private Thread senderThread, receiverThread;
    private static Queue eventQueue = new Queue();
    private static bool run = true;
    private static int[] randomList = new int[2000];

    private static void SenderThread()
    {
        Debug.Log("Sender is running");
        Profiler.BeginThreadProfiling("SenderThread", "SenderThread");
        int i = 0;
        while (run)
        {
            RandomSleep();
            sampler.Begin();
            Enqueue(setTexture, i++);
            lock (eventQueue)
            {
                eventQueue.Enqueue(null);
            }
            sampler.End();
        }
        Profiler.EndThreadProfiling();
        Debug.Log("Sender is stopped");
    }

    private static void ReceiverThread()
    {
        Debug.Log("Receiver is running");
        Profiler.BeginThreadProfiling("ReceiverThread", "ReceiverThread");
        while (run)
        {
            RandomSleep();
            sampler.Begin();
            lock (eventQueue)
            {
                if (eventQueue.Count <= 0)
                    continue;
                eventQueue.Dequeue();
            }
            Receive(setTexture.Queue);
            sampler.End();
        }
        Profiler.EndThreadProfiling();
        Debug.Log("Receiver is stopped");
    }

    static int randomListIdx = -1;
    private static void RandomSleep()
    {
        randomListIdx = Mathf.Clamp(++randomListIdx, 0, randomList.Length);
        if (randomList[randomListIdx] <= 3)
            return;
        Thread.Sleep(randomList[randomListIdx]);
    }

    private static void Enqueue(RenderThreadSyncObject rtso, int textureId)
    {
        var queue = rtso.Queue;
        lock (queue)
        {
            var msg = queue.Obtain<TestMessage>();
            msg.textureId = textureId;
            queue.Enqueue(msg);
        }
    }

    private static void Receive(PreAllocatedQueue queue)
    {
        int textureId = 0;
        lock (queue)
        {
            // Run in RenderThread
            var msg = (TestMessage)queue.Dequeue();
            textureId = msg.textureId;
            queue.Release(msg);
        }

        Debug.Log("Receive id=" + textureId);
    }
}
