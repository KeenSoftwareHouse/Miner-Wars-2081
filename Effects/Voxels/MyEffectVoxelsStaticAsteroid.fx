#include "MyEffectVoxelsBase.fxh"
#include "../MyPerlinNoise.fxh"

float4x4	ViewMatrix;
float4x4	WorldMatrix;
float4x4	ProjectionMatrix;
float3	    DiffuseColor;
float3	    Highlight = 0;

float3	    FieldStart;
float3	    FieldDir;

float EmissivityPower1;
float Time;

struct VertexShaderInputInstance
{
	float4 worldMatrixRow0 : BLENDWEIGHT0;
 	float4 worldMatrixRow1 : BLENDWEIGHT1;
 	float4 worldMatrixRow2 : BLENDWEIGHT2;
 	float4 worldMatrixRow3 : BLENDWEIGHT3;
	float4 diffuse : BLENDWEIGHT4;
	float4 SpecularIntensity_SpecularPower_Emisivity_HighlightFlag : BLENDWEIGHT5;
};

struct VertexShaderOutputInstance
{
	float4 Diffuse : BLENDWEIGHT0;
	float4 SpecularIntensity_SpecularPower_Emisivity_HighlightFlag : BLENDWEIGHT1;
};

struct VertexShaderOutput_Instanced
{
	VertexShaderOutput BaseOutput;
	VertexShaderOutputInstance InstanceOutput;
};

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

MyGbufferPixelShaderOutput PixelShaderFunction_Base(VertexShaderOutput input, float3 diffuse, float3 highlight, float3 si_sp_e, uniform int renderQuality)
{
	MyGbufferPixelShaderOutput output = GetTriplanarPixel(0, input.WorldPositionForTextureCoords, input.TriplanarWeights, input.Normal, input.ViewDistance.x, si_sp_e.x, si_sp_e.y, input.Ambient, renderQuality);
	output.DiffuseAndSpecIntensity.rgb = output.DiffuseAndSpecIntensity.rgb * diffuse + highlight;
	output.DepthAndEmissivity.a = PackGBufferEmissivityReflection((1 - output.DepthAndEmissivity.a) - length(highlight), 0.0f); //Shader wont compile if I put there only length(highlight)...
	return output;
}


MyGbufferPixelShaderOutput PixelShaderFunction_Multimaterial(VertexShaderOutput input, uniform int renderQuality)
{/*
	if (input.ViewDistance < 200)
	{
		discard;
		return (MyGbufferPixelShaderOutput)0;
	}
*/
	MyGbufferPixelShaderOutput output0 = GetTriplanarPixel(0, input.WorldPositionForTextureCoords, input.TriplanarWeights, input.Normal, input.ViewDistance.x, SpecularIntensity, SpecularPower, input.Ambient, renderQuality);
	MyGbufferPixelShaderOutput output1 = GetTriplanarPixel(1, input.WorldPositionForTextureCoords, input.TriplanarWeights, input.Normal, input.ViewDistance.x, SpecularIntensity2, SpecularPower2, input.Ambient, renderQuality);


//	float noise = fBm(input.Normal, 32, 16, 0.01) * 0.5f + 0.5f;
	//float noise = noiseFog(input.WorldPositionForTextureCoords, 0, 0.001f, 50.0f, 0);
//	float noise = noiseFog(input.WorldPositionForTextureCoords, 0, 0.0007f, 20.4f, Time * 0.0001f);
	float noise = turbulence(input.WorldPositionForTextureCoords * 0.007f, 4, Time * 0.0001f);

	float field = clamp(dot(normalize(input.WorldPositionForTextureCoords - FieldStart), FieldDir), 0, 1);

	float3 color0 = output0.DiffuseAndSpecIntensity.rgb * DiffuseColor + Highlight;
	float3 color1 = output1.DiffuseAndSpecIntensity.rgb * DiffuseColor + Highlight;

	float3 color = lerp(color0, color1, field);
	output1.DepthAndEmissivity.a = pow(abs(output1.DepthAndEmissivity.a), EmissivityPower1);
	float emissivity = lerp(output0.DepthAndEmissivity.a, output1.DepthAndEmissivity.a-0.5f, field* noise);

	MyGbufferPixelShaderOutput output = output0;
	output.DiffuseAndSpecIntensity.rgb = color;
	output.DepthAndEmissivity.a = PackGBufferEmissivityReflection((1- emissivity)  -length(Highlight), 0.0f); //Shader wont compile if I put there only length(highlight)...
	return output;
}

