using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Project.Scripts
{
    public class BallAuthoring : MonoBehaviour
    {
        public float Speed;
        public float2 Direction; 
        
        class Baker : Baker<BallAuthoring>
        {
            public override void Bake(BallAuthoring authoring)
            { 
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Ball());
                AddComponent(entity, new Velocity
                {
                    Speed = authoring.Speed,
                    Direction = authoring.Direction
                });
            }
        }
    }

    public struct Ball : IComponentData
    {
    }

    public struct Velocity : IComponentData
    {
        public float Speed;
        public float2 Direction;
    }
}