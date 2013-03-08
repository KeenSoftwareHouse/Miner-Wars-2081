#include "../MyEffectBase.fxh"

float3 ColorA;
float AlphaA;

float4x4 WorldMatrix;
float4x4 ViewProjectionMatrix;

Texture GridTexture;
sampler GridTextureSampler = sampler_state 
{ 
	texture = <GridTexture> ; 
	magfilter = LINEAR; 
	minfilter = LINEAR; 
	mipfilter = LINEAR; 
	AddressU = WRAP;
	AddressV = WRAP;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float3 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float3 TexCoord : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    float4 worldPosition = mul(input.Position, WorldMatrix);
    output.Position = mul(worldPosition, ViewProjectionMatrix);
    output.TexCoord = input.TexCoord;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float alpha = tex2D(GridTextureSampler, input.TexCoord).a;
	float4 resultColor = float4(ColorA, alpha * AlphaA);
	return resultColor;
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
