Shader "Custom/MirrorCrack"
{
    Properties
    {
        _MirrorTex   ("Mirror RenderTexture", 2D) = "white" {}
        _CrackTex    ("Crack Overlay (RGBA)",  2D) = "black" {}
        _CrackAmount ("Crack Progress", Range(0,1)) = 0
        _CenterUV    ("Crack Center UV", Vector) = (0.5, 0.5, 0, 0)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "MirrorCrackPass"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MirrorTex);   SAMPLER(sampler_MirrorTex);
            TEXTURE2D(_CrackTex);    SAMPLER(sampler_CrackTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MirrorTex_ST;
                float4 _CrackTex_ST;
                float  _CrackAmount;
                float4 _CenterUV;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv          = TRANSFORM_TEX(IN.uv, _MirrorTex);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Distancia desde el centro — controla expansión de grietas
                float dist = distance(IN.uv, _CenterUV.xy);
                float mask = step(dist, _CrackAmount * 0.8);

                half4 mirror = SAMPLE_TEXTURE2D(_MirrorTex, sampler_MirrorTex, IN.uv);
                half4 crack  = SAMPLE_TEXTURE2D(_CrackTex,  sampler_CrackTex,  IN.uv);

                // Mezcla reflejo + grietas según máscara
                half4 col = lerp(mirror, mirror * (1.0 - crack.r) + crack, mask * crack.a);
                return col;
            }
            ENDHLSL
        }
    }
}