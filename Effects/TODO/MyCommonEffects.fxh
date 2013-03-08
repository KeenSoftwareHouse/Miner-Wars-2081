////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//	Quality values:
//		0: high quality
//		1: normal quality (without normal map texture; without specular texture and light - but only on voxels, other shaders are unaffected)
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


const float TextureScaleModelDetail = 20;//3;

const float VoxelTextureScaleNear = 100;
const float VoxelTextureScaleFar = 1000;
const float VoxelTextureDistanceNear = 100;//10;
const float VoxelTextureDistanceFar = 1000;

const float ExplosionDebrisTextureScale = 60 * 0.25;

float FogDistanceNear;
float FogDistanceFar;

//	This will allow to blend result color not to full fog, but only e.g. 40% of it so distant object will still preserve part of their texture
const float FogMultiplier = 0.5;//0.4;

//	This is direction vector from every pixel (voxel, model, etc) to sun. It's not enough if you change it, because MyShadows assume that sun is in negative Z direction.
//	So, if you want to have sun on e.g. left side of background cube, change this vector and change logic in MyShadows. Or if you want to have sun in some not 90 degre
//	position (e.g. 20 degres on the horizont), than you must change a lot in MyShadows...
float3 DirectionToSun;// = normalize(float3(0, 0, -1));

//	Color of sun light. This one will influence 'sun' factor comming from shadows.
float4 SunColor;

//	This will make reflector's near light (not cone, but point, thus sphere around camera) little bit less shiny and less bright
const float ReflectorNearLightDistanceMultiplier = 0.5;

//	Ambient light coming from stars, not from sun. For particles.
//  If you change it here, change it also in MyConstants.cs in constant: AMBIENT_COLOR
const float AmbientColor = 0.07f;

//	Cockpit interior reflector is always offseted from camera position, so it feels like light is dynamic
const float3 CockpitReflectorPositionDelta = float3(1, 1, 1);

float4 CockpitInteriorLight;

float TextureScale;

float4x4	WorldViewProjectionMatrix;
float4x4	WorldMatrix;
float4x4	ViewProjectionMatrix;
float3		EyePosition;

//	Fog
float		ReflectorRange;
float4		FogColor;
float4		FogColorForBackground;

//	Reflector light - coming from the camera/player
float3		ReflectorPosition;
float3		ReflectorDirection;
float		ReflectorConeMaxAngleCos;
float4		ReflectorColor;

//	Near light (fake reflector light) - coming from the camera/player
//	Used for lighting near voxels and guns visible from the cockpit. It's also nice when guns are shoting.
float		NearLightRange;
float		NearLightRangeOneMinus;
float4		NearLightColor;

//	Material attributes
float		Shininess;			//	Output from phong specular will be scaled by this amount
float		SpecularPower;		//	Specular exponent from phong lighting model.  controls the "tightness" of specular highlights.

//	Dynamic light
struct DynamicLight 
{
    float3 Position;
    float4 Color;
    float Falloff;
    float Range;
};

//	Here we define array for holding lights with max size. No more light can be used, but less can. It depends on how many lights
//	are near object we want to draw and max number of light set by user in game options.
//	If you change length of this array, change it too in MyLightsConstants.MAX_LIGHTS_FOR_EFFECT
DynamicLight DynamicLights[8];

//	This number is set specificaly for every model or voxel cell and tells us how many light to use for lighting calculations
int DynamicLightsCount;
float ModelAlpha;


struct GbufferPixelShaderOutput
{
    float4 Depth : COLOR0;
    float4 Normal : COLOR1;
};

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//	Voxel rendering - This part is common for single-material, multi-material and multi-material clear too
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

Texture TextureDiffuseForAxisXZ;
sampler TextureDiffuseForAxisXZSampler = sampler_state 
{ 
	texture = <TextureDiffuseForAxisXZ> ; 
	magfilter = LINEAR; 
	minfilter = LINEAR; 
	mipfilter = LINEAR; 
	AddressU = WRAP; 
	AddressV = WRAP;
};

Texture TextureDiffuseForAxisY;
sampler TextureDiffuseForAxisYSampler = sampler_state 
{ 
	texture = <TextureDiffuseForAxisY> ; 
	magfilter = LINEAR; 
	minfilter = LINEAR; 
	mipfilter = LINEAR; 
	AddressU = WRAP; 
	AddressV = WRAP;
};