MyGbufferPixelShaderOutput PixelShaderFunction(VertexShaderOutput input, uniform int renderQuality)
{
	//Cut pixels from LOD1 which are before LodNear
	if (IsPixelCut(input.ViewDistance.y))
	{
		discard;
		return (MyGbufferPixelShaderOutput)0;
		//return PixelShaderFunction_Base(input, float4(1,0,0,1), Highlight, float3(SpecularIntensity, SpecularPower, 0), renderQuality);
	}
	else					
		return PixelShaderFunction_Base(input, DiffuseColor, Highlight, float3(SpecularIntensity, SpecularPower, 0), renderQuality);
}

MyGbufferPixelShaderOutput PixelShaderFunction_Instanced(VertexShaderOutput_Instanced input, uniform int renderQuality)
{
	float3 si_sp_e = input.InstanceOutput.SpecularIntensity_SpecularPower_Emisivity_HighlightFlag.xyz;
	float3 highlight = input.InstanceOutput.SpecularIntensity_SpecularPower_Emisivity_HighlightFlag.w * Highlight;
	return PixelShaderFunction_Base(input.BaseOutput, input.InstanceOutput.Diffuse, highlight, si_sp_e, renderQuality);
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

VertexShaderOutputInstance VertexShaderInstance_Copy(VertexShaderInputInstance instanceData)
{
	VertexShaderOutputInstance output;
	output.Diffuse = instanceData.diffuse;
	output.SpecularIntensity_SpecularPower_Emisivity_HighlightFlag = instanceData.SpecularIntensity_SpecularPower_Emisivity_HighlightFlag;
	return output;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

VertexShaderOutput VertexShaderFunction_Base(VertexShaderInput input, float4x4 world)
{
	VertexShaderOutput output;

	float4 Position = UnpackPositionAndScale(input.PositionAndAmbient);
	float Ambient = 0;
	input.Normal = UnpackNormal(input.Normal);

	output.WorldPositionForTextureCoords = mul(Position.xyz, (float3x3)world);
	float4 viewPosition = mul(Position, world);
	viewPosition = mul(viewPosition, ViewMatrix);

	//	We need distance between camera and the vertex. We don't want to use just Z, or Z/W, we just need that distance.	
	output.ViewDistance.x = -viewPosition.z;
	output.ViewDistance.y = length(viewPosition);
	
	
    output.Position = mul(viewPosition, ProjectionMatrix);        	
	//output.ViewDistance = output.Position.z;

    output.Normal = mul(input.Normal.xyz, (float3x3)world);
    output.TriplanarWeights = GetTriplanarWeights(output.Normal);
	output.Ambient = Ambient;
    return output;
}

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    return VertexShaderFunction_Base(input, WorldMatrix);
}

VertexShaderOutput_Instanced VertexShaderFunction_Instanced(VertexShaderInput input, VertexShaderInputInstance instanceData)
{
	float4x4 instanceWorldMatrix = {instanceData.worldMatrixRow0,
									instanceData.worldMatrixRow1,
									instanceData.worldMatrixRow2,
									instanceData.worldMatrixRow3};

	VertexShaderOutput_Instanced output;
    output.BaseOutput = VertexShaderFunction_Base(input, instanceWorldMatrix);
	output.InstanceOutput = VertexShaderInstance_Copy(instanceData);
	return output;
}



float4 PixelShaderFunction_Forward(VertexShaderOutput input, uniform int renderQuality) : COLOR0
{
	MyGbufferPixelShaderOutput output = GetTriplanarPixel(0, input.WorldPositionForTextureCoords, input.TriplanarWeights, input.Normal, input.ViewDistance, SpecularIntensity, SpecularPower, input.Ambient, renderQuality);
	return float4(output.DiffuseAndSpecIntensity.rgb * input.Normal * DiffuseColor + Highlight, 1);
}

VertexShaderOutput VertexShaderFunction_Forward(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 Position = UnpackPositionAndScale(input.PositionAndAmbient);
	float Ambient = 0;
	input.Normal = UnpackNormal(input.Normal);

	output.WorldPositionForTextureCoords = mul(Position.xyz, (float3x3)WorldMatrix);
	float4 worldPosition = mul(Position, WorldMatrix);
	float4 viewPosition = mul(worldPosition, ViewMatrix);

	//	We need distance between camera and the vertex. We don't want to use just Z, or Z/W, we just need that distance.	
	output.ViewDistance.x = -viewPosition.z;
	output.ViewDistance.y = length(viewPosition);

	output.Position = mul(viewPosition, ProjectionMatrix);        	
	//output.ViewDistance = output.Position.z;

	output.Normal = mul(input.Normal.xyz, (float3x3)WorldMatrix);
	output.TriplanarWeights = GetTriplanarWeights(output.Normal);
// 	output.Ambient = Ambient;
// 	return output;

	// Forward lighting
	//float lightIntensity = ComputeForwardLighting(worldPosition, input.Normal);
	//output.Ambient = 0.1f /*+ saturate(0.3f * output.Ambient)*/ + 0.7f * saturate(abs(lightIntensity));

	float4 lightColor = 0;//ComputeForwardLighting(worldPosition, input.Normal);
	float ambient = 0.15f * float3(1,1,1);
	output.Normal = lightColor.xyz + ambient;
	output.Ambient = 0;

	return output;
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

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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



technique Technique_RenderQualityLow_Instanced
{
    pass Pass1
    {
        DECLARE_TEXTURES_QUALITY_LOW

        VertexShader = compile vs_3_0 VertexShaderFunction_Instanced();
        PixelShader = compile ps_3_0 PixelShaderFunction_Instanced(RENDER_QUALITY_LOW);
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

technique Technique_RenderQualityNormal_Instanced
{
    pass Pass1
    {
        DECLARE_TEXTURES_QUALITY_NORMAL

        VertexShader = compile vs_3_0 VertexShaderFunction_Instanced();
        PixelShader = compile ps_3_0 PixelShaderFunction_Instanced(RENDER_QUALITY_NORMAL);
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

technique Technique_RenderQualityHigh_Instanced
{
    pass Pass1
    {
        DECLARE_TEXTURES_QUALITY_HIGH

        VertexShader = compile vs_3_0 VertexShaderFunction_Instanced();
        PixelShader = compile ps_3_0 PixelShaderFunction_Instanced(RENDER_QUALITY_HIGH);
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

technique Technique_RenderQualityExtreme_Instanced
{
    pass Pass1
    {		
        DECLARE_TEXTURES_QUALITY_EXTREME
        
        VertexShader = compile vs_3_0 VertexShaderFunction_Instanced();
        PixelShader = compile ps_3_0 PixelShaderFunction_Instanced(RENDER_QUALITY_EXTREME);
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

technique Technique_RenderQualityNormal_Multimaterial
{
    pass Pass1
    {
        DECLARE_TEXTURES_QUALITY_NORMAL

        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction_Multimaterial(RENDER_QUALITY_NORMAL);
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

technique Technique_RenderQualityHigh_Multimaterial
{
    pass Pass1
    {
        DECLARE_TEXTURES_QUALITY_HIGH

        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction_Multimaterial(RENDER_QUALITY_HIGH);
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

technique Technique_RenderQualityExtreme_Multimaterial
{
    pass Pass1
    {		
        DECLARE_TEXTURES_QUALITY_EXTREME
        
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction_Multimaterial(RENDER_QUALITY_EXTREME);
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////