#include "../MyEffectBase.fxh"
#include "../MyPerlinNoise.fxh"


Texture SourceRT;
sampler SourceRTSampler = sampler_state 
{ 
	texture = <SourceRT> ; 
	magfilter = POINT; 
	minfilter = POINT; 
	mipfilter = NONE; 
	AddressU = CLAMP; 
	AddressV = CLAMP;
};

Texture DepthsRT;
sampler DepthsRTSampler = sampler_state 
{ 
	texture = <DepthsRT> ; 
	magfilter = POINT; 
	minfilter = POINT; 
	mipfilter = NONE; 
	AddressU = CLAMP; 
	AddressV = CLAMP;
};

Texture NormalsTexture;
sampler NormalsTextureSampler = sampler_state 
{ 
	texture = <NormalsTexture> ; 
	magfilter = LINEAR; 
	minfilter = LINEAR; 
	mipfilter = NONE; 
	AddressU = WRAP; 
	AddressV = WRAP;
};


float2 HalfPixel;
float3 FrustumCorners[4];

float4x4 WorldMatrix;
float4x4 ViewProjectionMatrix;
float4x4 CameraMatrix;
float3 CameraPos;

float2 Scale = float2(1.0f, 1.0f);

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float3 TexCoordAndCornerIndex	: TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;	
	//float3 WorldPos : TEXCOORD1;
	float3 FrustumCorner : TEXCOORD1; 
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;
	output.Position = input.Position;
	output.TexCoord = (input.TexCoordAndCornerIndex.xy + HalfPixel) * Scale;
	output.FrustumCorner = FrustumCorners[input.TexCoordAndCornerIndex.z];
	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input, float2 screenPosition : VPOS) : COLOR0
{
	float4 encodedDepth = tex2D(DepthsRTSampler, input.TexCoord);
	float depth = DecodeFloatRGBA(encodedDepth);
	float depthForTest = GetViewDistanceFromDepth(depth, input.FrustumCorner);
	float4 sourceColor = tex2D(SourceRTSampler, input.TexCoord);

	float3 normal = GetNormalVectorFromRenderTarget(tex2D(NormalsTextureSampler, input.TexCoord).xyz);
	float blend = length(normal);
//	if(length(normal) < 0.1f) 
//		return sourceColor;
	/*
	float4 viewPos = float4(GetViewPositionFromDepth(min(0.4f, depth), input.FrustumCorner), 1);
	float4 worldPos4 = mul(viewPos, CameraMatrix);
	float3 worldPos = worldPos4.xyz;
	float f = noiseFog(worldPos, CameraPos, 0.001f, 4.4f, 0);
	
	return float4(f.xxx, 1.0f); 
*/
	//Linear fog
	
	//Exponential fog
	float density = 0.36f;
	//float3 diffuse = CalculateFogExponencial(sourceColor.xyz, depthForTest, density);

	float3 diffuse = sourceColor;

	//if (length(sourceColor) < 2)
	return CalculateFogLinear(sourceColor.xyz, depthForTest, blend);

	//float f = turbulence(input.WorldPos * 0.1f, 4, 0);
	//return float4(sourceColor.xyz, 1.0f);
	//return float4(diffuse, 1.0f);
}



float4 SkipBackgroundPixelShaderFunction(VertexShaderOutput input, float2 screenPosition : VPOS) : COLOR0
{
	float4 encodedDepth = tex2D(DepthsRTSampler, input.TexCoord);
	float depth = DecodeFloatRGBA(encodedDepth);
	float depthForTest = GetViewDistanceFromDepth(depth, input.FrustumCorner);
	float4 sourceColor = tex2D(SourceRTSampler, input.TexCoord);

	float3 normal = GetNormalVectorFromRenderTarget(tex2D(NormalsTextureSampler, input.TexCoord).xyz);
	float blend = length(normal);

	//Linear fog

	float4 returnColor = float4(0,0,0,0);

	if (depthForTest < 50000)	
		returnColor = CalculateFogLinear(sourceColor.xyz, depthForTest, blend);
	
	return returnColor;
}

technique Technique1
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
}

technique SkipBackgroundTechnique
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 SkipBackgroundPixelShaderFunction();
	}
}
