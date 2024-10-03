using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Project.Scripts
{
    public class PowerupAuthoring : MonoBehaviour
    {
        class Baker : Baker<PowerupAuthoring>
        {
            public override void Bake(PowerupAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Powerup());
                AddComponent(entity, new Velocity
                {
                    Speed = 10.0f,
                    Direction = new float2(0.0f, -1.0f)
                });
            }
        }
    }

    public struct Powerup : IComponentData
    {
        public int Type;
    }
}