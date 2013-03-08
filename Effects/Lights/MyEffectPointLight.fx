#include "../MyEffectSpotShadowBase.fxh"
#include "../MyEffectReflectorBase.fxh"

#define SHADOW_FILTERING 3

float2 HalfPixel;
float2 Scale;

float3 LightPosition;
float LightRadius;

float3 LightColor;
float LightIntensity;

float3 LightSpecularColor;
float Falloff;
float NearSlopeBiasDistance;


float4x4 LightViewProjection;
float4x4 WorldViewProjMatrix;
float4x4 ViewProjMatrix;
float4x4 WorldMatrix;
float4x4 ViewMatrix;

Texture NormalsRT;
sampler NormalsRTSampler = sampler_state 
{ 
	texture = <NormalsRT> ; 
	magfilter = POINT; 
	minfilter = POINT; 
	mipfilter = NONE; 
	AddressU = clamp; 
	AddressV = clamp;
};

Texture DiffuseRT;
sampler DiffuseRTSampler = sampler_state 
{ 
	texture = <DiffuseRT> ; 
	magfilter = POINT; 
	minfilter = POINT; 
	mipfilter = NONE; 
	AddressU = clamp; 
	AddressV = clamp;
};

Texture DepthsRT;
sampler DepthsRTSampler = sampler_state 
{ 
	texture = <DepthsRT> ; 
	magfilter = POINT; 
	minfilter = POINT; 
	mipfilter = NONE; 
	AddressU = clamp; 
	AddressV = clamp;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
};


struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float4 ScreenPosition : TEXCOORD0;
	float4 WorldPosition : TEXCOORD1;
	float4 ViewPosition : TEXCOORD2;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = mul(input.Position, WorldViewProjMatrix);
	output.ScreenPosition = output.Position;
	output.WorldPosition = mul(input.Position, WorldMatrix);
	output.ViewPosition = mul(output.WorldPosition, ViewMatrix);
    return output;
}

struct CalculatedWorldValues
{
    float3 Position;
	float3 ViewPosition;
    float3 ViewDir;
	float2 TexCoord;
};

struct CalculatedColorValues
{
	float3 Normal;
	float3 Specular;
	float4 Diffuse;
	float NdL;
};

//	Calculate the intensity of the light with exponential falloff
float GetDynamicLightBaseIntensity(float distance, float range, float falloff)
{
	return 1 - pow(saturate(distance / range), falloff);
}

//calc, not load
void LoadWorldValues(VertexShaderOutput input, out CalculatedWorldValues values)
{
	values.TexCoord = GetScreenSpaceTextureCoord(input.ScreenPosition, HalfPixel) * Scale;

	float4 encodedDepth = tex2D(DepthsRTSampler, values.TexCoord);
	float depthNormalized = DecodeFloatRGBA(encodedDepth);
	
	//VS
	values.ViewDir = input.ViewPosition.xyz / input.ViewPosition.w;	
	values.ViewPosition = GetViewPositionFromDepth(depthNormalized, values.ViewDir);

	float4 wPosition = mul(float4(values.ViewPosition,1), InvViewMatrix);
	wPosition.xyz = wPosition.xyz / wPosition.w;
	values.Position = wPosition.xyz;


}

float4 CalculateColorValues(CalculatedWorldValues worldValues, out CalculatedColorValues colorValues, float3 lightVector, float radius, float3 lightColor, float3 specularColor, float intensity, float falloff): COLOR0
{
	float4 normal = tex2D(NormalsRTSampler, worldValues.TexCoord);
	float specularPower = normal.a * SPECULAR_POWER_RATIO;


//blend?
	normal.xyz = GetNormalVectorFromRenderTargetNormalized(normal.xyz);
	colorValues.Normal = normal;

    colorValues.Diffuse = tex2D(DiffuseRTSampler, worldValues.TexCoord);

	float specularIntensity = colorValues.Diffuse.a * SPECULAR_INTENSITY_RATIO;

	float attenuation = GetDynamicLightBaseIntensity(length(lightVector), radius, falloff); 

	//if att > 0, int > 0

	lightVector = normalize(lightVector);

	//reflection vector
    float3 reflectionVector = normalize(reflect(-lightVector, normal));

    //camera-to-surface vector
    float3 directionToCamera = normalize(CameraPosition - worldValues.Position);

    //compute specular light
    float specularLight = specularIntensity * pow( saturate(dot(reflectionVector, directionToCamera)), specularPower);

	colorValues.NdL = max(0,dot(normal,lightVector));

    float3 diffuseLight = colorValues.NdL * lightColor * colorValues.Diffuse.rgb;
	colorValues.Specular = specularLight.xxx * lerp(float3(1,1,1), colorValues.Diffuse.rgb, 0.5);

	return float4(attenuation * intensity * (diffuseLight + colorValues.Specular), 1);
}


//what is it for?
half4 CalculateShadow(float3 viewPosition, float NdL, uniform int filterSize, float nearSlopeBiasDistance)
{
	// Spot shadows
	half length0 = length(viewPosition.xyz);
	half bias = 1 - NdL;
	//??
	if (length0 < nearSlopeBiasDistance) //cockpit
		bias = 4.0f;
	return half4(GetShadowTermFromPosition(float4(viewPosition, 1), -length0, SHADOW_FILTERING, bias), 1);
}

