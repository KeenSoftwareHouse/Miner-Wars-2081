#include "../MyEffectBase.fxh"

float Contrast;
float Hue;
float Saturation;

Texture DiffuseTexture;
sampler DiffuseSampler = sampler_state 
{ 
	texture = <DiffuseTexture> ; 
	magfilter = POINT; 
	minfilter = POINT; 
	mipfilter = NONE; 
	AddressU = CLAMP; 
	AddressV = CLAMP;
};

float2 HalfPixel;

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float3 TexCoordAndCornerIndex	: TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;	
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;
	output.Position = input.Position;
	output.TexCoord = input.TexCoordAndCornerIndex.xy + HalfPixel;
	return output;
}


// Converts the rgb value to hsv, where H's range is -1 to 5
float3 rgb_to_hsv(float3 RGB)
{
    float r = RGB.x;
    float g = RGB.y;
    float b = RGB.z;

    float minChannel = min(r, min(g, b));
    float maxChannel = max(r, max(g, b));

    float h = 0;
    float s = 0;
    float v = maxChannel;

    float delta = maxChannel - minChannel;

    if (delta != 0)
    {
        s = delta / v;

        if (r == v) h = (g - b) / delta;
        else if (g == v) h = 2 + (b - r) / delta;
        else if (b == v) h = 4 + (r - g) / delta;
    }

    return float3(h, s, v);
}

float3 hsv_to_rgb(float3 HSV)
{
    float3 RGB = HSV.z;

    float h = HSV.x;
    float s = HSV.y;
    float v = HSV.z;

    float i = floor(h);
    float f = h - i;

    float p = (1.0 - s);
    float q = (1.0 - s * f);
    float t = (1.0 - s * (1 - f));

    if (i == 0) { RGB = float3(1, t, p); }
    else if (i == 1) { RGB = float3(q, 1, p); }
    else if (i == 2) { RGB = float3(p, 1, t); }
    else if (i == 3) { RGB = float3(p, q, 1); }
    else if (i == 4) { RGB = float3(t, p, 1); }
    else /* i == -1 */ { RGB = float3(1, p, q); }

    RGB *= v;

    return RGB;
}


float4 PixelShaderFunction(VertexShaderOutput input, float2 screenPosition : VPOS) : COLOR0
{
	float4 col = tex2D(DiffuseSampler, input.TexCoord);
	//float4 color = pow(source, Contrast);//float4(1,0,0,1);



    float3 hsv = rgb_to_hsv(col.xyz);

    hsv.x += 0.1f;
    // Put the hue back to the -1 to 5 range
    //if (hsv.x > 5) { hsv.x -= 6.0; }
    hsv = hsv_to_rgb(hsv);
    float4 newColor = float4(hsv,col.w);

    float4 colorWithBrightnessAndContrast = newColor;

	
    colorWithBrightnessAndContrast.rgb /= colorWithBrightnessAndContrast.a;
    colorWithBrightnessAndContrast.rgb = colorWithBrightnessAndContrast.rgb + Hue;
    colorWithBrightnessAndContrast.rgb = ((colorWithBrightnessAndContrast.rgb - 0.5f) * max(Contrast + 1.0, 0)) + 0.5f;  
    colorWithBrightnessAndContrast.rgb *= colorWithBrightnessAndContrast.a;

    float greyscale = dot(colorWithBrightnessAndContrast.rgb, float3(0.3, 0.59, 0.11)); 
    colorWithBrightnessAndContrast.rgb = lerp(greyscale, colorWithBrightnessAndContrast.rgb, col.a * (Saturation + 1.0));       
    return colorWithBrightnessAndContrast;







//Screen
//color = 255 - ( ( 255 - bottom ) * ( 255 - top ) ) / 255

//Overlay
//color = bottom < 128 ? ( 2 * bottom * top ) / 255
//                     : 255 - ( 2 * ( 255 - bottom ) * ( 255 - top ) / 255 )


	//return color;
}

technique Default
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
}
