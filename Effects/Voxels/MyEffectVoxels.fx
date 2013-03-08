#include "MyEffectVoxelsBase.fxh"


float3		VoxelMapPosition;
float4x4	ViewMatrix;
float4x4	ProjectionMatrix;
float3      DiffuseColor;
float3      Highlight;

// for forward render
float4x4	WorldViewProjectionMatrix;
float4x4	WorldMatrix;


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

MyGbufferPixelShaderOutput PixelShaderFunction(VertexShaderOutput input, uniform int renderQuality)
{
    //Cut pixels from LOD1 which are before LodNear

    if (IsPixelCut(input.ViewDistance.y))
    {
        discard;
        return (MyGbufferPixelShaderOutput)0;
    }
    else	 
    {
        MyGbufferPixelShaderOutput output = GetTriplanarPixel(0, input.WorldPositionForTextureCoords, input.TriplanarWeights, input.Normal, input.ViewDistance.x, SpecularIntensity, SpecularPower, input.Ambient, renderQuality);
        output.DiffuseAndSpecIntensity.rgb = output.DiffuseAndSpecIntensity.rgb * DiffuseColor + Highlight;
        output.DepthAndEmissivity.a = PackGBufferEmissivityReflection((1 - output.DepthAndEmissivity.w) + length(Highlight), 0.0f); //inverted emissivity, zero reflection
        return output;
    }
}

MyGbufferPixelShaderOutput PixelShaderFunction_Multimaterial(VertexShaderOutput_Multimaterial multiInput, uniform int renderQuality)
{
	VertexShaderOutput input = multiInput.Single;
	if (IsPixelCut(input.ViewDistance.y))
    {
        discard;
        return (MyGbufferPixelShaderOutput)0;
    }
	else	 
    {
		VertexShaderOutput input = multiInput.Single;

		MyGbufferPixelShaderOutput output0 = GetTriplanarPixel(0, input.WorldPositionForTextureCoords, input.TriplanarWeights, input.Normal, input.ViewDistance.x, SpecularIntensity, SpecularPower, input.Ambient, renderQuality);
		MyGbufferPixelShaderOutput output1 = GetTriplanarPixel(1, input.WorldPositionForTextureCoords, input.TriplanarWeights, input.Normal, input.ViewDistance.x, SpecularIntensity2, SpecularPower2, input.Ambient, renderQuality);
		MyGbufferPixelShaderOutput output2 = GetTriplanarPixel(2, input.WorldPositionForTextureCoords, input.TriplanarWeights, input.Normal, input.ViewDistance.x, SpecularIntensity3, SpecularPower3, input.Ambient, renderQuality);

		MyGbufferPixelShaderOutput output;
		output.NormalAndSpecPower = output0.NormalAndSpecPower * multiInput.Alpha.x + output1.NormalAndSpecPower * multiInput.Alpha.y + output2.NormalAndSpecPower * multiInput.Alpha.z;
		output.DiffuseAndSpecIntensity = output0.DiffuseAndSpecIntensity * multiInput.Alpha.x + output1.DiffuseAndSpecIntensity * multiInput.Alpha.y + output2.DiffuseAndSpecIntensity * multiInput.Alpha.z;
		output.DepthAndEmissivity = output0.DepthAndEmissivity * multiInput.Alpha.x + output1.DepthAndEmissivity * multiInput.Alpha.y + output2.DepthAndEmissivity * multiInput.Alpha.z;

		output.DiffuseAndSpecIntensity.rgb = output.DiffuseAndSpecIntensity.rgb * DiffuseColor + Highlight;
		output.DepthAndEmissivity.a = PackGBufferEmissivityReflection((1 - output.DepthAndEmissivity.w) + length(Highlight), 0.0f); //inverted emissivity, zero reflection

		return output;
	}
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    input.PositionAndAmbient.xyz = UnpackVoxelPosition(input.PositionAndAmbient);
    output.Ambient = UnpackVoxelAmbient(input.PositionAndAmbient);
    input.Normal = UnpackNormal(input.Normal);

    output.WorldPositionForTextureCoords = input.PositionAndAmbient.xyz;	
    float4 viewPosition = mul(float4(input.PositionAndAmbient.xyz + VoxelMapPosition, 1), ViewMatrix);

    //	We need distance between camera and the vertex. We don't want to use just Z, or Z/W, we just need that distance.	
    output.ViewDistance.x = -viewPosition.z;
    output.ViewDistance.y = length(viewPosition);
    
    output.Position = mul(viewPosition, ProjectionMatrix);
    //output.ViewDistance = output.Position.z;
    output.Normal = input.Normal;
    output.TriplanarWeights = GetTriplanarWeights(output.Normal);
    
    return output;
}

