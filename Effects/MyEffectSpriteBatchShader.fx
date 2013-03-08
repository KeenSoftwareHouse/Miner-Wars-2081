sampler TextureSampler : register(s0);

#define DECLARE_TEXTURE(Name, index) \
    texture2D Name; \
    sampler Name##Sampler : register(s##index) = sampler_state { Texture = (Name); AddressU = WRAP; AddressV = WRAP; magfilter = LINEAR; minfilter = LINEAR; mipfilter = LINEAR;  };

#define SAMPLE_TEXTURE(Name, texCoord)  tex2D(Name##Sampler, texCoord)

DECLARE_TEXTURE(Texture1, 1);
DECLARE_TEXTURE(Texture2, 2);
float2 Texture2Tiling = (1.0,1.0);


float4 TextureOverlapMergeEffectMain(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{

    float4 tex1color = tex2D(TextureSampler, texCoord);
    float4 tex2overlay = SAMPLE_TEXTURE(Texture2, texCoord * Texture2Tiling);
	
    float4 retColor = tex1color;
    retColor *= color;
    retColor *= (1-tex2overlay.a);
    retColor.a = 0/*(tex1color.r+tex1color.g+tex1color.b)/3.0*/;
    return retColor;

}



technique TextureOverlapMergeEffect
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 TextureOverlapMergeEffectMain();
    }
}



