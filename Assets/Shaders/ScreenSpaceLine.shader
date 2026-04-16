Shader "OXYZ/LineRenderer"
{
    Properties
    {
    }

    SubShader
    {
        Tags { "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float3 vertex : POSITION;
                float2 uv0 : TEXCOORD0; // t, side
                float4 uv1 : TEXCOORD1; // A
                float4 uv2 : TEXCOORD2; // B
                float4 uv3 : TEXCOORD3; // thickness, dash, gap
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float dashCoord : TEXCOORD1;
                float4 color : COLOR;
            };

            v2f vert(appdata v)
            {
                v2f o;

                float3 A = v.uv1.xyz;
                float3 B = v.uv2.xyz;

                float4 clipA = UnityObjectToClipPos(A);
                float4 clipB = UnityObjectToClipPos(B);

                float2 ndcA = clipA.xy / clipA.w;
                float2 ndcB = clipB.xy / clipB.w;

                float2 dir = normalize(ndcB - ndcA);
                float2 normal = float2(-dir.y, dir.x);

                float thickness = v.uv3.x;

                float pixelSize = thickness / _ScreenParams.y;

                float t = v.uv0.x;
                float side = v.uv0.y;

                float4 clip = lerp(clipA, clipB, t);

                float2 offset = normal * side * pixelSize;

                clip.xy += offset * clip.w;

                o.pos = clip;
                o.uv = v.uv0;
                o.color = v.color;

                // длина линии в пиксел€х
                float2 screenDir = (ndcB - ndcA) * _ScreenParams.xy;
                float length = length(screenDir);

                o.dashCoord = t * length;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float dashSize = i.color.a; // можно по-другому передать

                // если нужен dash Ч лучше вынести в uv3.z
                // здесь упрощЄнно

                return i.color;
            }

            ENDCG
        }
    }
}