using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace Project.Scripts
{
    public class BrickAuthoring : MonoBehaviour
    {
        class Baker : Baker<BrickAuthoring>
        {
            public override void Bake(BrickAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Brick());
                AddComponent(entity, new URPMaterialPropertyBaseColor());
            }
        }
    }

    public struct Brick : IComponentData
    {
    }
}