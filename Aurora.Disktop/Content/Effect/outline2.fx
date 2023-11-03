#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	return tex2D(SpriteTextureSampler,input.TextureCoordinates) * input.Color;

    // 采样主场景纹理
    float4 mainColor = tex2D(SpriteTextureSampler,input.TextureCoordinates);

    // 检查是否在轮廓内部
    float4 outlineColor = float4(0, 0, 0, 1); // 黑色轮廓
    float threshold = 0.1; // 调整轮廓宽度

    float4 outline = step(threshold, mainColor);

    // 如果在轮廓内部，则将主场景颜色设置为轮廓颜色
    return lerp(mainColor, outlineColor, outline.a);


}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