Texture TextureNormalMapForAxisXZ;
sampler TextureNormalMapForAxisXZSampler = sampler_state 
{ 
	texture = <TextureNormalMapForAxisXZ> ; 
	magfilter = LINEAR; 
	minfilter = LINEAR; 
	mipfilter = LINEAR; 
	AddressU = WRAP; 
	AddressV = WRAP;
};

Texture TextureNormalMapForAxisY;
sampler TextureNormalMapForAxisYSampler = sampler_state 
{ 
	texture = <TextureNormalMapForAxisY> ; 
	magfilter = LINEAR; 
	minfilter = LINEAR; 
	mipfilter = LINEAR; 
	AddressU = WRAP; 
	AddressV = WRAP;
};

Texture TextureSpecularForAxisXZ;
sampler TextureSpecularForAxisXZSampler = sampler_state 
{ 
	texture = <TextureSpecularForAxisXZ> ; 
	magfilter = LINEAR; 
	minfilter = LINEAR; 
	mipfilter = LINEAR; 
	AddressU = WRAP; 
	AddressV = WRAP;
};

Texture TextureSpecularForAxisY;
sampler TextureSpecularForAxisYSampler = sampler_state 
{ 
	texture = <TextureSpecularForAxisY> ; 
	magfilter = LINEAR; 
	minfilter = LINEAR; 
	mipfilter = LINEAR; 
	AddressU = WRAP; 
	AddressV = WRAP;
};

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//	Textures for models / phys objects
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

Texture TextureMainDiffuse;
sampler TextureMainDiffuseSampler = sampler_state 
{ 
	texture = <TextureMainDiffuse> ; 
	magfilter = LINEAR; 
	minfilter = LINEAR; 
	mipfilter = LINEAR; 
	AddressU = WRAP; 
	AddressV = WRAP;
};

Texture TextureMainNormal;
sampler TextureMainNormalSampler = sampler_state 
{ 
	texture = <TextureMainNormal> ; 
	magfilter = LINEAR; 
	minfilter = LINEAR; 
	mipfilter = LINEAR; 
	AddressU = WRAP; 
	AddressV = WRAP;
};

Texture TextureMainSpecular;
sampler TextureMainSpecularSampler = sampler_state 
{ 
	texture = <TextureMainSpecular> ; 
	magfilter = LINEAR; 
	minfilter = LINEAR; 
	mipfilter = LINEAR; 
	AddressU = WRAP; 
	AddressV = WRAP;
};

Texture TextureLightMap;
sampler TextureLightMapSampler = sampler_state 
{ 
	texture = <TextureLightMap> ; 
	magfilter = LINEAR; 
	minfilter = LINEAR; 
	mipfilter = LINEAR; 
	AddressU = WRAP; 
	AddressV = WRAP;
};

Texture TextureDetailDiffuse;
sampler TextureDetailDiffuseSampler = sampler_state 
{ 
	texture = <TextureDetailDiffuse> ; 
	magfilter = LINEAR; 
	minfilter = LINEAR; 
	mipfilter = LINEAR; 
	AddressU = WRAP; 
	AddressV = WRAP;
};

Texture TextureDetailSpecular;
sampler TextureDetailSpecularSampler = sampler_state 
{ 
	texture = <TextureDetailSpecular> ; 
	magfilter = LINEAR; 
	minfilter = LINEAR; 
	mipfilter = LINEAR; 
	AddressU = WRAP; 
	AddressV = WRAP;
};

Texture TextureDetailNormal;
sampler TextureDetailNormalSampler = sampler_state 
{ 
	texture = <TextureDetailNormal> ; 
	magfilter = LINEAR; 
	minfilter = LINEAR; 
	mipfilter = LINEAR; 
	AddressU = WRAP; 
	AddressV = WRAP;
};


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//	Functions used by all types of shaders
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//	Calculate weight for triplaner mapping.
float3 GetTriplanarWeights(float3 normal)
{
	float3 axisWeights;
	axisWeights = (abs(normal.xyz) - 0.2) * 7;
	axisWeights = pow(axisWeights, 3.0);	
	axisWeights /= (axisWeights.x + axisWeights.y + axisWeights.z).xxx;	
	return axisWeights;
}

