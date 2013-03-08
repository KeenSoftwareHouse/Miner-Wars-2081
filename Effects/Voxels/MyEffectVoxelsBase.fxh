//#include "../MyEffectBase.fxh"
#include "../MyEffectDynamicLightingBase.fxh"
#include "../MyEffectReflectorBase.fxh"

//	This header implements all triplanar vertex/pixel shader logic used by dedicated model shader and voxel shader


//	Voxel 'texture scale by distance' trick (this is not about LOD at all)
const float TEXTURE_SCALE_NEAR = 100;
const float TEXTURE_SCALE_FAR = 1000;

int EnablePerVertexAmbient = 1;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//	Material
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Specular properties for material 1
float		SpecularIntensity;
float		SpecularPower;

// Specular properties for material 2
float		SpecularIntensity2;
float		SpecularPower2;

// Specular properties for material 3
float		SpecularIntensity3;
float		SpecularPower3;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//	Textures for different axis - common for single-material or multi-material voxels
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

Texture TextureDiffuseForAxisXZ;
Texture TextureDiffuseForAxisY;
Texture TextureNormalMapForAxisXZ;
Texture TextureNormalMapForAxisY;

Texture TextureDiffuseForAxisXZ2;
Texture TextureDiffuseForAxisY2;
Texture TextureNormalMapForAxisXZ2;
Texture TextureNormalMapForAxisY2;

Texture TextureDiffuseForAxisXZ3;
Texture TextureDiffuseForAxisY3;
Texture TextureNormalMapForAxisXZ3;
Texture TextureNormalMapForAxisY3;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//	Texture samplers
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

sampler TextureDiffuseForAxisXZSampler[3] =
{
	sampler_state
	{
		texture = <TextureDiffuseForAxisXZ> ; 
		mipfilter = LINEAR; 
		AddressU = WRAP; 
		AddressV = WRAP;
	},

	sampler_state
	{
		texture = <TextureDiffuseForAxisXZ2> ; 
		mipfilter = LINEAR; 
		AddressU = WRAP; 
		AddressV = WRAP;
	},

	sampler_state
	{
		texture = <TextureDiffuseForAxisXZ3> ; 
		mipfilter = LINEAR; 
		AddressU = WRAP; 
		AddressV = WRAP;
	}
};

sampler TextureDiffuseForAxisYSampler[3] =
{
	sampler_state
	{
		texture = <TextureDiffuseForAxisY> ; 
		mipfilter = LINEAR; 
		AddressU = WRAP; 
		AddressV = WRAP;
	},

	sampler_state
	{
		texture = <TextureDiffuseForAxisY2> ; 
		mipfilter = LINEAR; 
		AddressU = WRAP; 
		AddressV = WRAP;
	},

	sampler_state
	{
		texture = <TextureDiffuseForAxisY3> ; 
		mipfilter = LINEAR; 
		AddressU = WRAP; 
		AddressV = WRAP;
	}
};

sampler TextureNormalMapForAxisXZSampler[3] =
{
	sampler_state
	{
		texture = <TextureNormalMapForAxisXZ> ; 
		mipfilter = LINEAR; 
		AddressU = WRAP; 
		AddressV = WRAP;
	},

	sampler_state
	{
		texture = <TextureNormalMapForAxisXZ2> ; 
		mipfilter = LINEAR; 
		AddressU = WRAP; 
		AddressV = WRAP;
	},

	sampler_state
	{
		texture = <TextureNormalMapForAxisXZ3> ; 
		mipfilter = LINEAR; 
		AddressU = WRAP; 
		AddressV = WRAP;
	}
};

sampler TextureNormalMapForAxisYSampler[3] =
{ 
	sampler_state
	{
		texture = <TextureNormalMapForAxisY> ; 
		mipfilter = LINEAR; 
		AddressU = WRAP; 
		AddressV = WRAP;
	},

	sampler_state
	{
		texture = <TextureNormalMapForAxisY2> ; 
		mipfilter = LINEAR; 
		AddressU = WRAP; 
		AddressV = WRAP;
	},

	sampler_state
	{
		texture = <TextureNormalMapForAxisY3> ; 
		mipfilter = LINEAR; 
		AddressU = WRAP; 
		AddressV = WRAP;
	}
};


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//	Calculate weight for triplaner mapping.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

