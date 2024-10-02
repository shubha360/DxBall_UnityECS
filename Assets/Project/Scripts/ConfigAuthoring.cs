using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Project.Scripts
{
    public class ConfigAuthoring : MonoBehaviour
    {
        [Header("Boundaries")]
        public float TopBound;
        public float BottomBound;
        public float LeftBound;
        public float RightBound;

        [Header("Handle")] 
        public float HandleSpeed;
        public float HandleMaxSize;
        public float HandleMinSize;

        [Header("Bricks")]
        public GameObject BrickPrefab; 
        public float2 BricksTopLeftPos;
        public int TotalColumns;
        public int TotalRows;
        public float BrickSize;

        [Header("Powerups")] 
        public GameObject PowerupIncrease;
        public GameObject PowerupDecrease;
        public GameObject PowerupGoThrough;
        
        class Baker : Baker<ConfigAuthoring>
        {
            public override void Bake(ConfigAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new Config
                {
                    TopBound = authoring.TopBound,
                    BottomBound = authoring.BottomBound,
                    LeftBound = authoring.LeftBound,
                    RightBound = authoring.RightBound,
                    HandleSpeed = authoring.HandleSpeed,
                    BrickPrefab = GetEntity(authoring.BrickPrefab, TransformUsageFlags.Dynamic),
                    BricksTopLeftPos = authoring.BricksTopLeftPos,
                    TotalColumns = authoring.TotalColumns,
                    TotalRows = authoring.TotalRows,
                    BrickSize = authoring.BrickSize,
                    PowerupIncrease = GetEntity(authoring.PowerupIncrease, TransformUsageFlags.Dynamic),
                    PowerupDecrease = GetEntity(authoring.PowerupDecrease, TransformUsageFlags.Dynamic),
                    PowerupGoThrough = GetEntity(authoring.PowerupGoThrough, TransformUsageFlags.Dynamic)
                });
            }
        }
    }

    public struct Config : IComponentData
    {
        public float TopBound;
        public float BottomBound;
        public float LeftBound;
        public float RightBound;
        public float HandleSpeed;
        public float HandleMaxSize;
        public float HandleMinSize;
        public Entity BrickPrefab;
        public float2 BricksTopLeftPos;
        public int TotalColumns;
        public int TotalRows;
        public float BrickSize;
        public Entity PowerupIncrease;
        public Entity PowerupDecrease;
        public Entity PowerupGoThrough;
    }
}