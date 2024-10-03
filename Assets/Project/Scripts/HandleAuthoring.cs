using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Scripts
{
    public class HandleAuthoring : MonoBehaviour
    {
        class Baker : Baker<HandleAuthoring>
        {
            public override void Bake(HandleAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Handle());
            }
        }
    }

    public struct Handle : IComponentData
    {
    }
}