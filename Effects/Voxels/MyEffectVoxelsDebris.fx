#include "../Voxels/MyEffectVoxelsBase.fxh"

//	This shader renders a model with same shader as voxels (triplanar mapping), using diffuse & specular & normal map textures
//	But we use one texture-set for XZ plane, and the other for Y plan

float4x4	WorldMatrix;
float4x4	ViewWorldScaleMatrix;
float4x4	ProjectionMatrix;

//  This applies only for explosion debris, because we want to add some randomization to 'world position to texture coord' transformation
float		TextureCoordRandomPositionOffset;
float		TextureCoordScale;

//	Add random color overlay on explosion debris diffuse texture output
float		DiffuseTextureColorMultiplier;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

MyGbufferPixelShaderOutput PixelShaderFunction(VertexShaderOutput input, uniform int renderQuality)
{
	MyGbufferPixelShaderOutput output = GetTriplanarPixel(0, input.WorldPositionForTextureCoords, input.TriplanarWeights, input.Normal, input.ViewDistance.x, SpecularIntensity, SpecularPower, input.Ambient, renderQuality);
	output.DiffuseAndSpecIntensity.rgb *= DiffuseTextureColorMultiplier;
	output.DepthAndEmissivity.a = PackGBufferEmissivityReflection(1 - output.DepthAndEmissivity.w, 0.0f); //inverted emissivity, zero reflection
	return output;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	input.PositionAndAmbient = UnpackPositionAndScale(input.PositionAndAmbient);
	float4 Position = float4(input.PositionAndAmbient.xyz, 1);
	float Ambient = input.PositionAndAmbient.w;
	input.Normal = UnpackNormal(input.Normal);

	//	Here we get texture coord based on object-space coordinates of model. The correct way would be to do this
	//	after ScaleMatrix multiplication, but this version is faster (one less mul in vertex shader) and it looks
	//	fine, I am going with this one.
	output.WorldPositionForTextureCoords = (Position + TextureCoordRandomPositionOffset) * TextureCoordScale;
	float4 viewPosition = mul(Position, ViewWorldScaleMatrix);

	//	We need distance between camera and the vertex. We don't want to use just Z, or Z/W, we just need that distance.	
	output.ViewDistance.x = -viewPosition.z;
	output.ViewDistance.y = length(viewPosition);
	
    output.Position = mul(viewPosition, ProjectionMatrix);        	
    output.Normal = normalize(mul(input.Normal.xyz, (float3x3)WorldMatrix));
    output.TriplanarWeights = GetTriplanarWeights(output.Normal);
	output.Ambient = Ambient;
    return output;
}

VertexShaderOutput VertexShaderFunction_Forward(VertexShaderInput input)
{
	VertexShaderOutput output;

	input.PositionAndAmbient = UnpackPositionAndScale(input.PositionAndAmbient);
	float4 Position = float4(input.PositionAndAmbient.xyz, 1);
	float Ambient = input.PositionAndAmbient.w;
	input.Normal = UnpackNormal(input.Normal);

	//	Here we get texture coord based on object-space coordinates of model. The correct way would be to do this
	//	after ScaleMatrix multiplication, but this version is faster (one less mul in vertex shader) and it looks
	//	fine, I am going with this one.
	output.WorldPositionForTextureCoords = (Position + TextureCoordRandomPositionOffset) * TextureCoordScale;
	float4 viewPosition = mul(Position, ViewWorldScaleMatrix);

	//	We need distance between camera and the vertex. We don't want to use just Z, or Z/W, we just need that distance.	
	output.ViewDistance.x = -viewPosition.z;
	output.ViewDistance.y = length(viewPosition);


	output.Position = mul(viewPosition, ProjectionMatrix);        	
	output.Normal = normalize(mul(input.Normal.xyz, (float3x3)WorldMatrix));
	output.TriplanarWeights = GetTriplanarWeights(output.Normal);
	output.Ambient = Ambient;

	float4 worldPosition = mul(Position, WorldMatrix);

	// Lighting
	//float lightIntensity = ComputeForwardLighting(worldPosition.xyz, output.Normal);

	float4 lightColor = 0;//ComputeForwardLighting(worldPosition, input.Normal);
	float3 ambient = 0.15f * float3(1,1,1);
	output.Normal = lightColor.xyz + ambient;

	//output.Ambient = 0.1f + saturate(0.25 * output.Ambient) + lightIntensity;

	return output;
}

float4 PixelShaderFunction_Forward(VertexShaderOutput input, uniform int renderQuality) : COLOR0
{
	MyGbufferPixelShaderOutput output = GetTriplanarPixel(0, input.WorldPositionForTextureCoords, input.TriplanarWeights, input.Normal, input.ViewDistance.x, SpecularIntensity, SpecularPower, input.Ambient, renderQuality);
	output.DiffuseAndSpecIntensity.rgb *= DiffuseTextureColorMultiplier;
	output.DepthAndEmissivity.a = PackGBufferEmissivityReflection(1 - output.DepthAndEmissivity.w, 0.0f); //inverted emissivity, zero reflection

	output.NormalAndSpecPower = float4(1,0,0,1);
	return float4(output.DiffuseAndSpecIntensity.rgb * input.Normal /*Color is in Normal*/, 1);
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

technique Technique_RenderQualityLow_Forward
{
	pass Pass1
	{
		DECLARE_TEXTURES_QUALITY_LOW

		VertexShader = compile vs_3_0 VertexShaderFunction_Forward();
		PixelShader = compile ps_3_0 PixelShaderFunction_Forward(RENDER_QUALITY_LOW);
	}
}

technique Technique_RenderQualityLow
{
    pass Pass1
    {
        DECLARE_TEXTURES_QUALITY_LOW

        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction(RENDER_QUALITY_LOW);
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

technique Technique_RenderQualityNormal
{
    pass Pass1
    {
        DECLARE_TEXTURES_QUALITY_NORMAL

        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction(RENDER_QUALITY_NORMAL);
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

technique Technique_RenderQualityHigh
{
    pass Pass1
    {
        DECLARE_TEXTURES_QUALITY_HIGH

        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction(RENDER_QUALITY_HIGH);
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

technique Technique_RenderQualityExtreme
{
    pass Pass1
    {		
        DECLARE_TEXTURES_QUALITY_EXTREME
        
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction(RENDER_QUALITY_EXTREME);
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////