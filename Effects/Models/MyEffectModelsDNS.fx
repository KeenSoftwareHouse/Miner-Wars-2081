
#include "../MyEffectDynamicLightingBase.fxh"
#include "../MyEffectReflectorBase.fxh"

//	This shader renders a model with diffuse & specular & normal map textures, so it requires certain vertex shader data

//const float CHANNEL_TEXTURE_SCALE = 40;

float4x4	WorldMatrix;
float4x4	ViewMatrix;
float4x4	ProjectionMatrix;
float3	    DiffuseColor; 
float	    Emissivity = 0; 
float	    EmissivityOffset = 0; 
float2	    EmissivityUVAnim; 
float2	    DiffuseUVAnim; 
float3	    Highlight = 0; 

float		SpecularIntensity = 1;
float		SpecularPower = 1;

//float		Channel0Intensity = 0;
//float		Channel1Intensity = 0;
//float		Channel2Intensity = 0;
//float		Channel3Intensity = 0;

float2 HalfPixel;
float2 Scale;
Texture TextureDiffuse;
sampler TextureDiffuseSampler = sampler_state 
{ 
	texture = <TextureDiffuse> ; 
	mipfilter = LINEAR; 
	AddressU = WRAP; 
	AddressV = WRAP;
};

Texture TextureNormal;
sampler TextureNormalSampler = sampler_state 
{ 
	texture = <TextureNormal> ; 
	mipfilter = LINEAR; 
	AddressU = WRAP; 
	AddressV = WRAP;
};

/*
Texture TextureMask;
sampler TextureMaskSampler = sampler_state 
{ 
	texture = <TextureMask> ; 
	mipfilter = LINEAR; 
	AddressU = WRAP; 
	AddressV = WRAP;
};

Texture TextureChannel0;
sampler TextureChannel0Sampler = sampler_state 
{ 
	texture = <TextureChannel0> ; 
	mipfilter = LINEAR; 
	AddressU = WRAP; 
	AddressV = WRAP;
};

Texture TextureChannel1;
sampler TextureChannel1Sampler = sampler_state 
{ 
	texture = <TextureChannel1> ; 
	mipfilter = LINEAR; 
	AddressU = WRAP; 
	AddressV = WRAP;
};
  */

//This sampler is used for HOLO objects
Texture DepthTextureNear;
sampler DepthTextureNearSampler = sampler_state 
{ 
	texture = <DepthTextureNear>; 
	magfilter = POINT; 
	minfilter = POINT;
	mipfilter = NONE; 
	AddressU = WRAP; 
	AddressV = WRAP;
};

//This sampler is used for HOLO objects
Texture DepthTextureFar;
sampler DepthTextureFarSampler = sampler_state 
{ 
	texture = <DepthTextureFar>; 
	magfilter = POINT; 
	minfilter = POINT;
	mipfilter = NONE; 
	AddressU = WRAP; 
	AddressV = WRAP;
};

