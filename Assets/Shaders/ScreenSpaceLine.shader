Shader "Custom/ScreenSpaceLine"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Thickness ("Thickness (px)", Float) = 2
        _DashSize ("Dash Size", Float) = 10
        _GapSize ("Gap Size", Float) = 5
        _UseDash ("Use Dash", Float) = 0
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

            float4 _Color;
            float _Thickness;
            float _DashSize;
            float _GapSize;
            float _UseDash;

            struct appdata
            {
                float3 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float linePos : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;

                float4 clip = UnityObjectToClipPos(v.vertex);

                // экранное направление (используем uv.x как позицию вдоль линии)
                float2 dir = float2(1,0);

                // перпендикул€р
                float2 normal = float2(-dir.y, dir.x);

                float pixelWidth = _Thickness / _ScreenParams.y;

                float2 offset = normal * v.uv.y * pixelWidth;

                clip.xy += offset * clip.w;

                o.pos = clip;
                o.uv = v.uv;
                o.linePos = v.uv.x;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                if (_UseDash > 0.5)
                {
                    float total = _DashSize + _GapSize;
                    float m = fmod(i.linePos * 100, total);

                    if (m > _DashSize)
                        discard;
                }

                return _Color;
            }
            ENDCG
        }
    }
}