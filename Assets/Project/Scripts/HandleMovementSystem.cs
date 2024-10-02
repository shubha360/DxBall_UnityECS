using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Project.Scripts
{
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct HandleMovementSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Handle>();
            state.RequireForUpdate<Config>();
        }
    
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<Config>();
            var handle = SystemAPI.GetSingletonEntity<Handle>();
            
            var input = Input.GetAxis("Horizontal") * SystemAPI.Time.DeltaTime * config.HandleSpeed;
            if (!(input < float.Epsilon) && !(input > float.Epsilon)) return;
            
            var handleMatrix = state.EntityManager.GetComponentData<PostTransformMatrix>(handle);
            var scaleX = handleMatrix.Value.Scale().x;

            var handleTransform = state.EntityManager.GetComponentData<LocalTransform>(handle);
            var newPosX = handleTransform.Position.x + input;

            if (newPosX - (scaleX / 2.0f) < config.LeftBound)
            {
                newPosX = config.LeftBound + (scaleX / 2.0f);
            } 
            else if (newPosX + (scaleX / 2.0f) > config.RightBound)
            {
                newPosX = config.RightBound - (scaleX / 2.0f);
            }

            var newPos = new float3(newPosX, handleTransform.Position.y, handleTransform.Position.z);
            
            state.EntityManager.SetComponentData(handle, new LocalTransform
            {
                Position = newPos,
                Scale = handleTransform.Scale,
                Rotation = handleTransform.Rotation
            });
        }
    }
}