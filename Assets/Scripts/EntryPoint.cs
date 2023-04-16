using Stopwatch = System.Diagnostics.Stopwatch;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

public class EntryPoint : MonoBehaviour
{
    // 10000000 : 40ms
    // jobsystem : 33ms
    // burst : 10ms 早い！！！



    private Vector3[] array = new Vector3[10000000];

    private void Start()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        for (int i = 0; i < array.Length; i++)
        {
            array[i] = new(
                array[i].x + 3,
                array[i].y + 3,
                array[i].z + 4
            );
        }

        sw.Stop();
        Debug.Log(sw.ElapsedMilliseconds + " ms");

        MyJob myJob = new();
        NativeArray<Vector3> nArray = new(array, Allocator.TempJob);
        myJob.array = nArray;

        JobHandle handle = myJob.Schedule(array.Length, 2);

        sw.Restart();
        handle.Complete();
        sw.Stop();
        Debug.Log(sw.ElapsedMilliseconds + " ms");

        nArray.Dispose();

        MyJobBurst myJob2 = new();
        NativeArray<Vector3> nArray2 = new(array, Allocator.TempJob);
        myJob2.array = nArray2;

        JobHandle handle2 = myJob2.Schedule(array.Length, 2);

        sw.Restart();
        handle2.Complete();
        sw.Stop();
        Debug.Log(sw.ElapsedMilliseconds + " ms");

        nArray2.Dispose();
    }
}

public struct MyJob : IJobParallelFor
{
    public NativeArray<Vector3> array;

    public void Execute(int index)
    {
        array[index] = new(
            array[index].x + 3,
            array[index].y + 3,
            array[index].z + 4
        );
    }
}


[BurstCompile]
public struct MyJobBurst : IJobParallelFor
{
    public NativeArray<Vector3> array;

    public void Execute(int index)
    {
        array[index] = new(
            array[index].x + 3,
            array[index].y + 3,
            array[index].z + 4
        );
    }
}
