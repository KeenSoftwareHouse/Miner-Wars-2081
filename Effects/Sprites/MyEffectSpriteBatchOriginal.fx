//-----------------------------------------------------------------------------
// SpriteBatch.fx
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------


// Input parameters.
//float2   ViewportSize    : register(c0);
//float2   TextureSize     : register(c1);
float4x4 MatrixTransform : register(c0);
sampler  TextureSampler  : register(s0);

struct vsOut
{
	float4 pos: POSITION0;
	float4 color: COLOR0;
	float2 tex: TEXCOORD0;
};

// Vertex shader for rendering sprites on Windows.
vsOut SpriteVertexShader(float4 color : COLOR0, float2 texCoord : TEXCOORD0, float4 position : POSITION0)
{
    // Apply the matrix transform.
	vsOut output;

    //output.pos = mul(position, transpose(MatrixTransform));
	output.pos = mul(position, MatrixTransform);
    output.color = color;
	output.tex = texCoord;

	return output;
	// Half pixel offset for correct texel centering.
	//position.xy -= 0.5;
//
	//// Viewport adjustment.
	//position.xy /= ViewportSize;
	//position.xy *= float2(2, -2);
	//position.xy -= float2(1, -1);
//
	//// Compute the texture coordinate.
	//texCoord /= TextureSize;
}

// Pixel shader for rendering sprites (shared between Windows and Xbox).
void SpritePixelShader(inout float4 color : COLOR0, float2 texCoord : TEXCOORD0)
{
    color *= tex2D(TextureSampler, texCoord);
}


technique SpriteBatch
{
	pass
	{
		VertexShader = compile vs_3_0 SpriteVertexShader();
		PixelShader  = compile ps_3_0 SpritePixelShader();
	}
}
