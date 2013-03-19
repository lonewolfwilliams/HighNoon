/*
float4x4 World;
float4x4 View;
float4x4 Projection;
*/
// TODO: add effect parameters here.

//required
//uniform int videoHeight;
uniform float gameTimeInMS;
uniform float PI = 3.14159265f;

//optional
uniform float amplitude = 0.006f;
uniform float scanClamp = 0.3f;
uniform int strobeRate = 20000;
uniform float strobeDepth = 0.6f;
uniform float vignetteOffset = 0.6f;

//cgwg 
uniform float2 textureSize;
uniform float2 videoSize;
uniform float distortion = 0.1f;

//s0 is the semantic for the first texture register on the graphics device
//this is the default that spritebatch draws into
sampler ColorMapSampler : register(s0); 
/*
struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float4 Color : COLOR0;

    // TODO: add input channels such as texture
    // coordinates and vertex colors here.
};
*/
struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float4 Color : COLOR0;

    // TODO: add vertex shader outputs such as colors and texture
    // coordinates here. These values will automatically be interpolated
    // over the triangle, and provided as input to your pixel shader.
};
/*
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    // TODO: add your vertex shader code here.
	output.TexCoord = input.TexCoord; //pass through
	output.Color = input.Color;

    return output;
}
*/

// Apply radial distortion to the given coordinate.
// gw added a vignette
float2 radialDistortion(float2 coord, out float4 vignette) 
{
	coord *= textureSize.x / videoSize.x;
        float2 cc = coord - 0.5;
        float dist = dot(cc, cc) * distortion;		
		vignette = float4(1,1,1,1) * saturate(vignetteOffset - dist);
        return (coord + cc * (1.0 + dist) * dist) * videoSize.x / textureSize.x;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    // TODO: add your pixel shader code here.

	//colour bleed inspiration from here:
	//http://active.tutsplus.com/tutorials/effects/create-a-retro-crt-distortion-effect-using-rgb-shifting/
	//and
	//http://xboxforums.create.msdn.com/forums/p/93304/559284.aspx

	//stolen from cgwg-crt cg shader
	float4 vignette;
	float2 xy = radialDistortion(input.TexCoord, vignette);

	//not very efficient, I know ^_^

	//floating abheration
	float freq = PI / 1000;
	float twoHerz = sin(freq * gameTimeInMS);
	float abherationOne = twoHerz * amplitude;
	float2 texCoordOffsetRed = float2(0, abherationOne);
    
	float abherationTwo = sin((freq * 1.3) * gameTimeInMS) * amplitude;
	float2 texCoordOffsetBlu = float2(0, abherationTwo);

	float r = tex2D(ColorMapSampler, xy + texCoordOffsetRed).r; 
    float g = tex2D(ColorMapSampler, xy).g; 
    float b = tex2D(ColorMapSampler, xy + texCoordOffsetBlu).b; 
	
	float4 imageColor = float4(r,g,b,1); 
	
	//rolling bar
	float phase = (PI / strobeRate) * gameTimeInMS;
	float currentScanlinePhase = videoSize.y * (input.TexCoord.y - phase);
	float brightnessAdjustment = sin( (PI * 2 / videoSize.y) * currentScanlinePhase ) * strobeDepth + 1;
	
	//scanline with brightness + dark bar
	float scanMultiplier = (int)(videoSize.y * input.TexCoord.y) % 2; //can't use bitwise (reach profile == dx9) 
	if(scanMultiplier == 0)
	{
		scanMultiplier = scanClamp;
	}

	float4 scanlineColor = float4(1, 1, 1, 1) * (scanMultiplier + brightnessAdjustment);

    return input.Color * imageColor * scanlineColor * vignette; 
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

		//VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
