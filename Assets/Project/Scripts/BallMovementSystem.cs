using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Project.Scripts
{
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct BallMovementSystem : ISystem
    {
        private const float MinDistSq = 0.5f * 0.5f;
        private float2 _closestPoint;
        private Random _random;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Handle>();
            state.RequireForUpdate<Ball>();
            state.RequireForUpdate<Config>();
            
            _random = Random.CreateFromIndex((uint) (UnityEngine.Random.Range(1, 1000)));
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<Config>();
            var ball = SystemAPI.GetSingletonEntity<Ball>();

            var ballTransform = state.EntityManager.GetComponentData<LocalTransform>(ball);
            var velocity = state.EntityManager.GetComponentData<Velocity>(ball);

            var goThrough = SystemAPI.IsComponentEnabled<GoThrough>(ball);

            var newPos = ballTransform.Position.xy + (velocity.Direction * velocity.Speed * SystemAPI.Time.DeltaTime);

            if (newPos.y > config.TopBound)
            {
                newPos.y = config.TopBound - 0.5f;
                velocity.Direction.y = -velocity.Direction.y;
            }
            else if (newPos.x < config.LeftBound)
            {
                newPos.x = config.LeftBound + 0.5f;
                velocity.Direction.x = -velocity.Direction.x;
            }
            else if (newPos.x > config.RightBound)
            {
                newPos.x = config.RightBound - 0.5f;
                velocity.Direction.x = -velocity.Direction.x;
            }
            else // collision with handle
            {
                var handle = SystemAPI.GetSingletonEntity<Handle>();
                var handlePosition = state.EntityManager.GetComponentData<LocalTransform>(handle).Position;
                var handleScale = state.EntityManager.GetComponentData<PostTransformMatrix>(handle).Value.Scale();

                if (newPos.y < handlePosition.y + handleScale.y / 2.0f &&
                    newPos.y > handlePosition.y - handleScale.y / 2.0f &&
                    newPos.x < handlePosition.x + handleScale.x / 2.0f &&
                    newPos.x > handlePosition.x - handleScale.x / 2.0f)
                {
                    newPos.y = handlePosition.y + handleScale.y / 2.0f + 0.5f;
                    velocity.Direction.y = -velocity.Direction.y;
                }
            }

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (brickTransform, entity) in
                     SystemAPI.Query<RefRO<LocalTransform>>()
                         .WithAll<Brick>()
                         .WithEntityAccess())
            {
                if (newPos.x > brickTransform.ValueRO.Position.x + config.BrickSize / 2.0f)
                    _closestPoint.x = brickTransform.ValueRO.Position.x + config.BrickSize / 2.0f;
                else if (newPos.x < brickTransform.ValueRO.Position.x - config.BrickSize / 2.0f)
                    _closestPoint.x = brickTransform.ValueRO.Position.x - config.BrickSize / 2.0f;
                else
                    _closestPoint.x = newPos.x;

                if (newPos.y > brickTransform.ValueRO.Position.y + config.BrickSize / 2.0f)
                    _closestPoint.y = brickTransform.ValueRO.Position.y + config.BrickSize / 2.0f;
                else if (newPos.y < brickTransform.ValueRO.Position.y - config.BrickSize / 2.0f)
                    _closestPoint.y = brickTransform.ValueRO.Position.y - config.BrickSize / 2.0f;
                else
                    _closestPoint.y = newPos.y;

                if (math.distancesq(newPos, _closestPoint) <= MinDistSq)
                {
                    var powerUpFlag = _random.NextBool();
                    if (powerUpFlag)
                    {
                        var puType = _random.NextInt(0, 3);

                        var prefab = config.PowerupIncrease;

                        if (puType == 1) prefab = config.PowerupDecrease;
                        else if (puType == 2) prefab = config.PowerupGoThrough;
                        
                        var powerup = state.EntityManager.Instantiate(prefab);
                        state.EntityManager.SetComponentData(powerup, new LocalTransform
                        {
                            Scale = 1.0f,
                            Rotation = quaternion.identity,
                            Position = brickTransform.ValueRO.Position
                        });
                        state.EntityManager.SetComponentData(powerup, new Powerup
                        {
                            Type = puType
                        });
                    }

                    ecb.DestroyEntity(entity);

                    if (!goThrough)
                    {
                        // horizontal hit
                        if (Mathf.Approximately(_closestPoint.y, newPos.y))
                            velocity.Direction.x = -velocity.Direction.x;

                        // vertical hit
                        else if (Mathf.Approximately(_closestPoint.x, newPos.x))
                            velocity.Direction.y = -velocity.Direction.y;

                        // hit in the corner
                        else
                        {
                            velocity.Direction.x = -velocity.Direction.x;
                            velocity.Direction.y = -velocity.Direction.y;
                        }
                    }

                    break;
                }
            }
            ecb.Playback(state.EntityManager);
            
            state.EntityManager.SetComponentData(ball, new LocalTransform
            {
                Position = new float3(newPos.x, newPos.y, 0.0f),
                Scale = 1.0f,
                Rotation = quaternion.identity
            });

            state.EntityManager.SetComponentData(ball, new Velocity
            {
                Speed = velocity.Speed,
                Direction = velocity.Direction
            });
        }
    }
}