float4 CalculatePointLighting(VertexShaderOutput input): COLOR0
{
	CalculatedWorldValues values;
	LoadWorldValues(input, values);

	float3 lightVector = LightPosition - values.Position;
	//test if lit pixel is too far from light
    if (length(lightVector) > LightRadius)
    {
		discard;
		// Adding return could save us precious processing power
		// http://www.jb101.co.uk/2008/02/14/hlsl-discard-doesnt-return/
		return float4(0,0,0,1);
    }//else

	CalculatedColorValues colorValues;
	return CalculateColorValues(values, colorValues, lightVector, LightRadius, LightColor, LightSpecularColor, LightIntensity, Falloff);
}

//duplicity
float4 CalculateReflectorLighting(VertexShaderOutput input): COLOR0
{
	CalculatedWorldValues values;
	LoadWorldValues(input, values);

	float3 lightVector = LightPosition - values.Position;
	//test if lit pixel is too far from light
    if (length(lightVector) > LightRadius)
    {
		discard;
		// Adding return could save us precious processing power
		// http://www.jb101.co.uk/2008/02/14/hlsl-discard-doesnt-return/
		return float4(0,0,0,1);
    }//else

	CalculatedColorValues colorValues;
	float4 color = CalculateColorValues(values, colorValues, lightVector, LightRadius, LightColor, LightSpecularColor, LightIntensity, Falloff);

	float3 directionToReflector = -values.ViewDir;
	 //	Reflector - diffuse
    float reflectorDiffuseMultiplier = saturate(dot(colorValues.Normal, directionToReflector));
   
   	float distanceToReflector = GetDistanceToReflector(values.Position);
	float distanceToReflectorInverted = 1 - saturate(distanceToReflector / ReflectorRange);

	//	Reflector - attenuation
    float4 reflectorAttenuation = GetReflectorAttenuation(distanceToReflectorInverted, directionToReflector);       

    //take into account attenuation and lightIntensity.
	float4 colorRefl = float4(reflectorAttenuation * (ReflectorColor * reflectorDiffuseMultiplier * colorValues.Diffuse.xyz + colorValues.Specular), 1);

	return color + colorRefl;
}

float4 CalculateSpotLighting(VertexShaderOutput input, uniform int enableShadows): COLOR0
{
	CalculatedWorldValues values;
	LoadWorldValues(input, values);

	float3 lightVector = LightPosition - values.Position;
	//test if lit pixel is too far from light
    if (length(lightVector) > ReflectorRange)
    {
		discard;
		// Adding return could save us precious processing power
		// http://www.jb101.co.uk/2008/02/14/hlsl-discard-doesnt-return/
		return float4(0,0,0,1);
    }

	float SdL = dot(normalize(ReflectorDirection), normalize(-lightVector));
	if(SdL < ReflectorConeMaxAngleCos)
	{
		discard;
		// Adding return could save us precious processing power
		// http://www.jb101.co.uk/2008/02/14/hlsl-discard-doesnt-return/
		return float4(0.0f,0.0f,0,1);
	}

	CalculatedColorValues colorValues;
	float4 color = CalculateColorValues(values, colorValues, lightVector, ReflectorRange, ReflectorColor, LightSpecularColor, ReflectorIntensity, ReflectorFalloff);

	if(enableShadows)
	{
		half4 shadow = CalculateShadow(values.ViewPosition, colorValues.NdL, SHADOW_FILTERING, NearSlopeBiasDistance);
		color *= shadow;
	}

	// This makes light fade out when no texture set
	float spotIntensity = lerp((SdL - ReflectorConeMaxAngleCos) / (1 - ReflectorConeMaxAngleCos), 1.0f, 0);
	color.xyz *= spotIntensity;

	return color;
}


float4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float nearPlaneDistance, float farPlaneDistance)
{
	float4x4 m;

	m._m00 = (2 * nearPlaneDistance) / (right - left);
	m._m01_m02_m03 = 0;
	m._m11 = (2 * nearPlaneDistance) / (top - bottom);
	m._m10_m12_m13 = 0;
	m._m20 = (left + right) / (right - left);
	m._m21 = (top + bottom) / (top - bottom);
	m._m22 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
	m._m23 = -1;
	m._m32 = (nearPlaneDistance * farPlaneDistance) / (nearPlaneDistance - farPlaneDistance);
	m._m30_m31_m33 = 0;

	return m;
}


float4 PSLighting(VertexShaderOutput input) : COLOR0
{
	return CalculatePointLighting(input);
}

float4 PSLightingWithReflector(VertexShaderOutput input) : COLOR0
{
	return CalculateReflectorLighting(input);
}

float4 PSLightingSpot(VertexShaderOutput input, uniform int enableShadows) : COLOR0
{
	return CalculateSpotLighting(input, enableShadows);
}

// No HDR techniques
technique Technique_Lighting //PointLight
{
    pass Pass1
    {
		VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PSLighting();
    }
}


technique Technique_Spot
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PSLightingSpot(0);
	}
}

technique Technique_SpotShadows
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PSLightingSpot(1);
	}
}

technique Technique_LightingWithReflector
{
    pass Pass1
    {
		VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PSLightingWithReflector();
    }
}