//	Calculate the intensity of the light with exponential falloff
float GetDynamicLightBaseIntensity(DynamicLight light, float distance)
{
	return pow(saturate((light.Range - distance) / light.Range), light.Falloff);
}

//	Calculate point light with attenuation, diffuse and specular texture. Can be used for normal mapping too.
float4 CalculateDynamicLight_DiffuseSpecular(DynamicLight light, float3 position, float3 normal, float4 diffuseTexture, float4 specularTexture)
{
	float3 lightVector = light.Position - position;
	float lightDist = length(lightVector);
	float3 directionToLight = normalize(lightVector);

	float baseIntensity = GetDynamicLightBaseIntensity(light, lightDist);

	float diffuseIntensity = saturate( dot(directionToLight, normal));
	float4 diffuse = diffuseIntensity * light.Color * diffuseTexture;

	//	Calculate Phong components per-pixel
	float3 reflectionVector = normalize(reflect(-directionToLight, normal));
	float3 directionToCamera = normalize(EyePosition - position);
	
	//	Calculate specular component
	float4 specular = light.Color * specularTexture * Shininess * pow(saturate(dot(reflectionVector, directionToCamera)), SpecularPower);

	return baseIntensity * (diffuse + specular);
}


//	Compute distance from camera to vertex. This is then interpolated by pixel shader.
//	Notice: this value IS NOT in range <0..1>, but contains reall distance in meters.
float GetDistanceToReflector(float3 position)
{
	return length(EyePosition - position);

	//float dist = length(EyePosition - position);
    //return 1 - saturate(dist / ReflectorRange);
    
    //return pow(1 - saturate(dist / ReflectorRange), 5);
    
    //float ret = 1 - saturate(dist / ReflectorRange);
    
    //ret = 1 / exp((1 - ret) * 0.33);
    //ret = exp(ret * 0.33);
    
    //return ret;
}



//	Calculate the view direction, from the eye to the surface.  not normalized, in world space.
float3 GetViewDirection(float3 position)
{
    return position - EyePosition;    
}

//	Calculate attenuation factor of reflector light. Result will be in interval <0..1> and can be used to multiply diffuse/specular components.
float4 GetReflectorColorWithAttenuation(float distanceToReflectorInvertedNormalized, float3 directionToReflector)
{
	float actualAngle = 1 - dot(ReflectorDirection, -directionToReflector);	
	float4 ret = 6 * distanceToReflectorInvertedNormalized;

	//	Attenuate by cone angle
	ret *= 1 - saturate(actualAngle / ReflectorConeMaxAngleCos);
	ret = saturate(ret);
	
	//	Make the light not too bright, so it will be in interval <0..0,6>
    ret = ret * 0.6 * ReflectorColor;
	return ret;
}

float GetNearLightAttenuation(float distanceToReflector)
{
    //	This light's range depends on normalized distance, so here we take it from it
	//	Make the light not too bright, so it will be in interval <0..0,8>
    //return saturate((distanceToReflectorInvertedNormalized - NearLightRangeOneMinus) / NearLightRange) * 0.8;
    return (1 - saturate(distanceToReflector / NearLightRange)) * 0.8;
}

//	This light is here only because I want dynamic light on cockpit, even if in total dark. But I don't want big differences, so that's why here is LERP.
//	This light is connected to miner ship reflector or 'near light'. Thus if reflector is off or player isn't shooting, it's black.
float GetCockpitLightDiffuseMultiplier(float3 normal, float3 directionToReflector)
{
    //float cockpitLightDiffuseMultiplier = saturate(dot(normal, directionToReflector));
    //cockpitLightDiffuseMultiplier = lerp(0.25, 0.75, cockpitLightDiffuseMultiplier);

    return 0.3 + 0.4 * saturate(dot(normal, directionToReflector));
}

//	Sun - diffuse (see that I use ABS for the angle, because I want to have sun affecting the glass from both sides
float GetSunDiffuseMultiplierForCockpitGlassAndDecals(float3 normal)
{
	return saturate(abs(dot(normal, DirectionToSun)));
}

//	Voxel rendering
void GetTextureForLowQuality(float2 uvForAxisX, float2 uvForAxisY, float2 uvForAxisZ, float3 triplanarWeights, out float4 diffuseTexture)
{
    diffuseTexture = 
		tex2D(TextureDiffuseForAxisXZSampler, uvForAxisX) * triplanarWeights.x + 
		tex2D(TextureDiffuseForAxisYSampler, uvForAxisY) * triplanarWeights.y + 
		tex2D(TextureDiffuseForAxisXZSampler, uvForAxisZ) * triplanarWeights.z; 
}