float3 GetTriplanarWeights(float3 normal)
{
	float3 axisWeights;
	axisWeights = (abs(normal.xyz) - 0.2) * 7;
	axisWeights = pow(axisWeights, 3.0);	
	axisWeights /= (axisWeights.x + axisWeights.y + axisWeights.z).xxx;	
	return axisWeights;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//	Tri-planar texture mapping for render quality low and normal
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

void GetTextureForRenderQualityLow(int materialIndex, float2 uvForAxis, out float4 diffuseTexture)
{
    diffuseTexture = 
		tex2D(TextureDiffuseForAxisXZSampler[materialIndex], uvForAxis); 
}

void GetTextureForRenderQualityNormal(int materialIndex, float2 uvForAxisX, float2 uvForAxisY, float2 uvForAxisZ, float3 triplanarWeights, out float4 diffuseTexture)
{
    diffuseTexture = 
		tex2D(TextureDiffuseForAxisXZSampler[materialIndex], uvForAxisX) * triplanarWeights.x + 
		tex2D(TextureDiffuseForAxisYSampler[materialIndex], uvForAxisY) * triplanarWeights.y + 
		tex2D(TextureDiffuseForAxisXZSampler[materialIndex], uvForAxisZ) * triplanarWeights.z; 
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//	Tri-planar texture mapping for render quality high or extreme
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

void GetTextureForRenderQualityHighOrExtreme(
	int materialIndex, float2 uvForAxisX, float2 uvForAxisY, float2 uvForAxisZ, float3 normal, float3 triplanarWeights, 
	out float4 diffuseTexture, out float4 normalNew)
{
    diffuseTexture = 
		tex2D(TextureDiffuseForAxisXZSampler[materialIndex], uvForAxisX) * triplanarWeights.x + 
		tex2D(TextureDiffuseForAxisYSampler[materialIndex], uvForAxisY) * triplanarWeights.y + 
		tex2D(TextureDiffuseForAxisXZSampler[materialIndex], uvForAxisZ) * triplanarWeights.z; 

    ////////////////////////////////////////////////////////////////////////////////////
    //	This is trick like hell. We don't do trasnformation of normal from texture to world space
    //	using tangent space as others do. Because our textures are always on three axis, we can
    //	extract xyz from texture and just put it into world space. Always with regard to texture axis.
    //	So we don't need to do matrix multiplications.
    //	You can see that we don't need tangent or binormal vectors. Even 'normal' we use only for getting
    //	proper orientation.
    ////////////////////////////////////////////////////////////////////////////////////

    //	Normal for axis X
	float4 encodedNormalX = tex2D(TextureNormalMapForAxisXZSampler[materialIndex], uvForAxisX);
    float3 normalForAxisX = GetNormalVectorFromDDS(encodedNormalX).zyx;
    normalForAxisX.x *= sign(normal.x);
    
    //	Normal for axis Y
	float4 encodedNormalY = tex2D(TextureNormalMapForAxisYSampler[materialIndex], uvForAxisY);
    float3 normalForAxisY = GetNormalVectorFromDDS(encodedNormalY).xzy;
    normalForAxisY.y *= sign(normal.y);
    
    //	Normal for axis Z
	float4 encodedNormalZ = tex2D(TextureNormalMapForAxisXZSampler[materialIndex], uvForAxisZ);
    float3 normalForAxisZ = GetNormalVectorFromDDS(encodedNormalZ).yxz;
    normalForAxisZ.z *= sign(normal.z);
    
    //	Blend normals using triplanar weights
    normalNew = float4( 
		normalForAxisX * triplanarWeights.x + 
		normalForAxisY * triplanarWeights.y + 
		normalForAxisZ * triplanarWeights.z, 1.0f);

 
 #if 0
	//	We don't need to normalize because it works even without it - I haven't noticed any problems
    //normalNew.xyz = normalize(normalNew);

#else

	//
	// NOTE:
	//
	// It seems, that upper version of voxel surface geometry normal calculation is wrong. At least it completely
	// removes low-frequency surface details and substitutes it with tangent space normal swizzled to worldspace
	//

	normalNew.xyz = normalize(normal + normalNew * 2);	

#endif



	//emissivity
	//we read from w component because normals in our DDS are encoded as RGBA
	normalNew.w = 
		encodedNormalX.w * triplanarWeights.x + 
		encodedNormalY.w * triplanarWeights.y + 
		encodedNormalZ.w * triplanarWeights.z;
} 


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//	Triplanar rendering
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

MyGbufferPixelShaderOutput GetTriplanarPixel(int materialIndex, float3 worldPositionForTextureCoords, float3 triplanarWeights, float3 normal, float viewDistance, float specularIntensity, float specularPower, float perVertexAmbient, uniform int renderQuality)
{
	//	Vertex normal vector	
    normal = normalize(normal);	

	float4 diffuseTextureNear;
	float4 diffuseTextureFar;
	float4 normalNear;
	float4 normalFar;
	float4 diffuseTexture;

	float baseScale = 1 + 9 * step(1000, viewDistance);

	float SCALE_NEAR = TEXTURE_SCALE_NEAR * baseScale;
	float SCALE_FAR = TEXTURE_SCALE_FAR * baseScale;

	//	Voxel 'texture scale by distance' trick (this is not about LOD at all)
	float interpolatorForTextureByDistance = saturate((viewDistance - SCALE_NEAR) / (SCALE_FAR - SCALE_NEAR));

	float3 texCoordNear = worldPositionForTextureCoords.xyz / SCALE_NEAR;
	float3 texCoordFar = worldPositionForTextureCoords.xyz / SCALE_FAR;

	//	Near texture		
	float2 uvForAxisX_Near = texCoordNear.zy;
	float2 uvForAxisY_Near = texCoordNear.xz;
	float2 uvForAxisZ_Near = texCoordNear.xy;

	//	Far texture
	float2 uvForAxisX_Far = texCoordFar.zy;
	float2 uvForAxisY_Far = texCoordFar.xz;
	float2 uvForAxisZ_Far = texCoordFar.xy;

	if (renderQuality == RENDER_QUALITY_LOW)
	{
		GetTextureForRenderQualityNormal(materialIndex, uvForAxisX_Near, uvForAxisY_Near, uvForAxisZ_Near, triplanarWeights, diffuseTextureNear);
		GetTextureForRenderQualityNormal(materialIndex, uvForAxisX_Far, uvForAxisY_Far, uvForAxisZ_Far, triplanarWeights, diffuseTextureFar);
	}
	else 	//	This else will be executed for: RENDER_QUALITY_NORMAL, RENDER_QUALITY_HIGH, RENDER_QUALITY_EXTREME
	{
		GetTextureForRenderQualityHighOrExtreme(materialIndex, uvForAxisX_Near, uvForAxisY_Near, uvForAxisZ_Near, normal, triplanarWeights, diffuseTextureNear, normalNear);
		GetTextureForRenderQualityHighOrExtreme(materialIndex, uvForAxisX_Far, uvForAxisY_Far, uvForAxisZ_Far, normal, triplanarWeights, diffuseTextureFar, normalFar);
		normal = lerp(normalNear, normalFar, interpolatorForTextureByDistance);
		specularIntensity = normalNear.w * specularIntensity;
	}

		//	Combine near and far textures
	diffuseTexture = lerp(diffuseTextureNear, diffuseTextureFar, interpolatorForTextureByDistance);
		
	if (renderQuality != RENDER_QUALITY_LOW)
	{
		float highAmbientStart = 2000;
		float highAmbientFull = 2500;
		float ambientMultiplier = lerp(1.0f, 1.5f, (viewDistance - highAmbientStart) / (highAmbientFull - highAmbientStart));
		ambientMultiplier = clamp(ambientMultiplier, 1, 1.5f);
		diffuseTexture.xyz += EnablePerVertexAmbient * perVertexAmbient * diffuseTexture.xyz * ambientMultiplier;
	}
	
	//	Output into MRT
	MyGbufferPixelShaderOutput output = GetGbufferPixelShaderOutput(normal, diffuseTexture.xyz, specularIntensity / SPECULAR_INTENSITY_RATIO, specularPower / SPECULAR_POWER_RATIO, viewDistance);
	output.DepthAndEmissivity.a = diffuseTexture.w;
	return output;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//	Triplanar rendering - for forward render (low details)
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

float4 GetTriplanarPixelForward(int materialIndex, float3 worldPositionForTextureCoords, float3 triplanarWeights, float3 normal, float viewDistance, float specularIntensity, float specularPower, float perVertexAmbient, uniform int renderQuality)
{
	//	Vertex normal vector	
	normal = normalize(normal);

	//	Near texture		
	float2 uvForAxisX_Near = worldPositionForTextureCoords.zy / TEXTURE_SCALE_NEAR;
	float2 uvForAxisY_Near = worldPositionForTextureCoords.xz / TEXTURE_SCALE_NEAR;
	float2 uvForAxisZ_Near = worldPositionForTextureCoords.xy / TEXTURE_SCALE_NEAR;

	// Get diffuse
	float4 diffuseTextureNear;
	GetTextureForRenderQualityNormal(materialIndex, uvForAxisX_Near, uvForAxisY_Near, uvForAxisZ_Near, triplanarWeights, diffuseTextureNear);

	// Add per vertex Ambient
	diffuseTextureNear += EnablePerVertexAmbient * perVertexAmbient * diffuseTextureNear;

	// Fog
	float fogBlend = (viewDistance - FogDistanceNear) / (FogDistanceFar - FogDistanceNear);
	diffuseTextureNear.xyz = lerp(diffuseTextureNear.xyz, FogColor, saturate(fogBlend) * FogMultiplier);

	return diffuseTextureNear;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//	Here begins single-material rendering part
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

struct VertexShaderInput
{
    float4 PositionAndAmbient : POSITION0;
    float4 Normal : NORMAL;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 ViewDistance : TEXCOORD0; //Store linear depth in x, radial in y
	float3 WorldPositionForTextureCoords : TEXCOORD1;
    float3 Normal : TEXCOORD2;
    float3 TriplanarWeights : TEXCOORD3;
	float Ambient: TEXCOORD4;
};

struct VertexShaderOutput_Forward
{
    float4 Position : POSITION0;
	float4 LightColor : COLOR0;
    float ViewDistance : TEXCOORD0;
	float3 WorldPositionForTextureCoords : TEXCOORD1;
    float3 Normal : TEXCOORD2;
	float3 WorldPosition : TEXCOORD3;
	float3 TriplanarWeights : TEXCOORD4;
	float Ambient: TEXCOORD5;
};

struct VertexShaderOutput_Multimaterial
{
	VertexShaderOutput Single;
	float3 Alpha : TEXCOORD5;
};

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Macro for texture quality NORMAL
//
//	I think this Texture[] assignement isn't needed but I rather did it

#define DECLARE_TEXTURES_QUALITY_LOW \
    MinFilter[0] = LINEAR; \
    MagFilter[0] = LINEAR; \
    MinFilter[1] = LINEAR; \
    MagFilter[1] = LINEAR; \
    MinFilter[2] = LINEAR; \
    MagFilter[2] = LINEAR; \
    MinFilter[3] = LINEAR; \
    MagFilter[3] = LINEAR; \
    MinFilter[4] = LINEAR; \
    MagFilter[4] = LINEAR; \
    MinFilter[5] = LINEAR; \
    MagFilter[5] = LINEAR; \



#define DECLARE_TEXTURES_QUALITY_NORMAL \
    MinFilter[0] = LINEAR; \
    MagFilter[0] = LINEAR; \
    MinFilter[1] = LINEAR; \
    MagFilter[1] = LINEAR; \
    MinFilter[2] = LINEAR; \
    MagFilter[2] = LINEAR; \
    MinFilter[3] = LINEAR; \
    MagFilter[3] = LINEAR; \
    MinFilter[4] = LINEAR; \
    MagFilter[4] = LINEAR; \
    MinFilter[5] = LINEAR; \
    MagFilter[5] = LINEAR; \
    MinFilter[6] = LINEAR; \
    MagFilter[6] = LINEAR; \
    MinFilter[7] = LINEAR; \
    MagFilter[7] = LINEAR; \
    MinFilter[8] = LINEAR; \
    MagFilter[8] = LINEAR; \
    MinFilter[9] = LINEAR; \
    MagFilter[9] = LINEAR; \
    MinFilter[10] = LINEAR; \
    MagFilter[10] = LINEAR; \
    MinFilter[11] = LINEAR; \
    MagFilter[11] = LINEAR; \
  
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Macro for texture quality HIGH and EXTREME
//
//	I think this Texture[] assignement isn't needed but I rather did it

#define DECLARE_TEXTURES_QUALITY_HIGH \
    MinFilter[0] = LINEAR; \
    MagFilter[0] = LINEAR; \
    MinFilter[1] = LINEAR; \
    MagFilter[1] = LINEAR; \
    MinFilter[2] = LINEAR; \
    MagFilter[2] = LINEAR; \
    MinFilter[3] = LINEAR; \
    MagFilter[3] = LINEAR; \
    MinFilter[4] = LINEAR; \
    MagFilter[4] = LINEAR; \
    MinFilter[5] = LINEAR; \
    MagFilter[5] = LINEAR; \
    MinFilter[6] = LINEAR; \
    MagFilter[6] = LINEAR; \
    MinFilter[7] = LINEAR; \
    MagFilter[7] = LINEAR; \
    MinFilter[8] = LINEAR; \
    MagFilter[8] = LINEAR; \
    MinFilter[9] = LINEAR; \
    MagFilter[9] = LINEAR; \
    MinFilter[10] = LINEAR; \
    MagFilter[10] = LINEAR; \
    MinFilter[11] = LINEAR; \
    MagFilter[11] = LINEAR; \
	MinFilter[12] = LINEAR; \
    MagFilter[12] = LINEAR; \
	MinFilter[13] = LINEAR; \
    MagFilter[13] = LINEAR; \
	MinFilter[14] = LINEAR; \
    MagFilter[14] = LINEAR; \
	MinFilter[15] = LINEAR; \
    MagFilter[15] = LINEAR; \


	#define DECLARE_TEXTURES_QUALITY_EXTREME \
    MinFilter[0] = ANISOTROPIC; \
    MagFilter[0] = ANISOTROPIC; \
    MaxAnisotropy[0] = 16; \
    MinFilter[1] = ANISOTROPIC; \
    MagFilter[1] = ANISOTROPIC; \
    MaxAnisotropy[1] = 16; \
    MinFilter[2] = ANISOTROPIC; \
    MagFilter[2] = ANISOTROPIC; \
    MaxAnisotropy[2] = 16; \
    MinFilter[3] = ANISOTROPIC; \
    MagFilter[3] = ANISOTROPIC; \
    MaxAnisotropy[3] = 16; \
    MinFilter[4] = ANISOTROPIC; \
    MagFilter[4] = ANISOTROPIC; \
    MaxAnisotropy[4] = 16; \
    MinFilter[5] = ANISOTROPIC; \
    MagFilter[5] = ANISOTROPIC; \
    MaxAnisotropy[5] = 16; \
    MinFilter[6] = ANISOTROPIC; \
    MagFilter[6] = LINEAR; \
    MaxAnisotropy[6] = 16; \
    MinFilter[7] = ANISOTROPIC; \
    MagFilter[7] = LINEAR; \
    MaxAnisotropy[7] = 16; \
    MinFilter[8] = ANISOTROPIC; \
    MagFilter[8] = ANISOTROPIC; \
    MaxAnisotropy[8] = 16; \
    MinFilter[9] = ANISOTROPIC; \
    MagFilter[9] = ANISOTROPIC; \
    MaxAnisotropy[9] = 16; \
    MinFilter[10] = ANISOTROPIC; \
    MagFilter[10] = ANISOTROPIC; \
    MaxAnisotropy[10] = 16; \
    MinFilter[11] = ANISOTROPIC; \
    MagFilter[11] = LINEAR; \
    MaxAnisotropy[11] = 16; \
	MinFilter[12] = ANISOTROPIC; \
    MagFilter[12] = ANISOTROPIC; \
    MaxAnisotropy[12] = 16; \
	MinFilter[13] = ANISOTROPIC; \
    MagFilter[13] = ANISOTROPIC; \
    MaxAnisotropy[13] = 16; \
	MinFilter[14] = ANISOTROPIC; \
    MagFilter[14] = ANISOTROPIC; \
    MaxAnisotropy[14] = 16; \
	MinFilter[15] = ANISOTROPIC; \
    MagFilter[15] = ANISOTROPIC; \
    MaxAnisotropy[15] = 16; \
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