// DNS low
struct VertexShaderInputLow_DNS
{
    float4 Position : POSITION0;
    float4 Normal : NORMAL;
    float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutputLow_DNS
{
    float4 Position : POSITION0;
    float4 TexCoordAndViewDistance : TEXCOORD0; //z is linear depth, w is radial depth
	float4 ScreenPosition : TEXCOORD1;
	float3 Normal : TEXCOORD2;
	float3 WorldPos : TEXCOORD3;
};

struct VertexShaderOutputForward_DNS
{
    float4 Position : POSITION0;
	float4 LightColor : COLOR0;
    float2 TexCoord : TEXCOORD0;
	float3 WorldPos : TEXCOORD1;
};

struct VertexShaderOutputInstance
{
	float4 Diffuse : BLENDWEIGHT0;
	float4 SpecularIntensity_SpecularPower_Emisivity_None : BLENDWEIGHT1;
	float3 Highlight : BLENDWEIGHT2;
};

struct VertexShaderOutputLow_DNS_Instanced
{
	VertexShaderOutputLow_DNS BaseOutput;
	VertexShaderOutputInstance InstanceOutput;
};

// DNS normal
struct VertexShaderInput_DNS
{
    VertexShaderInputLow_DNS BaseInput;
    float4 Tangent : TANGENT;
    float4 Binormal : BINORMAL;
};

struct VertexShaderOutput_DNS
{
    VertexShaderOutputLow_DNS BaseOutput;
    float3x3 TangentToWorld : TEXCOORD5;
};

struct VertexShaderOutput_DNS_Instanced
{
	VertexShaderOutput_DNS BaseOutput;
	VertexShaderOutputInstance InstanceOutput;
};
		  /*
// Channels normal & higher
struct VertexShaderInput_DNS_Channels
{
    VertexShaderInput_DNS Input;
	float2 MaskCoord : TEXCOORD1;
};

struct VertexShaderOutput_DNS_Channels
{
    VertexShaderOutput_DNS Output;
	float2 MaskCoord : TEXCOORD8;
};
			
// Instance data
struct VertexShaderInput_Instance
{
	float4 worldMatrixRow0 : BLENDWEIGHT0;
 	float4 worldMatrixRow1 : BLENDWEIGHT1;
 	float4 worldMatrixRow2 : BLENDWEIGHT2;
 	float4 worldMatrixRow3 : BLENDWEIGHT3;
	float4 diffuse : BLENDWEIGHT4;
	float4 SpecularIntensity_SpecularPower_Emisivity_None : BLENDWEIGHT5;
	float3 Highlight : BLENDWEIGHT6;
};

VertexShaderOutputInstance VertexShaderInstance_Base(VertexShaderInput_Instance instanceData)
{
	VertexShaderOutputInstance output;
	output.Diffuse = instanceData.diffuse;
	output.SpecularIntensity_SpecularPower_Emisivity_None = instanceData.SpecularIntensity_SpecularPower_Emisivity_None;
	output.Highlight = instanceData.Highlight;
	return output;
}
			*/
// Low VS

VertexShaderOutputLow_DNS VertexShaderFunctionLow_DNS_Base(VertexShaderInputLow_DNS input, float4x4 world)
{
	VertexShaderOutputLow_DNS output;

	input.Position = UnpackPositionAndScale(input.Position);
	input.Normal = UnpackNormal(input.Normal);

	output.WorldPos = input.Position;
	output.Position = mul(input.Position, world);
    output.Position = mul(output.Position, ViewMatrix);
	output.TexCoordAndViewDistance.z = -output.Position.z;
	output.TexCoordAndViewDistance.w = length(output.Position.xyz);
    output.Position = mul(output.Position, ProjectionMatrix);    
	output.ScreenPosition = output.Position;
    output.TexCoordAndViewDistance.xy = input.TexCoord;
	output.Normal =  normalize(mul(input.Normal.xyz, (float3x3)world));    

    return output;
}

VertexShaderOutputLow_DNS VertexShaderFunctionLow_DNS(VertexShaderInputLow_DNS input)
{
    return VertexShaderFunctionLow_DNS_Base(input, WorldMatrix);
}
/*
VertexShaderOutputLow_DNS_Instanced VertexShaderFunctionLow_DNS_Instanced(VertexShaderInputLow_DNS input, VertexShaderInput_Instance instanceData)
{
	float4x4 instanceWorldMatrix = {instanceData.worldMatrixRow0,
									instanceData.worldMatrixRow1,
									instanceData.worldMatrixRow2,
									instanceData.worldMatrixRow3};

	VertexShaderOutputLow_DNS_Instanced output;
    output.BaseOutput = VertexShaderFunctionLow_DNS_Base(input, instanceWorldMatrix);
	output.InstanceOutput = VertexShaderInstance_Base(instanceData);
	return output;
}
  */
// Normal, High, Extreme VS

VertexShaderOutput_DNS VertexShaderFunction_DNS_Base(VertexShaderInput_DNS input, float4x4 world)
{
	VertexShaderOutput_DNS output;

	output.BaseOutput = VertexShaderFunctionLow_DNS_Base(input.BaseInput, world);
	
	input.Tangent = UnpackNormal(input.Tangent);
	input.Binormal = UnpackNormal(input.Binormal);
	
    output.TangentToWorld[0] = mul(input.Tangent, (float3x3)world);
    output.TangentToWorld[1] = mul(input.Binormal, (float3x3)world);
    output.TangentToWorld[2] = output.BaseOutput.Normal;

    return output;
}

VertexShaderOutput_DNS VertexShaderFunction_DNS(VertexShaderInput_DNS input)
{
    return VertexShaderFunction_DNS_Base(input, WorldMatrix);
}
/*
VertexShaderOutput_DNS_Instanced VertexShaderFunction_DNS_Instanced(VertexShaderInput_DNS input, VertexShaderInput_Instance instanceData)
{
	float4x4 instanceWorldMatrix = {instanceData.worldMatrixRow0,
									instanceData.worldMatrixRow1,
									instanceData.worldMatrixRow2,
									instanceData.worldMatrixRow3};

	VertexShaderOutput_DNS_Instanced output;

	output.BaseOutput = VertexShaderFunction_DNS_Base(input, instanceWorldMatrix);
	output.InstanceOutput = VertexShaderInstance_Base(instanceData);
	return output;
} */
							  /*
VertexShaderOutput_DNS_Channels VertexShaderFunction_DNS_Channels(VertexShaderInput_DNS_Channels input)
{
	VertexShaderOutput_DNS_Channels output;
	output.Output = VertexShaderFunction_DNS(input.Input);
	output.MaskCoord = input.MaskCoord;
	return output;
}								*/

VertexShaderOutputForward_DNS VertexShaderFunctionLow_DNS_Forward(VertexShaderInputLow_DNS input)
{
	VertexShaderOutputForward_DNS output = (VertexShaderOutputForward_DNS)0;

	float4 worldPos = mul(input.Position, WorldMatrix);
	output.WorldPos = worldPos.xyz;
	float4 viewPos = mul(worldPos, ViewMatrix);
	output.Position = mul(viewPos, ProjectionMatrix);
    output.TexCoord = input.TexCoord;

	// Lighting	
	float4 lightColor = CalculateDynamicLight_Diffuse(worldPos, input.Normal);
	output.LightColor = lightColor;

	return output;
}



float4 PixelShaderFunctionLow_DNS_Forward(VertexShaderOutputForward_DNS input) : COLOR0
{
//return float4(1,1,1,1);
	/*if (IsPixelCut(input.ViewDistance))
	{
		discard;
		return float4(1,1,1,1);
	}*/

	float4 diffuseTexture = tex2D(TextureDiffuseSampler, input.TexCoord);

	float3 diffuse = diffuseTexture.xyz * DiffuseColor.xyz;

	float emissivity = 1 - diffuseTexture.w;

	float4 color = float4(diffuse*AmbientColor + diffuse* (emissivity + input.LightColor /*+ GetSunColor(input.Normal) */ + GetReflectorColor(input.WorldPos)) + Highlight, 1);
	//color = float4(1,1,1,1);
	return color;
}

MyGbufferPixelShaderOutput CalculateOutput(VertexShaderOutputLow_DNS input, float3 normal, float specularIntensity, float3 diffuseColor, float3 si_sp_e, float3 highlight)
{
	//To check normals from vertices
	//normal.xyz = normalize(input.TangentToWorld[2]);    
	//float3 diffusec = GetNormalVectorIntoRenderTarget(normalize(input.TangentToWorld[1]));

	float4 diffuseTexture = tex2D(TextureDiffuseSampler, input.TexCoordAndViewDistance.xy);

	float3 diffuse = diffuseTexture.xyz * diffuseColor.xyz;
	//float fogBlend = (input.TexCoordAndViewDistance.z - FogDistanceNear) / (FogDistanceFar - FogDistanceNear);
	//diffuse = lerp(diffuse, FogColor, saturate(fogBlend) * FogMultiplier);

	//	Output into MRT
	MyGbufferPixelShaderOutput output = GetGbufferPixelShaderOutput(normal.xyz,  diffuse + highlight, 
	specularIntensity * si_sp_e.x / SPECULAR_INTENSITY_RATIO, si_sp_e.y / SPECULAR_POWER_RATIO, input.TexCoordAndViewDistance.z);

	//inverted emissivity, reflection by specular intensity
	output.DepthAndEmissivity.a = PackGBufferEmissivityReflection((1 - diffuseTexture.w) + (si_sp_e.z + length(highlight)), 1.0f);
	return output;
}

// Low PS

MyGbufferPixelShaderOutput PixelShaderFunctionLow_DNS_Base(VertexShaderOutputLow_DNS input, float3 diffuse, float3 si_sp_e, float3 highlight)
{
	return CalculateOutput(input, input.Normal, 1, diffuse, si_sp_e, highlight);
}

MyGbufferPixelShaderOutput PixelShaderFunctionLow_DNS(VertexShaderOutputLow_DNS input)
{
    return PixelShaderFunctionLow_DNS_Base(input, DiffuseColor, float3(SpecularIntensity, SpecularPower, Emissivity), Highlight);
}
															   /*
MyGbufferPixelShaderOutput PixelShaderFunctionLow_DNS_Instanced(VertexShaderOutputLow_DNS_Instanced input)
{
	float3 si_sp_e = input.InstanceOutput.SpecularIntensity_SpecularPower_Emisivity_None.xyz;
	return PixelShaderFunctionLow_DNS_Base(input.BaseOutput, input.InstanceOutput.Diffuse, si_sp_e, input.InstanceOutput.Highlight);
}																 */

// Normal, High, Extreme PS

MyGbufferPixelShaderOutput PixelShaderFunction_DNS_Base(VertexShaderOutput_DNS input, float3 diffuse, float3 si_sp_e, float3 highlight)
{
	
	//bool useParallaxMapping = true;
	if (UseParallaxMapping == true)
	{
		//camera-to-surface vector
		float3 directionToCamera = normalize(CameraPosition - input.BaseOutput.WorldPos);
		float3 halfVector = normalize(directionToCamera);
	
		float height = tex2D(TextureHeightSampler, input.BaseOutput.TexCoordAndViewDistance.xy).r; //TexCoordAndViewDistance
	
		float scale = 0.4f;
		float bias = -0.03f;

		height = height * scale + bias;

		input.BaseOutput.TexCoordAndViewDistance.xy = input.BaseOutput.TexCoordAndViewDistance.xy + (height * halfVector.xy); // Camera.xy
	}
	float4 diffuseTexture = tex2D(TextureDiffuseSampler, input.BaseOutput.TexCoordAndViewDistance.xy);

	input.TangentToWorld[0] = normalize(input.TangentToWorld[0]);
	input.TangentToWorld[1] = normalize(input.TangentToWorld[1]);
	input.TangentToWorld[2] = normalize(input.TangentToWorld[2]);
    
	float4 encodedNormal = tex2D(TextureNormalSampler, input.BaseOutput.TexCoordAndViewDistance.xy);
    float3 normal = GetNormalVectorFromDDS(encodedNormal);
    normal.xyz = normalize(mul(normal.xyz, input.TangentToWorld));    

	//float specularIntensity = encodedNormal.x; //swizzled x and w
	float specularIntensity = encodedNormal.w; //non-swizzled x and w


	return CalculateOutput(input.BaseOutput, normal, specularIntensity, diffuse, si_sp_e, highlight);
}

MyGbufferPixelShaderOutput PixelShaderFunction_DNS(VertexShaderOutput_DNS input)
{
    //Cut pixels from LOD1 which are before LodNear
	/*if (input.BaseOutput.TexCoordAndViewDistance.w < LodCut)
	{
		discard;
		return (MyGbufferPixelShaderOutput)0;
		//return PixelShaderFunction_Base(input, float4(1,0,0,1), Highlight, float3(SpecularIntensity, SpecularPower, 0), renderQuality);
	}
	else*/
	if (IsPixelCut(input.BaseOutput.TexCoordAndViewDistance.w))
	{
		discard;
		return (MyGbufferPixelShaderOutput)0;
	}
	else
	{
		return PixelShaderFunction_DNS_Base(input, DiffuseColor, float3(SpecularIntensity, SpecularPower, Emissivity), Highlight);
	}
}
															/*
MyGbufferPixelShaderOutput PixelShaderFunction_DNS_Instanced(VertexShaderOutput_DNS_Instanced input)
{
	float3 si_sp_e = input.InstanceOutput.SpecularIntensity_SpecularPower_Emisivity_None.xyz;
	return PixelShaderFunction_DNS_Base(input.BaseOutput, input.InstanceOutput.Diffuse, si_sp_e, input.InstanceOutput.Highlight);
}															  */
				 /*
// Rounds normal, return vector where most significant component is 1, others are 0
float3 RoundNormal(float3 normal)
{
	normal = abs(normal);

	float3 normalShift = float3(normal.z, normal.x, normal.y);
	float3 normalStep = step(normal, normalShift); // return 1 where shift is more
	return float3(normalStep.y * (1 - normalStep.x), normalStep.z * (1 - normalStep.y), normalStep.x * (1 - normalStep.z));
}
				 
float4 AddChannels(float4 diffuseAndSpecular, float2 maskCoord, float3 worldPos, float3 normal)
{
	worldPos /= CHANNEL_TEXTURE_SCALE;
	
	normal = RoundNormal(normal);
	float2 chanCoord = normal.x * worldPos.zy + normal.y * worldPos.xz + normal.z * worldPos.xy;
	float4 mask = tex2D(TextureMaskSampler, maskCoord);

	float4 chan0 = float4(0,0,0,1) * (1 - mask.x);
	float4 chan1 = tex2D(TextureChannel1Sampler, chanCoord) * (1 - mask.x);

	chan0.a *= Channel0Intensity;
	chan1.a *= Channel1Intensity;

	// Channel 1 is on the top of channel 0
	float4 blend;
	blend.rgb = lerp(chan0.rgb, chan1.rgb, chan1.a);

	// Combine alpha, example: 0.6 & 0.6 = 0.84
	blend.a = (1 - (1 - chan0.a) * (1 - chan1.a));

	float power = 3;
	float v = saturate((blend.a) + 0.5f - pow(0.5f, power));
	float lerpCoef = pow(v, power);

	float4 res = lerp(diffuseAndSpecular, float4(blend.rgb, diffuseAndSpecular.a * 0.25f), lerpCoef); //we blend to zero spec
	return res;
	
}

MyGbufferPixelShaderOutput PixelShaderFunction_DNS_Channels(VertexShaderOutput_DNS_Channels input)
{
	MyGbufferPixelShaderOutput output = PixelShaderFunction_DNS(input.Output);
	output.DiffuseAndSpecIntensity =  AddChannels(output.DiffuseAndSpecIntensity.rgba * float4(DiffuseColor.xyz, 1) , input.MaskCoord, input.Output.BaseOutput.WorldPos, input.Output.BaseOutput.Normal);
	return output;
}
				   */
MyGbufferPixelShaderOutput CalculateValuesBlended(VertexShaderOutputLow_DNS input, float4 normal)
{
	float4 diffuseTexture = tex2D(TextureDiffuseSampler, input.TexCoordAndViewDistance.xy);
  
	float4 diffuseColor = float4(diffuseTexture.xyz * DiffuseColor.xyz + Highlight, diffuseTexture.a);

	float emissivity = (1 - normal.w) + (Emissivity + length(Highlight));

	//diffuseColor = float4(1,1,0,1);

	//	Output into MRT
	MyGbufferPixelShaderOutput output = GetGbufferPixelShaderOutputBlended(float4(normal.xyz, diffuseColor.a), diffuseColor, emissivity, 1.0f);	
	output.DepthAndEmissivity.a = PackGBufferEmissivityReflection(emissivity, 1.0f);
	return output;
}

MyGbufferPixelShaderOutput PixelShaderFunctionLow_DNS_Blended(VertexShaderOutputLow_DNS input)
{
	float4 normal = GetNormalVectorFromRenderTarget(tex2D(TextureNormalSampler, input.TexCoordAndViewDistance.xy));
	normal.xyz = input.Normal;
	normal.w = 0;
	return CalculateValuesBlended(input, normal);
}


MyGbufferPixelShaderOutput PixelShaderFunction_DNS_Blended(VertexShaderOutput_DNS input)
{
	float4 diffuseTexture = tex2D(TextureDiffuseSampler, input.BaseOutput.TexCoordAndViewDistance.xy);

	input.TangentToWorld[0] = normalize(input.TangentToWorld[0]);
	input.TangentToWorld[1] = normalize(input.TangentToWorld[1]);
	input.TangentToWorld[2] = normalize(input.TangentToWorld[2]);
    
    float4 normal = GetNormalVectorFromRenderTarget(tex2D(TextureNormalSampler, input.BaseOutput.TexCoordAndViewDistance.xy));
    normal.xyz = normalize(mul(normal.xyz, input.TangentToWorld));    
    
	return CalculateValuesBlended(input.BaseOutput, normal);
}

MyGbufferPixelShaderOutput CalculateOutputHolo(VertexShaderOutputLow_DNS input, float4 normal)
{	
	float4 diffuseTexture = tex2D(TextureDiffuseSampler, input.TexCoordAndViewDistance.xy + EmissivityOffset * DiffuseUVAnim);
    float emissivity2 = 1 - tex2D(TextureNormalSampler, input.TexCoordAndViewDistance.xy + EmissivityOffset * EmissivityUVAnim).w;

	float4 diffuseColor = float4(diffuseTexture.xyz * DiffuseColor.xyz + Highlight, diffuseTexture.a);
					 
	float emissivity = emissivity2 * diffuseTexture.a + (length(Highlight));
	
	//diffuseColor = float4(1,0,1,1);

	//	Output into MRT
	MyGbufferPixelShaderOutput output = GetGbufferPixelShaderOutputBlended(float4(normal.xyz, diffuseColor.a), diffuseColor, emissivity, 1.0f);
	return output; 
}


float4 PixelShaderFunction_Holo_Forward(VertexShaderOutputForward_DNS input) : COLOR
{
	float4 diffuseTexture = tex2D(TextureDiffuseSampler, input.TexCoord);
    
	float4 diffuseColor = float4(diffuseTexture.xyz * DiffuseColor.xyz + Highlight, diffuseTexture.a);
					 
	float emissivity = 0.5f;

	float4 color = diffuseTexture;

	return color; 
}

MyGbufferPixelShaderOutput PixelShaderFunction_Holo(VertexShaderOutput_DNS input)
{
	float2 texCoord = GetScreenSpaceTextureCoord(input.BaseOutput.ScreenPosition, HalfPixel) * Scale;
	/*float nearDepth = DecodeFloatRGBA(tex2D(DepthTextureNearSampler, texCoord));
	float farDepth = DecodeFloatRGBA(tex2D(DepthTextureFarSampler, texCoord));

	float depth = min(nearDepth, farDepth) * FAR_PLANE_DISTANCE;
	if (depth + 0.01f < input.BaseOutput.TexCoordAndViewDistance.z)
		discard;*/

	float4 diffuseTexture = tex2D(TextureDiffuseSampler, input.BaseOutput.TexCoordAndViewDistance.xy);

	
	input.TangentToWorld[0] = normalize(input.TangentToWorld[0]);
	input.TangentToWorld[1] = normalize(input.TangentToWorld[1]);
	input.TangentToWorld[2] = normalize(input.TangentToWorld[2]);
    
    float4 normal = GetNormalVectorFromRenderTarget(tex2D(TextureNormalSampler, input.BaseOutput.TexCoordAndViewDistance.xy));
    normal.xyz = normalize(mul(normal.xyz, input.TangentToWorld));    
    							  
	return CalculateOutputHolo(input.BaseOutput, normal);
}

MyGbufferPixelShaderOutput PixelShaderFunction_Holo_IgnoreDepth(VertexShaderOutput_DNS input)
{
	float4 diffuseTexture = tex2D(TextureDiffuseSampler, input.BaseOutput.TexCoordAndViewDistance.xy);

	input.TangentToWorld[0] = normalize(input.TangentToWorld[0]);
	input.TangentToWorld[1] = normalize(input.TangentToWorld[1]);
	input.TangentToWorld[2] = normalize(input.TangentToWorld[2]);
    
    float4 normal = GetNormalVectorFromRenderTarget(tex2D(TextureNormalSampler, input.BaseOutput.TexCoordAndViewDistance.xy));
    normal.xyz = normalize(mul(normal.xyz, input.TangentToWorld));    
    							  
	return CalculateOutputHolo(input.BaseOutput, normal);
}


MyGbufferPixelShaderOutput PixelShaderFunction_DNS_Masked(VertexShaderOutput_DNS input)
{
	float4 diffuseTexture = tex2D(TextureDiffuseSampler, input.BaseOutput.TexCoordAndViewDistance.xy);

	if (diffuseTexture.a == 0)
		discard;

    return PixelShaderFunction_DNS(input);
}

float4 PixelShaderFunction_DNS_LowMasked(VertexShaderOutputForward_DNS input) : COLOR
{
	float4 diffuseTexture = tex2D(TextureDiffuseSampler, input.TexCoord);
    
	if (diffuseTexture.a == 0)
		discard;

	float4 color = diffuseTexture;

	return color; 
}

MyGbufferPixelShaderOutput PixelShaderFunction_Stencil(VertexShaderOutput_DNS input)
{
	return GetGbufferPixelShaderOutput(float3(0,0,0), float3(0,0,0), input.BaseOutput.TexCoordAndViewDistance.z);
}

MyGbufferPixelShaderOutput PixelShaderFunction_Stencil_Low(VertexShaderOutputLow_DNS input)
{
	return GetGbufferPixelShaderOutput(float3(0,0,0), float3(0,0,0), input.TexCoordAndViewDistance.z);
}
																/*
MyGbufferPixelShaderOutput PixelShaderFunction_Stencil_Instanced(VertexShaderOutput_DNS_Instanced input)
{
	return GetGbufferPixelShaderOutput(float3(0,0,0), float3(0,0,0), input.BaseOutput.BaseOutput.TexCoordAndViewDistance.z);
}

MyGbufferPixelShaderOutput PixelShaderFunction_Stencil_Low_Instanced(VertexShaderOutputLow_DNS_Instanced input)
{
	return GetGbufferPixelShaderOutput(float3(0,0,0), float3(0,0,0), input.BaseOutput.TexCoordAndViewDistance.z);
}																  */

technique Technique_RenderQualityLow_Forward
{
	pass Pass1
	{
		MinFilter[0] = LINEAR; 
		MagFilter[0] = LINEAR; 

		MinFilter[1] = LINEAR; 
		MagFilter[1] = LINEAR; 

		MinFilter[2] = LINEAR; 
		MagFilter[2] = LINEAR; 

		VertexShader = compile vs_2_0 VertexShaderFunctionLow_DNS_Forward();
		PixelShader = compile ps_2_0 PixelShaderFunctionLow_DNS_Forward();
	}
}

technique Technique_RenderQualityLow
{
    pass Pass1
    {
        MinFilter[0] = LINEAR; 
        MagFilter[0] = LINEAR; 

        MinFilter[1] = LINEAR; 
        MagFilter[1] = LINEAR; 

        MinFilter[2] = LINEAR; 
        MagFilter[2] = LINEAR; 

        VertexShader = compile vs_3_0 VertexShaderFunctionLow_DNS();
        PixelShader = compile ps_3_0 PixelShaderFunctionLow_DNS();
    }
}

technique Technique_RenderQualityNormal
{
    pass Pass1
    {
        MinFilter[0] = LINEAR; 
        MagFilter[0] = LINEAR; 

        MinFilter[1] = LINEAR; 
        MagFilter[1] = LINEAR; 

        MinFilter[2] = LINEAR; 
        MagFilter[2] = LINEAR; 

        VertexShader = compile vs_3_0 VertexShaderFunction_DNS();
        PixelShader = compile ps_3_0 PixelShaderFunction_DNS();
    }
}

technique Technique_RenderQualityHigh
{
    pass Pass1
    {
        MinFilter[0] = LINEAR; 
        MagFilter[0] = LINEAR; 

        MinFilter[1] = LINEAR; 
        MagFilter[1] = LINEAR; 

        MinFilter[2] = LINEAR; 
        MagFilter[2] = LINEAR; 
		
        VertexShader = compile vs_3_0 VertexShaderFunction_DNS();
        PixelShader = compile ps_3_0 PixelShaderFunction_DNS();
    }
}

technique Technique_RenderQualityExtreme
{
    pass Pass1
    {
        MinFilter[0] = ANISOTROPIC; 
        MagFilter[0] = ANISOTROPIC; 
        MaxAnisotropy[0] = 16;

        MinFilter[1] = ANISOTROPIC; 
        MagFilter[1] = ANISOTROPIC; 
        MaxAnisotropy[1] = 16;

        MinFilter[2] = ANISOTROPIC; 
        MagFilter[2] = ANISOTROPIC; 
        MaxAnisotropy[2] = 16;

        VertexShader = compile vs_3_0 VertexShaderFunction_DNS();
        PixelShader = compile ps_3_0 PixelShaderFunction_DNS();
    }
}

technique Technique_Holo
{
    pass Pass1
    {
        MinFilter[0] = LINEAR; 
        MagFilter[0] = LINEAR; 

        MinFilter[1] = LINEAR; 
        MagFilter[1] = LINEAR; 

        MinFilter[2] = LINEAR; 
        MagFilter[2] = LINEAR; 

        VertexShader = compile vs_3_0 VertexShaderFunction_DNS();
        PixelShader = compile ps_3_0 PixelShaderFunction_Holo();
    }
}

technique Technique_Holo_IgnoreDepth
{
    pass Pass1
    {
        MinFilter[0] = LINEAR; 
        MagFilter[0] = LINEAR; 

        MinFilter[1] = LINEAR; 
        MagFilter[1] = LINEAR; 

        MinFilter[2] = LINEAR; 
        MagFilter[2] = LINEAR; 

        VertexShader = compile vs_3_0 VertexShaderFunction_DNS();
        PixelShader = compile ps_3_0 PixelShaderFunction_Holo_IgnoreDepth();
    }
}

technique Technique_HoloForward
{
    pass Pass1
    {
        MinFilter[0] = LINEAR; 
        MagFilter[0] = LINEAR; 

        MinFilter[1] = LINEAR; 
        MagFilter[1] = LINEAR; 

        MinFilter[2] = LINEAR; 
        MagFilter[2] = LINEAR; 
        
        VertexShader = compile vs_2_0 VertexShaderFunctionLow_DNS_Forward();
        PixelShader = compile ps_2_0 PixelShaderFunction_Holo_Forward();
    }
}
technique Technique_Stencil
{
    pass Pass1
    {
        MinFilter[0] = LINEAR; 
        MagFilter[0] = LINEAR; 

        MinFilter[1] = LINEAR; 
        MagFilter[1] = LINEAR; 

        MinFilter[2] = LINEAR; 
        MagFilter[2] = LINEAR; 

        VertexShader = compile vs_3_0 VertexShaderFunction_DNS();
        PixelShader = compile ps_3_0 PixelShaderFunction_Stencil();
    }
}
			 	
technique Technique_StencilLow
{
    pass Pass1
    {
        MinFilter[0] = LINEAR; 
        MagFilter[0] = LINEAR; 

        MinFilter[1] = LINEAR; 
        MagFilter[1] = LINEAR; 

        MinFilter[2] = LINEAR; 
        MagFilter[2] = LINEAR; 
        
        VertexShader = compile vs_3_0 VertexShaderFunctionLow_DNS();
        PixelShader = compile ps_3_0 PixelShaderFunction_Stencil_Low();
    }
}	 	
technique Technique_RenderQualityLowBlended_Forward
{
	pass Pass1
	{
		MinFilter[0] = LINEAR; 
		MagFilter[0] = LINEAR; 

		MinFilter[1] = LINEAR; 
		MagFilter[1] = LINEAR; 

		MinFilter[2] = LINEAR; 
		MagFilter[2] = LINEAR; 

		VertexShader = compile vs_3_0 VertexShaderFunctionLow_DNS_Forward();
		PixelShader = compile ps_3_0 PixelShaderFunctionLow_DNS_Forward();
	}
}

technique Technique_RenderQualityNormalBlended
{
    pass Pass1
    {
        MinFilter[0] = LINEAR; 
        MagFilter[0] = LINEAR; 

        MinFilter[1] = LINEAR; 
        MagFilter[1] = LINEAR; 

        MinFilter[2] = LINEAR; 
        MagFilter[2] = LINEAR; 

        VertexShader = compile vs_3_0 VertexShaderFunction_DNS();
        PixelShader = compile ps_3_0 PixelShaderFunction_DNS_Blended();
    }
}

technique Technique_RenderQualityHighBlended
{
    pass Pass1
    {
        MinFilter[0] = LINEAR; 
        MagFilter[0] = LINEAR; 

        MinFilter[1] = LINEAR; 
        MagFilter[1] = LINEAR; 

        MinFilter[2] = LINEAR; 
        MagFilter[2] = LINEAR; 

        VertexShader = compile vs_3_0 VertexShaderFunction_DNS();
        PixelShader = compile ps_3_0 PixelShaderFunction_DNS_Blended();
    }
}

technique Technique_RenderQualityExtremeBlended
{
    pass Pass1
    {
        MinFilter[0] = ANISOTROPIC; 
        MagFilter[0] = ANISOTROPIC; 
        MaxAnisotropy[0] = 16;

        MinFilter[1] = ANISOTROPIC; 
        MagFilter[1] = ANISOTROPIC; 
        MaxAnisotropy[1] = 16;

        MinFilter[2] = ANISOTROPIC; 
        MagFilter[2] = ANISOTROPIC; 
        MaxAnisotropy[2] = 16;

        VertexShader = compile vs_3_0 VertexShaderFunction_DNS();
        PixelShader = compile ps_3_0 PixelShaderFunction_DNS_Blended();
    }
}


technique Technique_RenderQualityLowMasked
{
    pass Pass1
    {
        MinFilter[0] = LINEAR; 
        MagFilter[0] = LINEAR; 

        MinFilter[1] = LINEAR; 
        MagFilter[1] = LINEAR; 

        MinFilter[2] = LINEAR; 
        MagFilter[2] = LINEAR; 

        VertexShader = compile vs_3_0 VertexShaderFunctionLow_DNS();
        PixelShader = compile ps_3_0 PixelShaderFunction_DNS_LowMasked();
    }
}

technique Technique_RenderQualityNormalMasked
{
    pass Pass1
    {
        MinFilter[0] = LINEAR; 
        MagFilter[0] = LINEAR; 

        MinFilter[1] = LINEAR; 
        MagFilter[1] = LINEAR; 

        MinFilter[2] = LINEAR; 
        MagFilter[2] = LINEAR; 

        VertexShader = compile vs_3_0 VertexShaderFunction_DNS();
        PixelShader = compile ps_3_0 PixelShaderFunction_DNS_Masked();
    }
}

technique Technique_RenderQualityHighMasked
{
    pass Pass1
    {
        MinFilter[0] = LINEAR; 
        MagFilter[0] = LINEAR; 

        MinFilter[1] = LINEAR; 
        MagFilter[1] = LINEAR; 

        MinFilter[2] = LINEAR; 
        MagFilter[2] = LINEAR; 

        VertexShader = compile vs_3_0 VertexShaderFunction_DNS();
        PixelShader = compile ps_3_0 PixelShaderFunction_DNS_Masked();
    }
}

technique Technique_RenderQualityExtremeMasked
{
    pass Pass1
    {
        MinFilter[0] = ANISOTROPIC; 
        MagFilter[0] = ANISOTROPIC; 
        MaxAnisotropy[0] = 16;

        MinFilter[1] = ANISOTROPIC; 
        MagFilter[1] = ANISOTROPIC; 
        MaxAnisotropy[1] = 16;

        MinFilter[2] = ANISOTROPIC; 
        MagFilter[2] = ANISOTROPIC; 
        MaxAnisotropy[2] = 16;

        VertexShader = compile vs_3_0 VertexShaderFunction_DNS();
        PixelShader = compile ps_3_0 PixelShaderFunction_DNS_Masked();
    }
}
		