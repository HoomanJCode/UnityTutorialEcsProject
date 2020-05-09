using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class MyIJobParallelForExample : MonoBehaviour
{
        private NativeArray<byte> myImageData;
        [SerializeField] private int arrayLen = 1000;
        [SerializeField] private byte amount = 10;

        private void Start()
        {
                myImageData = new NativeArray<byte>(arrayLen, Allocator.TempJob);
                for (var i = 0; i < arrayLen; i++)
                        myImageData[i] = (byte) Random.Range(0, 255);
                var myWatch = Stopwatch.StartNew();
                var jobHandle=new AddToImageJob()
                {
                        MyImageData = myImageData,
                        Amount=amount
                }.Schedule(arrayLen,100);
                jobHandle.Complete();
                myWatch.Stop();
                Debug.Log(myWatch.ElapsedMilliseconds);

                var myImageDataArray = myImageData.ToArray();
                myWatch = Stopwatch.StartNew();
                Parallel.For(0,arrayLen, (int index) =>
                {
                        for (var i = 0; i < 100; i++)
                                myImageDataArray[index] = (byte) (myImageDataArray[index] + amount);
                });
                myWatch.Stop();
                Debug.Log(myWatch.ElapsedMilliseconds);
                myImageData.Dispose();
        }

        [BurstCompile]
        private struct AddToImageJob : IJobParallelFor
        {
                [ReadOnly] public byte Amount;
                public NativeArray<byte> MyImageData;

                public void Execute(int index)
                {
                        for (var i = 0; i < 100; i++)
                                MyImageData[index] = (byte) (MyImageData[index] + Amount);    
                }
        }
}