VertexShaderOutput_Multimaterial VertexShaderFunction_Multimaterial(VertexShaderInput input)
{
    VertexShaderOutput_Multimaterial output;
    output.Alpha = UnpackVoxelAlpha(input.PositionAndAmbient);
    output.Single = VertexShaderFunction(input);
    return output;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

VertexShaderOutput_Forward VertexShaderFunction_Forward(VertexShaderInput input)
{
    VertexShaderOutput_Forward output;

    input.PositionAndAmbient.xyz = UnpackVoxelPosition(input.PositionAndAmbient);
    input.Normal = UnpackNormal(input.Normal);

    output.WorldPositionForTextureCoords = input.PositionAndAmbient.xyz;	
    float4 worldPosition = float4(input.PositionAndAmbient.xyz + VoxelMapPosition, 1);
    float4 viewPosition = mul(worldPosition, ViewMatrix);
    output.WorldPosition = worldPosition.xyz;	
    output.Ambient = UnpackVoxelAmbient(input.PositionAndAmbient);

    //	We need distance between camera and the vertex. We don't want to use just Z, or Z/W, we just need that distance.	
    output.ViewDistance = -viewPosition.z;
    //output.ViewDistanceRadial = length(viewPosition);
    
    output.Position = mul(viewPosition, ProjectionMatrix);
    output.Normal = input.Normal;
    output.TriplanarWeights = GetTriplanarWeights(output.Normal);

    //Lighting
    float4 lightColor = CalculateDynamicLight_Diffuse(worldPosition, input.Normal);
    output.LightColor = lightColor;

    return output;
}


float4 PixelShaderFunctionLow_Forward(VertexShaderOutput_Forward input, uniform int renderQuality) : COLOR0
{
//	MyGbufferPixelShaderOutput output = GetTriplanarPixel(0, input.WorldPositionForTextureCoords, input.TriplanarWeights, input.Normal, input.ViewDistance, SpecularIntensity, SpecularPower, input.Ambient, renderQuality);
//	output.DiffuseAndSpecIntensity.rgb = output.DiffuseAndSpecIntensity.rgb * DiffuseColor + Highlight;
//	output.DepthAndEmissivity.a = PackGBufferEmissivityReflection((1 - output.DepthAndEmissivity.w) - length(Highlight), 0.0f); //inverted emissivity, zero reflection
//	return output;
        
               
    if (IsPixelCut(input.ViewDistance.x))
    {
        discard;
        return float4(1,1,1,1);
    }	  

    //float4 diffuseTexture = tex2D(TextureDiffuseForAxisXZSampler[0], input.WorldPositionForTextureCoords.zy / TEXTURE_SCALE_FAR); 
    float3 diffuseTexture = GetTriplanarPixel(0, input.WorldPositionForTextureCoords, input.TriplanarWeights, input.Normal, input.ViewDistance.x, SpecularIntensity, SpecularPower, input.Ambient, renderQuality).DiffuseAndSpecIntensity.rgb;

    //MyGbufferPixelShaderOutput output = GetTriplanarPixel(0, input.WorldPositionForTextureCoords, input.TriplanarWeights, input.Normal, input.ViewDistance, SpecularIntensity, SpecularPower, input.Ambient, renderQuality);

    //output.DiffuseAndSpecIntensity.rgb = output.DiffuseAndSpecIntensity.rgb * DiffuseColor + Highlight;
    //output.DepthAndEmissivity.a = PackGBufferEmissivityReflection((1 - output.DepthAndEmissivity.w) + length(Highlight), 0.0f); //inverted emissivity, zero reflection

    float4 color = float4(diffuseTexture, 1) * float4(DiffuseColor, 1);
    color.xyz = color * (/*input.Ambient +*/ AmbientColor + Highlight  + input.LightColor + /*GetSunColor(input.Normal) + */GetReflectorColor(input.WorldPosition));
    color.w = 1;

    return color;
}


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

technique Technique_RenderQualityLow_Forward
{
    pass Pass1
    {
        DECLARE_TEXTURES_QUALITY_LOW

        VertexShader = compile vs_2_0 VertexShaderFunction_Forward();
        PixelShader = compile ps_2_0 PixelShaderFunctionLow_Forward(RENDER_QUALITY_LOW);
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
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

technique Technique_RenderQualityNormal_Multimaterial
{
    pass Pass1
    {
        DECLARE_TEXTURES_QUALITY_NORMAL

        VertexShader = compile vs_3_0 VertexShaderFunction_Multimaterial();
        PixelShader = compile ps_3_0 PixelShaderFunction_Multimaterial(RENDER_QUALITY_NORMAL);
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

technique Technique_RenderQualityHigh_Multimaterial
{
    pass Pass1
    {
        DECLARE_TEXTURES_QUALITY_HIGH

        VertexShader = compile vs_3_0 VertexShaderFunction_Multimaterial();
        PixelShader = compile ps_3_0 PixelShaderFunction_Multimaterial(RENDER_QUALITY_HIGH);
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

technique Technique_RenderQualityExtreme_Multimaterial
{
    pass Pass1
    {		
        DECLARE_TEXTURES_QUALITY_EXTREME
        
        VertexShader = compile vs_3_0 VertexShaderFunction_Multimaterial();
        PixelShader = compile ps_3_0 PixelShaderFunction_Multimaterial(RENDER_QUALITY_EXTREME);
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
