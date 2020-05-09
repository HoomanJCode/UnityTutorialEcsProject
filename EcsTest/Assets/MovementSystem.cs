using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public class MovementSystem : JobComponentSystem
{
    private EntityQuery cubeQuery;
    private MovementJob movementJob;
    private float startTime;
    protected override void OnCreate()
    {
        cubeQuery = GetEntityQuery(new EntityQueryDesc()
        {
            All = new []{
                ComponentType.ReadOnly<MovementData>(),
                ComponentType.ReadWrite<Translation>()
            }
        });
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        movementJob.TimeFromStart = (startTime+=Time.DeltaTime);
        movementJob.MovementArch = GetArchetypeChunkComponentType<MovementData>(true);
        movementJob.TranslationArch = GetArchetypeChunkComponentType<Translation>();

        return movementJob.Schedule(cubeQuery,inputDeps);
    }
    [BurstCompile]
    private struct MovementJob : IJobChunk
    {
        [ReadOnly] public float TimeFromStart;
        [ReadOnly] public ArchetypeChunkComponentType<MovementData> MovementArch;
        public ArchetypeChunkComponentType<Translation> TranslationArch;
        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            var movementDataArray = chunk.GetNativeArray(MovementArch);
            var translationsArray = chunk.GetNativeArray(TranslationArch);
            for (var i = 0; i < chunk.Count; i++)
            {
                var translations = translationsArray[i];
                translations.Value.z = Mathf.PerlinNoise(TimeFromStart, movementDataArray[i].Speed);
                translationsArray[i] = translations;
            }
        }
    }
}
