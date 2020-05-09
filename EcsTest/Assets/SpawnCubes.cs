using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class SpawnCubes : MonoBehaviour
{
    [SerializeField] private int numberOfCubes = 100;
    [SerializeField] private bool spawnAtEcs = true;
    [SerializeField] private GameObject ecsPrefab;
    [SerializeField] private GameObject monoPrefab;
    private BeginSimulationEntityCommandBufferSystem commandBufferSystem;
    private void Start()
    {
        if (spawnAtEcs)
        {
            commandBufferSystem = World.DefaultGameObjectInjectionWorld
                .GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
            var entityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(ecsPrefab,
                new GameObjectConversionSettings(World.DefaultGameObjectInjectionWorld,
                    GameObjectConversionUtility.ConversionFlags.SceneViewLiveLink, new BlobAssetStore()));
            var jobHandle=new MyEcsSpawnJob()
            {
                CommandBuffer = commandBufferSystem.CreateCommandBuffer(),
                NumberOfCubes = numberOfCubes,
                EcsPrefab = entityPrefab,
                MyRandom = new Random((uint) UnityEngine.Random.Range(1,100000))
            }.Schedule();
            commandBufferSystem.AddJobHandleForProducer(jobHandle);
        }
        else
        {
            for (var i = 0; i < numberOfCubes; i++)
                Instantiate(monoPrefab).transform.position=new Vector3(UnityEngine.Random.Range(-100, 100f), 0, 0);
        }
    }
    private struct MyEcsSpawnJob: IJob
    {
        [ReadOnly] public int NumberOfCubes;
        [ReadOnly] public Entity EcsPrefab;
        public EntityCommandBuffer CommandBuffer;
        public Random MyRandom;
        
        public void Execute()
        {
            for (var i = 0; i < NumberOfCubes; i++)
            {
                var tempEntity = CommandBuffer.Instantiate(EcsPrefab);
                CommandBuffer.SetComponent(tempEntity, new MovementData()
                {
                    Speed = MyRandom.NextFloat(0,100)
                });
                CommandBuffer.SetComponent(tempEntity, new Translation()
                {
                    Value = new float3(MyRandom.NextFloat(-100,100), 0, 0)
                });
            }
        }
    }
}