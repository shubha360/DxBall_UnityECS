using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Project.Scripts
{
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial class HandleMovementSystem : SystemBase
    {
        public float2 TouchDelta; 
        
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Handle>();
            state.RequireForUpdate<Config>();
        }

        protected override void OnUpdate()
        {
            var config = SystemAPI.GetSingleton<Config>();
            var handle = SystemAPI.GetSingletonEntity<Handle>();
            
            if (!(TouchDelta.x < float.Epsilon) && !(TouchDelta.x > float.Epsilon)) return;
            
            var handleMatrix = SystemAPI.GetComponentRO<PostTransformMatrix>(handle);
            var scaleX = handleMatrix.ValueRO.Value.Scale().x;

            var handleTransform = SystemAPI.GetComponentRW<LocalTransform>(handle);
            var newPosX = handleTransform.ValueRO.Position.x + (TouchDelta.x * config.HandleSpeed * SystemAPI.Time.DeltaTime);

            if (newPosX - (scaleX / 2.0f) < config.LeftBound)
            {
                newPosX = config.LeftBound + (scaleX / 2.0f);
            } 
            else if (newPosX + (scaleX / 2.0f) > config.RightBound)
            {
                newPosX = config.RightBound - (scaleX / 2.0f);
            }

            var newPos = new float3(newPosX, handleTransform.ValueRO.Position.y, handleTransform.ValueRO.Position.z);
            handleTransform.ValueRW.Position = newPos;
            
            TouchDelta = float2.zero;
        }
    }
}