//	Voxel rendering
void GetTextureForHighQuality(float2 uvForAxisX, float2 uvForAxisY, float2 uvForAxisZ, float3 normal, float3 triplanarWeights, out float4 diffuseTexture, out float4 specularTexture, out float3 normalNew)
{
    diffuseTexture = 
		tex2D(TextureDiffuseForAxisXZSampler, uvForAxisX) * triplanarWeights.x + 
		tex2D(TextureDiffuseForAxisYSampler, uvForAxisY) * triplanarWeights.y + 
		tex2D(TextureDiffuseForAxisXZSampler, uvForAxisZ) * triplanarWeights.z; 

    specularTexture = 
		tex2D(TextureSpecularForAxisXZSampler, uvForAxisX) * triplanarWeights.x + 
		tex2D(TextureSpecularForAxisYSampler, uvForAxisY) * triplanarWeights.y + 
		tex2D(TextureSpecularForAxisXZSampler, uvForAxisZ) * triplanarWeights.z; 

    ////////////////////////////////////////////////////////////////////////////////////
    //	This is trick like hell. We don't do trasnformation of normal from texture to world space
    //	using tangent space as others do. Because our textures are always on three axis, we can
    //	extract xyz from texture and just put it into world space. Always with regard to texture axis.
    //	So we don't need to do matrix multiplications.
    //	You can see that we don't need tangent or binormal vectors. Even 'normal' we use only for getting
    //	proper orientation.
    ////////////////////////////////////////////////////////////////////////////////////
    
    //	Normal for axis X
    float3 normalForAxisX = tex2D(TextureNormalMapForAxisXZSampler, uvForAxisX).zyx;
    normalForAxisX.x *= sign(normal.x);
    
    //	Normal for axis Y
    float3 normalForAxisY = tex2D(TextureNormalMapForAxisYSampler, uvForAxisY).xzy;
    normalForAxisY.y *= sign(normal.y);
    
    //	Normal for axis Z
    float3 normalForAxisZ = tex2D(TextureNormalMapForAxisXZSampler, uvForAxisZ).yxz;
    normalForAxisZ.z *= sign(normal.z);
    
    //	Blend normals using triplanar weights
    normalNew = 
		normalForAxisX * triplanarWeights.x + 
		normalForAxisY * triplanarWeights.y + 
		normalForAxisZ * triplanarWeights.z;
		
	//	I guess we don't need to normalize because it works even without it.
    //normalNew = normalize(normalNew);
} 

