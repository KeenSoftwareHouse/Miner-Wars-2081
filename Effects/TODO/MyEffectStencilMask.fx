float4x4 ProjectionMatrix;
float ThresholdAlpha;

Texture StencilTexture;
sampler StencilTextureSampler = sampler_state 
{ 
	texture = <StencilTexture> ; 
	magfilter = LINEAR; 
	minfilter = LINEAR; 
	mipfilter = LINEAR; 
	AddressU = clamp; 
	AddressV = clamp;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    output.Position = mul(input.Position, ProjectionMatrix);
    output.TexCoord = input.TexCoord;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{	
	float alphaTexture = tex2D(StencilTextureSampler, input.TexCoord).x;
	
	//	Function 'clip' - discards the current pixel if the specified value is less than zero.	
	//	So the pixel will survive only if its texel alpha is greater or equal than 'threshold alpha' - it will survive and will get rendered as 1 into stencil buffer.
	//	Otherwise it gets discarded and will not be written into stencil buffer (and of course not into color buffer)
	clip(alphaTexture >= ThresholdAlpha ? 0 : -1);

	//	This will be called only if pixel survived the 'discard' above (but I also think that GPU will execute it no matter what and only then do the discard operation)
    return float4(1, 1, 1, alphaTexture);
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}