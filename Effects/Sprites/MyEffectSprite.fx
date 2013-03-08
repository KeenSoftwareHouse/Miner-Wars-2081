#include "../MyEffectBase.fxh"

float2 HalfPixel;

Texture SpriteTexture;
sampler SpriteTextureSampler = sampler_state 
{ 
	texture = <SpriteTexture> ; 
	magfilter = LINEAR; 
	minfilter = LINEAR; 
	mipfilter = LINEAR; 
	AddressU = clamp; 
	AddressV = clamp;
};

TextureCube SpriteTextureCube;
sampler SpriteTextureSamplerCube = sampler_state 
{ 
	texture = <SpriteTextureCube> ; 
	magfilter = LINEAR; 
	minfilter = LINEAR; 
	mipfilter = NONE; 
	AddressU = clamp; 
	AddressV = clamp;
};


struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float4 Color : COLOR0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float4 Color : COLOR0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = input.Position;
    output.TexCoord = input.TexCoord + HalfPixel;
    output.Color = input.Color;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    return tex2D(SpriteTextureSampler, input.TexCoord) * input.Color;    
}

float4 PixelShaderFunctionCube(VertexShaderOutput input, uniform int faceIndex) : COLOR0
{
	float2 c = (input.TexCoord - 0.5f) * 2;

	float3 coord;

	if(faceIndex == 0) coord = float3(1, -c.y, -c.x);
	else if(faceIndex == 1) coord = float3(-1, -c.y, c.x);
	else if(faceIndex == 2) coord = float3(c.x, 1, c.y);
	else if(faceIndex == 3) coord = float3(c.x, -1, -c.y);
	else if(faceIndex == 4) coord = float3(c.x, -c.y, 1.0f);
	else coord = float3(-c.x, -c.y, -1.0f);

    return texCUBE(SpriteTextureSamplerCube, coord) * input.Color;    
}

technique Technique_RenderQualityNormal
{
    pass Pass1
    {
		VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}

technique Technique_RenderQualityNormalCube0
{
    pass Pass1
    {
		VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunctionCube(0);
    }
}

technique Technique_RenderQualityNormalCube1
{
    pass Pass1
    {
		VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunctionCube(1);
    }
}

technique Technique_RenderQualityNormalCube2
{
    pass Pass1
    {
		VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunctionCube(2);
    }
}

technique Technique_RenderQualityNormalCube3
{
    pass Pass1
    {
		VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunctionCube(3);
    }
}

technique Technique_RenderQualityNormalCube4
{
    pass Pass1
    {
		VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunctionCube(4);
    }
}

technique Technique_RenderQualityNormalCube5
{
    pass Pass1
    {
		VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunctionCube(5);
    }
}
