using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Project.Scripts
{
    public partial struct BrickSpawningSystem : ISystem
    {
        private Random _random;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Config>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;
            
            var config = SystemAPI.GetSingleton<Config>();
            _random = Random.CreateFromIndex((uint) UnityEngine.Random.Range(1, 1000));

            for (var row = 0; row < config.TotalRows; row++)
            {
                for (var col = 0; col < config.TotalColumns; col++)
                {
                    var flag = _random.NextBool();
                    if (flag) continue;

                    var brick = state.EntityManager.Instantiate(config.BrickPrefab);
                    state.EntityManager.SetComponentData(brick, new LocalTransform
                    {
                        Scale = 2,
                        Rotation = quaternion.identity,
                        Position = new float3(config.BricksTopLeftPos.x + col * config.BrickSize, 
                            config.BricksTopLeftPos.y - row * config.BrickSize, 0.0f)
                    });
                    
                    state.EntityManager.SetComponentData(brick, new URPMaterialPropertyBaseColor
                    {
                        Value = GetRandomColor(ref _random)
                    });
                }
            }
        }

        private static float4 GetRandomColor(ref Random random)
        {
            var hue = (random.NextFloat() + 0.618033988749895f) % 1.0f;
            return (Vector4) Color.HSVToRGB(hue, 1.0f, 1.0f);
        }
    }
}