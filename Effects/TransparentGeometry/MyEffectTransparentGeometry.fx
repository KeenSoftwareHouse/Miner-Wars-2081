#include "../MyEffectDynamicLightingBase.fxh"
#include "../MyEffectReflectorBase.fxh"

//	Renders transparent quads (billboards, polyline...) through forward renderer, therefore needs to 
//	handle lights, shadows, fog... we can't use deferred rendering here.

float2		HalfPixel;
float2		Scale;
float4x4	WorldMatrix;
float4x4	ViewMatrix;
float4x4	ProjectionMatrix;
float		SoftParticleDistanceScale;

float4x4	WorldViewMatrix;
float4x4	WorldViewProjectionMatrix;

float4		ColorizeColor;
float3		ColorizePlaneNormal;
float		ColorizePlaneDistance;
float		ColorizeSoftDistance;

float2		AlphaMultiplierSaturation = float2(1,1);

Texture BillboardTexture;
sampler BillboardTextureSampler = sampler_state 
{ 
	texture = <BillboardTexture> ; 
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
	AddressU = wrap; 
	AddressV = wrap;
};

Texture BillboardBlendTexture;
sampler BillboardBlendTextureSampler = sampler_state 
{ 
	texture = <BillboardBlendTexture> ; 
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
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
	float4 Color : NORMAL0;
	float4 TexCoord : TEXCOORD0;
	float2 TexCoord2 : TEXCOORD1; //blend tex coords
};


struct VertexShaderUnlitOutput
{
	float4 Position : POSITION0;
	float4 Color : NORMAL0;						//	Color must be passed as NORMAL0 (and not COLOR0), because we need to pass values higher than 1, and COLOR0 was clamping them to <0..1>
	float4 TexCoord: TEXCOORD0;				//	In XY we pass texture coordinate, in Z texture blend 
	float3 WorldPosition : TEXCOORD1;
	float4 ViewPosition : TEXCOORD2;
	float4 ProjectedPosition: TEXCOORD3;
	float2 TexCoord2: TEXCOORD4;
};

struct VertexShaderLitOutput
{
	VertexShaderUnlitOutput UnlitOutput;
	float2 ShadowColorAndReflectorAtt : TEXCOORD5;
	float3 LightColor : TEXCOORD6;
};


VertexShaderUnlitOutput VertexShaderFunctionUnlit(VertexShaderInput input)
{
	VertexShaderUnlitOutput output;
	
	output.Position = mul(input.Position, WorldMatrix);
	output.WorldPosition = output.Position;
	output.ViewPosition = mul(output.Position, ViewMatrix);
	output.Position = mul(output.ViewPosition, ProjectionMatrix);
	//output.ViewPosition = mul(input.Position, WorldViewMatrix);    
	//output.Position = mul(input.Position, WorldViewProjectionMatrix);    
	output.ProjectedPosition = output.Position;
	output.TexCoord = input.TexCoord;
	output.TexCoord2 = input.TexCoord2;  
	output.Color = input.Color;

	return output;
}


VertexShaderLitOutput VertexShaderFunctionLit(VertexShaderInput input)
{
	VertexShaderUnlitOutput output = VertexShaderFunctionUnlit(input);
	VertexShaderLitOutput litOutput;
	litOutput.UnlitOutput = output;

	float diff = 0;
#ifdef COLOR_SHADOW_MAP_FORMAT
	litOutput.ShadowColorAndReflectorAtt.x = 1.0f; 
#else
	litOutput.ShadowColorAndReflectorAtt.x = GetShadowTermFromPosition(float4(output.ViewPosition.xyz, 1), output.ViewPosition.z, 0, 0, diff);
#endif

	float distanceToReflector = GetDistanceToReflector(litOutput.UnlitOutput.WorldPosition);
	float3 directionToReflector = GetDirectionToReflector(litOutput.UnlitOutput.WorldPosition);
	float distanceToReflectorInverted = 1 - saturate(distanceToReflector / ReflectorRange);

	
	//	Reflector - attenuation
	litOutput.ShadowColorAndReflectorAtt.y = GetReflectorAttenuation(distanceToReflectorInverted, normalize(directionToReflector));       
	//litOutput.ShadowColorAndReflectorAtt.y = 0;       

	litOutput.LightColor = float3(0,0,0);

	for (int i = 0; i < DynamicLightsCount; i++)
	{
		litOutput.LightColor += CalculateDynamicLightForParticle(DynamicLights[i], output.WorldPosition);
	}

	//litOutput.LightColor = saturate(litOutput.LightColor);

	return litOutput;
}

float4 SaturateAlpha(float4 resultColor, float alpha)
{
	if (AlphaMultiplierSaturation.y < 1)
	{
		float invSat = 1 - AlphaMultiplierSaturation.y;
		float alphaSaturate = clamp(alpha - invSat, 0, 1) / AlphaMultiplierSaturation.y;
		resultColor += float4(1,1,1,1) * float4(alphaSaturate.xxx, 0) * alpha * AlphaMultiplierSaturation.y;// *  *
	}
	return resultColor;
}

