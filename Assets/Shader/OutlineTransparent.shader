Shader "Lit/Outline Shader"
{
    Properties {
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", float) = 1.0
    }
    SubShader {
        Tags { "Queue" = "Overlay" }
        Pass {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            float4 _OutlineColor;
            float _OutlineWidth;

            v2f vert_img(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv);

                float2 size = _ScreenParams.xy;
                float depthCenter = tex2D(_CameraDepthTexture, i.uv).r;
                float outlineScale = _OutlineWidth / 1000.0;

                float depthN = tex2D(_CameraDepthTexture, i.uv + float2(0, -outlineScale)).r;
                float depthS = tex2D(_CameraDepthTexture, i.uv + float2(0, outlineScale)).r;
                float depthE = tex2D(_CameraDepthTexture, i.uv + float2(outlineScale, 0)).r;
                float depthW = tex2D(_CameraDepthTexture, i.uv + float2(-outlineScale, 0)).r;

                bool isOutline = abs(depthCenter - depthN) > 0.001 || abs(depthCenter - depthS) > 0.001 || 
                                 abs(depthCenter - depthE) > 0.001 || abs(depthCenter - depthW) > 0.001;

                return isOutline ? _OutlineColor : col;
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}