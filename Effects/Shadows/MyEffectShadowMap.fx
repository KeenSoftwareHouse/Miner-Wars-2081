#include "../MyEffectShadowBase.fxh"

float4x4	WorldMatrix;
float4x4	ViewProjMatrix;
float2		ShadowTermHalfPixel;
float3		FrustumCornersVS[4];
float2		HalfPixel;

texture DepthTexture;
sampler2D DepthTextureSampler = sampler_state
{
    Texture = <DepthTexture>;
    MinFilter = POINT;
    MagFilter = POINT;
    MipFilter = none;
};

// Instance data
struct VertexShaderInput_Instance
{
	float4 worldMatrixRow0 : BLENDWEIGHT0;
 	float4 worldMatrixRow1 : BLENDWEIGHT1;
 	float4 worldMatrixRow2 : BLENDWEIGHT2;
 	float4 worldMatrixRow3 : BLENDWEIGHT3;
};

void GenerateShadowMapVS_Base(in float4 in_vPositionOS, in float4x4 worldMatrix, out float4 out_vPositionCS, out float2 out_vDepthCS)
{
	// Unpack position
	in_vPositionOS = UnpackPositionAndScale(in_vPositionOS);

	// Figure out the position of the vertex in view space and clip space
	out_vPositionCS = mul(in_vPositionOS, worldMatrix);
    out_vPositionCS = mul(out_vPositionCS, ViewProjMatrix);
	out_vDepthCS = out_vPositionCS.zw;
}

// Vertex shader for outputting light-space depth to the shadow map
void GenerateShadowMapVS(	in float4 in_vPositionOS	: POSITION,
							out float4 out_vPositionCS	: POSITION,
							out float2 out_vDepthCS		: TEXCOORD0	)
{
	GenerateShadowMapVS_Base(in_vPositionOS, WorldMatrix, out_vPositionCS, out_vDepthCS);
}

void GenerateShadowMapVS_Instanced(	in float4 in_vPositionOS	: POSITION,
							in VertexShaderInput_Instance instanceData,
							out float4 out_vPositionCS	: POSITION,
							out float2 out_vDepthCS		: TEXCOORD0	)
{
	float4x4 instanceWorldMatrix = {instanceData.worldMatrixRow0,
									instanceData.worldMatrixRow1,
									instanceData.worldMatrixRow2,
									instanceData.worldMatrixRow3};

	GenerateShadowMapVS_Base(in_vPositionOS, instanceWorldMatrix, out_vPositionCS, out_vDepthCS);
}

// Vertex shader for outputting light-space depth to the shadow map for voxels
void GenerateVoxelShadowMapVS(	in float4 in_vPositionOS	: POSITION,
							out float4 out_vPositionCS	: POSITION,
							out float2 out_vDepthCS		: TEXCOORD0	)
{
	// Unpack voxel position
	float4 Position = float4(UnpackVoxelPosition(in_vPositionOS), 1);

	// Figure out the position of the vertex in view space and clip space
	
	Position =  mul(Position, WorldMatrix);
	out_vPositionCS = mul(Position, ViewProjMatrix);
	out_vDepthCS = out_vPositionCS.zw;
}

void GenerateVoxelShadowMapVS_Instanced(in float4 in_vPositionOS	: POSITION,
							in VertexShaderInput_Instance instanceData,
							out float4 out_vPositionCS	: POSITION,
							out float2 out_vDepthCS		: TEXCOORD0	)
{
	float4x4 instanceWorldMatrix = {instanceData.worldMatrixRow0,
									instanceData.worldMatrixRow1,
									instanceData.worldMatrixRow2,
									instanceData.worldMatrixRow3};

	// Unpack voxel position
	float4 Position = float4(UnpackVoxelPosition(in_vPositionOS), 1);

	// Figure out the position of the vertex in view space and clip space
	
	Position =  mul(Position, instanceWorldMatrix);
	out_vPositionCS = mul(Position, ViewProjMatrix);
	out_vDepthCS = out_vPositionCS.zw;
}

// Pixel shader for outputting light-space depth to the shadow map
float4 GenerateShadowMapPS(in float2 in_vDepthCS : TEXCOORD0) : COLOR0
{
	// Negate and divide by distance to far clip (so that depth is in range [0,1])
	float fDepth = in_vDepthCS.x / in_vDepthCS.y;			
		
#ifdef COLOR_SHADOW_MAP_FORMAT
	return EncodeFloatRGBAPrecise(fDepth);
#else
    return float4(fDepth, 0, 0, 0); 
#endif	
}

