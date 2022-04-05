void MainLight_half(float3 WorldPos, out half3 Direction, out half3 Color, out half Attenuation)
{
#if SHADERGRAPH_PREVIEW
	Direction = half3(0.5, 0.5, 0);
	Color = 1;
	Attenuation = 1;
#else
#if SHADOWS_SCREEN
	half4 clipPos = TransformWorldToHClip(WorldPos);
	half4 shadowCoord = ComputeScreenPos(clipPos);
#else
	half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
#endif
	Light mainLight = GetMainLight(shadowCoord);
	Direction = mainLight.direction;
	Color = mainLight.color;
	Attenuation = mainLight.distanceAttenuation * mainLight.shadowAttenuation;
#endif
}

void AdditionalLights_half(int LightNum, float3 WorldPos, out half3 Direction, out half3 Color, out half Attenuation, out half LightCount)
{
#if SHADERGRAPH_PREVIEW
	Direction = half3(0.5, 0.5, 0);
	Color = 1;
	Attenuation = 1;
	LightCount = 1;
#else
	int pixelLightCount = GetAdditionalLightsCount();
	LightCount = pixelLightCount;
	Light light = GetAdditionalLight(LightNum, WorldPos);
	Direction = light.direction;
	Color = light.color * (light.distanceAttenuation * light.shadowAttenuation);
	Attenuation = light.distanceAttenuation * light.shadowAttenuation;
#endif
}