//	Voxel rendering
GbufferPixelShaderOutput GetVoxelTexturedLitBumpTriplanarPixel(float3 worldPositionForTextureCoords, float3 worldPosition, float3 triplanarWeights, float3 normal, float ambient, float sun, int quality)
{
	//return float4(0,0,1,1);

	GbufferPixelShaderOutput output;
	
    normal = normalize(normal);
    //viewDirection = normalize(viewDirection);
    //directionToReflector = normalize(directionToReflector);    
	
	float distanceToReflector = GetDistanceToReflector(worldPosition);
	float distanceForTextures = saturate((distanceToReflector - VoxelTextureDistanceNear) / (VoxelTextureDistanceFar - VoxelTextureDistanceNear));
	//float distanceToReflectorInverted = 1 - saturate(distanceToReflector / ReflectorRange);
	//float distanceForFog = saturate((distanceToReflector - FogDistanceNear) / (FogDistanceFar - FogDistanceNear));

	//	Near texture		
	float2 uvForAxisX_Near = worldPositionForTextureCoords.zy / VoxelTextureScaleNear;
	float2 uvForAxisY_Near = worldPositionForTextureCoords.xz / VoxelTextureScaleNear;
	float2 uvForAxisZ_Near = worldPositionForTextureCoords.xy / VoxelTextureScaleNear;
	float4 diffuseTextureNear;
	float4 specularTextureNear;
	float3 normalNear;

	//	Far texture
	float2 uvForAxisX_Far = worldPositionForTextureCoords.zy / VoxelTextureScaleFar;
	float2 uvForAxisY_Far = worldPositionForTextureCoords.xz / VoxelTextureScaleFar;
	float2 uvForAxisZ_Far = worldPositionForTextureCoords.xy / VoxelTextureScaleFar;
	float4 diffuseTextureFar;
	float4 specularTextureFar;
	float3 normalFar;

	float4 specularTexture;
	if (quality == 0)
	{	    
		GetTextureForHighQuality(uvForAxisX_Near, uvForAxisY_Near, uvForAxisZ_Near, normal, triplanarWeights, diffuseTextureNear, specularTextureNear, normalNear);
		GetTextureForHighQuality(uvForAxisX_Far, uvForAxisY_Far, uvForAxisZ_Far, normal, triplanarWeights, diffuseTextureFar, specularTextureFar, normalFar);
		normal = lerp(normalNear, normalFar, distanceForTextures);
		specularTexture = lerp(specularTextureNear, specularTextureFar, distanceForTextures);		
	}
	else		
	{
		GetTextureForLowQuality(uvForAxisX_Near, uvForAxisY_Near, uvForAxisZ_Near, triplanarWeights, diffuseTextureNear);
		GetTextureForLowQuality(uvForAxisX_Far, uvForAxisY_Far, uvForAxisZ_Far, triplanarWeights, diffuseTextureFar);
	}	
	
	//	Combine near and far textures
	float4 diffuseTexture = lerp(diffuseTextureNear, diffuseTextureFar, distanceForTextures);
	
    ////////////////////////////////////////////////////////////////////////////////////
    //	Calculate lighting (diffuse + specular + attenuation)
    ////////////////////////////////////////////////////////////////////////////////////
    
    //	Sun - diffuse
	//float sunDiffuseMultiplier = saturate(dot(normal, DirectionToSun));

    //	Reflector - diffuse
    //float reflectorDiffuseMultiplier = saturate(dot(normal, directionToReflector));
   
	//	Reflector - attenuation
    //float4 reflectorAttenuation = GetReflectorColorWithAttenuation(distanceToReflectorInverted, directionToReflector);       

	//	Combine sun + ambient + reflector + near light (diffuse)
	//	Near light is made almost two times: first is more distant near light and it's in fact reflector spread in 360 degress, but not as far; Second is really near light, and is visible when player shoots
	//float4 resultColor = ambient + SunColor * sun * sunDiffuseMultiplier + reflectorDiffuseMultiplier * (ReflectorNearLightDistanceMultiplier * distanceToReflectorInverted * ReflectorColor + reflectorAttenuation + nearLight);
	//resultColor *= diffuseTexture;
	
	/*
	//	Combine sun + reflector + near light (specular)
	if (quality == 0)
	{	    
		//	Sun - specular
		float3 sunReflectedLight = reflect(DirectionToSun, normal);
		float sunrDotV = saturate(dot(sunReflectedLight, viewDirection));        
		float4 sunSpecularMultiplier = Shininess * pow(sunrDotV, SpecularPower);    

		//	Reflector - specular
		float3 reflectorReflectedLight = reflect(directionToReflector, normal);
		float reflectorrDotV = saturate(dot(reflectorReflectedLight, viewDirection));        
		float4 reflectorSpecularMultiplier = Shininess * pow(reflectorrDotV, SpecularPower);    

	    resultColor += specularTexture * (reflectorSpecularMultiplier * reflectorAttenuation + SunColor * sun * sunSpecularMultiplier);
	}
    
    //	Add dynamic lights
    for (int i = 0; i < DynamicLightsCount; i++)
    {
		if (quality == 0)
		{
			resultColor += CalculateDynamicLight_DiffuseSpecular(DynamicLights[i], worldPosition, normal, diffuseTexture, specularTexture);
		}
		else if (quality == 1)
		{
			resultColor += CalculateDynamicLight_Diffuse(DynamicLights[i], worldPosition, normal, diffuseTexture);
		}
    }

	//	Don't blend to full-fog-color, just little bit (we don't want to have farther objects same color)
    return lerp(resultColor, FogColor, FogMultiplier * distanceForFog);
    */
    
	output.Depth = float4(1, 1.0f, 1.0f, 1.0f); //float4(-input.ViewDepth / FarPlane, 1.0f, 1.0f, 1.0f);	output.Normal.rgb = 0.5f * (normalize(input.Normal) + 1.0f);
	output.Normal.xyz = normal;
	output.Normal.a = 1;	
    
    return output;
}