// Vertex shader for rendering the full-screen quad used for calculating
// the shadow occlusion factor.
void ShadowTermVS (	in float3 in_vPositionOS				: POSITION,
					in float3 in_vTexCoordAndCornerIndex	: TEXCOORD0,		
					out float4 out_vPositionCS				: POSITION,
					out float2 out_vTexCoord				: TEXCOORD0,
					out float3 out_vFrustumCornerVS			: TEXCOORD1	)
{
	// Offset the position by half a pixel to correctly align texels to pixels
	out_vPositionCS.x = in_vPositionOS.x - ShadowTermHalfPixel.x;
	out_vPositionCS.y = in_vPositionOS.y + ShadowTermHalfPixel.y;
	out_vPositionCS.z = in_vPositionOS.z;
	out_vPositionCS.w = 1.0f;
	
	// Pass along the texture coordiante and the position of the frustum corner
	out_vTexCoord = in_vTexCoordAndCornerIndex.xy + HalfPixel;
	out_vFrustumCornerVS = FrustumCornersVS[in_vTexCoordAndCornerIndex.z];
}	

// Pixel shader for computing the shadow occlusion factor
float4 ShadowTermPS(	in float2 in_vTexCoord			: TEXCOORD0,
						in float3 in_vFrustumCornerVS	: TEXCOORD1,
						uniform int iFilterSize	)	: COLOR0
{
/*
	NOT USED
	float fSceneDepth = DecodeFloatRGBA(tex2D(DepthTextureSampler,in_vTexCoord)) * FAR_PLANE_DISTANCE;
	
	float4 vPositionVS = float4(normalize(in_vFrustumCornerVS) * fSceneDepth, 1);
	
	float diff = 0;
	float3 fShadowTerm1 = GetShadowTermFromPosition(vPositionVS, vPositionVS.z, iFilterSize, 0, diff);

	float blendDiff = vPositionVS.z / -10.0f;
	float testDepth = vPositionVS.z - blendDiff;
	
	float3 fShadowTerm2 = GetShadowTermFromPosition(vPositionVS, testDepth, iFilterSize, 0, diff);
	float blend = saturate(diff / blendDiff);
		
	return float4( fShadowTerm1 * (1 - blend) + fShadowTerm2 * blend, 1);
	*/
	return float4(0,0,0, 1);
}

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};

VertexShaderInput ClearVS(VertexShaderInput input)
{
	//	We're using a full screen quad, no need for transforming vertices.
	VertexShaderInput output;
	output.Position = input.Position;
	output.TexCoord = input.TexCoord + HalfPixel;
	return output;
}

float4 ClearPS(VertexShaderInput input) : COLOR0
{
	return float4(1,1,1,1);
}

technique GenerateShadowMap
{
	pass p0
	{
		VertexShader = compile vs_2_0 GenerateShadowMapVS();
        PixelShader = compile ps_2_0 GenerateShadowMapPS();
	}
}

technique GenerateVoxelShadowMap
{
	pass p0
	{
		VertexShader = compile vs_2_0 GenerateVoxelShadowMapVS();
        PixelShader = compile ps_2_0 GenerateShadowMapPS();
	}
}

// Instancing requires SM 3.0
technique GenerateShadowMapInstanced
{
	pass p0
	{
		VertexShader = compile vs_3_0 GenerateShadowMapVS_Instanced();
        PixelShader = compile ps_3_0 GenerateShadowMapPS();
	}
}

// Instancing requires SM 3.0
technique GenerateVoxelShadowMapInstanced
{
	pass p0
	{
		VertexShader = compile vs_3_0 GenerateVoxelShadowMapVS_Instanced();
        PixelShader = compile ps_3_0 GenerateShadowMapPS();
	}
}

technique CreateShadowTerm2x2PCF
{
    pass p0
    {
		ZWriteEnable = false;
		ZEnable = false;
		AlphaBlendEnable = false;
		CullMode = NONE;

        VertexShader = compile vs_3_0 ShadowTermVS();
        PixelShader = compile ps_3_0 ShadowTermPS(2);	
    }
}

technique CreateShadowTerm3x3PCF
{
    pass p0
    {
		ZWriteEnable = false;
		ZEnable = false;
		AlphaBlendEnable = false;
		CullMode = NONE;

        VertexShader = compile vs_3_0 ShadowTermVS();
        PixelShader = compile ps_3_0 ShadowTermPS(3);	
    }
}

technique CreateShadowTerm5x5PCF
{
    pass p0
    {
		ZWriteEnable = false;
		ZEnable = false;
		AlphaBlendEnable = false;
		CullMode = NONE;

        VertexShader = compile vs_3_0 ShadowTermVS();
        PixelShader = compile ps_3_0 ShadowTermPS(5);	
    }
}

technique CreateShadowTerm7x7PCF
{
    pass p0
    {
		ZWriteEnable = false;
		ZEnable = false;
		AlphaBlendEnable = false;
		CullMode = NONE;

        VertexShader = compile vs_3_0 ShadowTermVS();
        PixelShader = compile ps_3_0 ShadowTermPS(7);	
    }
}

technique Clear
{
	pass p0
	{
		VertexShader = compile vs_3_0 ClearVS();
        PixelShader = compile ps_3_0 ClearPS();
	}
}
