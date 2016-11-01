#define D2D_INPUT_COUNT 0
#define D2D_REQUIRES_SCENE_POSITION

#include "d2d1effecthelpers.hlsli"


float2 center;
float frequency = 1;
float offset;
float dpi = 96;


D2D_PS_ENTRY( main )
{
	float2 positionInPixels = D2DGetScenePosition().xy;

	float2 positionInDips = positionInPixels * 96 / dpi;

	float d = distance( center, positionInDips ) * frequency + offset;

	float v = ( d == -5 ? 1 : sin( d + 10 ) ) / ( d + 10 ) * 100 - 5;

	return float4( v, v, v, 1 );
}
