using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Project.Scripts
{
	[UpdateBefore(typeof(TransformSystemGroup))]
	// [UpdateAfter(typeof(BallMovementSystem))]
    public partial struct PowerupMovementSystem : ISystem
    {
	    public void OnCreate(ref SystemState state)
	    {
		    state.RequireForUpdate<Handle>();
		    state.RequireForUpdate<Config>();
	    }

	    public void OnUpdate(ref SystemState state)
	    {
		    var config = SystemAPI.GetSingleton<Config>();
		    var handle = SystemAPI.GetSingletonEntity<Handle>();
		    var handlePosition = state.EntityManager.GetComponentData<LocalTransform>(handle).Position;
		    var handleMatrix = state.EntityManager.GetComponentData<PostTransformMatrix>(handle);
		    var handleScale = handleMatrix.Value.Scale();
		    
		    var ecb = new EntityCommandBuffer(Allocator.Temp);
		    
		    foreach (var (powerupTransform, power, velocity, entity) in
		             SystemAPI.Query<RefRW<LocalTransform>, RefRO<Powerup>, RefRO<Velocity>>()
			             .WithAll<Powerup>()
			             .WithEntityAccess())
		    {
			    var newPosition = powerupTransform.ValueRO.Position.xy + (velocity.ValueRO.Direction * velocity.ValueRO.Speed * SystemAPI.Time.DeltaTime);

			    if (newPosition.y <= -25.0f)
				    ecb.DestroyEntity(entity);
			    
			    powerupTransform.ValueRW.Position.xy = newPosition;
			    
			    if (newPosition.y - 0.5f <= handlePosition.y + handleScale.y / 2.0f &&
			        newPosition.y + 0.5f >= handlePosition.y - handleScale.y / 2.0f &&
			        newPosition.x + 0.5f >= handlePosition.x - handleScale.x / 2.0f && 
			        newPosition.x - 0.5f <= handlePosition.x + handleScale.x / 2.0f)
			    {
				    // handle increase
				    if (power.ValueRO.Type == 0)
				    {
					    handleScale.x = handleScale.x + 1.0f;
					    handleMatrix.Value.c0.x = handleScale.x;
				    }
				    // handle decrease
				    else if (power.ValueRO.Type == 1)
				    {
					    handleScale.x = handleScale.x - 1.0f;
					    handleMatrix.Value.c0.x = handleScale.x;
				    }
				    
				    state.EntityManager.SetComponentData(handle, new PostTransformMatrix
				    {
					    Value = handleMatrix.Value
				    });
				    
				    ecb.DestroyEntity(entity);
			    }
		    }
		    ecb.Playback(state.EntityManager);
	    }
    }
}