float4 calculateColor( VertexShaderUnlitOutput input, bool depthTest )
{
	//	Conversion of 4D position into 'screen space texture coord' must be calculated in pixel shader, because W is 
	//	not linear and it would give wrong results if calculated in vertex shader and interpolated into pixel shader.
	//	IMPORTANT: HalfPixel is calculated from depth render target size, not from regular screen
	float2 screenSpaceTexCoord = GetScreenSpaceTextureCoord(input.ProjectedPosition, HalfPixel) * Scale;
	
	float softParticleFade = 1;

	if (depthTest)
	{
		//	Pixel distance must be calculated per-pixel (not in vertex shader and then interpolated) because
		//	otherwise we won't get good distance due to perspective projection
		//	This is a specific feature of camera facing billboards
		float viewDistance = length(input.ViewPosition.xyz);
		float depthBufferDistance = GetViewDistanceFromDepth( DecodeFloatRGBA(tex2D(DepthsRTSampler, screenSpaceTexCoord)), input.ViewPosition.xyz);    

		
		if (SoftParticleDistanceScale >= 10000)
		{
			viewDistance -= 2;
		}


		//	This is just an optimization, based on distance. Don't do it on "alpha <= 0", because now we use
		//	pre-multiplied alpha and we render particles even if their alpha is zero.
		if (viewDistance > depthBufferDistance)
		{	
			discard;
			return float4(1,1,1,1);
		}

		softParticleFade = saturate(SoftParticleDistanceScale * (depthBufferDistance - viewDistance));			
	}


	float4 resultColor = tex2D(BillboardTextureSampler, input.TexCoord.xy);
	float alpha = resultColor.x*resultColor.y*resultColor.z;
	if (input.TexCoord.z > 0)
	{
		float4 blendColor = tex2D(BillboardBlendTextureSampler, input.TexCoord2.xy);
		resultColor = lerp(resultColor, blendColor, input.TexCoord.z);
	}
		
	resultColor = saturate (resultColor * input.Color * AlphaMultiplierSaturation.x);            
	
	resultColor += 100 * input.TexCoord.w * resultColor; 

	resultColor = SaturateAlpha(resultColor, alpha);

	resultColor *= softParticleFade;	

	return resultColor;
}

float4 calculateColorIgnoreDepth( VertexShaderUnlitOutput input )
{
	//	Conversion of 4D position into 'screen space texture coord' must be calculated in pixel shader, because W is 
	//	not linear and it would give wrong results if calculated in vertex shader and interpolated into pixel shader.
	//	IMPORTANT: HalfPixel is calculated from depth render target size, not from regular screen
	float2 screenSpaceTexCoord = GetScreenSpaceTextureCoord(input.ProjectedPosition, HalfPixel) * Scale;
	float4 resultColor = tex2D(BillboardTextureSampler, input.TexCoord.xy);
	float alpha = resultColor.x * resultColor.y * resultColor.z;
	return SaturateAlpha(resultColor * input.Color, alpha);
}

float4 PixelShaderFunctionColorizeHeight(VertexShaderUnlitOutput input) : COLOR0
{	
	float4 color = calculateColorIgnoreDepth(input);

	float dist = dot(input.WorldPosition, normalize(ColorizePlaneNormal)) + ColorizePlaneDistance;
	float softDist = ColorizeSoftDistance;
	return color - color * ColorizeColor * saturate((-dist) / softDist);
	//return color * ColorizeColor * saturate((ColorizeSoftDistance - dist) / ColorizeSoftDistance);
}

float4 PixelShaderFunctionUnlit(VertexShaderUnlitOutput input) : COLOR0
{	
	return calculateColor( input, true );
}

float4 PixelShaderFunctionUnlit_Forward(VertexShaderUnlitOutput input) : COLOR0
{	
	return calculateColor( input, false);
}

float4 PixelShaderFunctionLit(VertexShaderLitOutput input) : COLOR0
{	
	float4 resultColor = calculateColor( input.UnlitOutput, true );
	//If this particle can be influenced by light
	resultColor.xyz = resultColor.xyz * (input.ShadowColorAndReflectorAtt.x + input.ShadowColorAndReflectorAtt.y * ReflectorColor + input.LightColor);
	
	/*resultColor.xyz = resultColor.xyz * (input.ShadowColorAndReflectorAtt.x) + input.LightColor * resultColor.w;
	
	float distanceToReflector = GetDistanceToReflector(input.UnlitOutput.WorldPosition);
	float3 directionToReflector = GetDirectionToReflector(input.UnlitOutput.WorldPosition);
	float distanceToReflectorInverted = 1 - saturate(distanceToReflector / ReflectorRange);

	float reflectorAtt = GetReflectorAttenuation(distanceToReflectorInverted, normalize(directionToReflector));       

	resultColor.xyz += reflectorAtt * ReflectorColor * resultColor.w;
	*/

	return resultColor;
}

float4 PixelShaderFunctionIgnoreDepth(VertexShaderUnlitOutput input) : COLOR0
{	
	return calculateColorIgnoreDepth( input );
}

float4 PixelShaderFunctionVisualizeOverdraw(VertexShaderUnlitOutput input) : COLOR0
{	
	return float4(1,0,0,1);
}


technique Technique_UnlitBasic
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 VertexShaderFunctionUnlit();
		PixelShader = compile ps_3_0 PixelShaderFunctionUnlit();
	}
}

technique Technique_UnlitBasic_Forward
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 VertexShaderFunctionUnlit();
		PixelShader = compile ps_3_0 PixelShaderFunctionUnlit_Forward();
	}
}

technique Technique_LitBasic
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 VertexShaderFunctionLit();
		PixelShader = compile ps_3_0 PixelShaderFunctionLit();
	}
}

technique Technique_IgnoreDepthBasic
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 VertexShaderFunctionUnlit();
		PixelShader = compile ps_3_0 PixelShaderFunctionIgnoreDepth();
	}
}

technique Technique_ColorizeHeight
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 VertexShaderFunctionUnlit();
		PixelShader = compile ps_3_0 PixelShaderFunctionColorizeHeight();
	}
}

technique Technique_VisualizeOverdraw
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 VertexShaderFunctionUnlit();
		PixelShader = compile ps_3_0 PixelShaderFunctionVisualizeOverdraw();
	}
}