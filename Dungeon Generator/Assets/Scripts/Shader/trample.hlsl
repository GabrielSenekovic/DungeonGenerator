#ifndef GRASS_TRAMPLED_INCLUDED
#define GRASS_TRAMPLED_INCLUDED

float _NumGrassTramplePositions;

float4 _GrassTramplePositions[8];

void CalculateTrample_float(float3 WorldPosition, float MaxDistance, float FallOff, float PushAwayStrength, float PushDownStrength, out float3 Offset, out float WindMultiplier)
{
    Offset = 0;
    WindMultiplier = 1;

    #ifndef SHADERGRAPH_PREVIEW
    for(int i = 0; i < _NumGrassTramplePositions; i++)
    {
        float3 objectPositionWS = _GrassTramplePositions[i].xyz;
        objectPositionWS.z = min(0, objectPositionWS.z -= 1);
        float3 distanceVector = WorldPosition -objectPositionWS;
        float distance = length(distanceVector);
        float strength = 1 - pow(saturate(distance / MaxDistance), FallOff);

        float3 xyDistance = distanceVector;
        xyDistance.z = 0; //Remember that z is up in DunGenes
        float3 PushAwayOffset = normalize(xyDistance) * PushAwayStrength * strength;

        float3 SquishOffset = float3(0,0,1) * PushDownStrength * strength;
        Offset += PushAwayOffset + SquishOffset;
        WindMultiplier = min(WindMultiplier, 1 - strength);
    }
    #endif
}

#endif