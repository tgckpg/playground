#define D2D_INPUT_COUNT 2
#define D2D_INPUT0_SIMPLE
#define D2D_INPUT1_SIMPLE

#include "d2d1effecthelpers.hlsli"


// Current position in the dissolve effect:
//  0 = no dissolve
//  1 = image fully disappeared
float dissolveAmount;


// Makes pixels gradually fade out as dissolveAmount approaches their mask threshold,
// rather than instantly disappearing.
float feather = 0.1;


D2D_PS_ENTRY( main )
{
	// Sample the two input textures.
	float4 color = D2DGetInput( 0 );
	float mask = D2DGetInput( 1 ).r;

	// Adjust the range of the mask threshold so that 0 and 1 map all
	// the way to fully on or off, even if the presence of feathering.
	float adjustedMask = mask * ( 1 - feather ) + feather / 2;

	// Compute alpha based on how close dissolveAmount is to the mask threshold for this pixel.
	float alpha = saturate( ( adjustedMask - dissolveAmount ) / feather + 0.5 );

	return color * alpha;
}
