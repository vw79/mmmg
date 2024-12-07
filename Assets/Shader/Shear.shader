Shader "Unlit/Shear"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}  // Main texture
        _Color ("Color", Color) = (1, 1, 1, 1) // Tint color with alpha
        _TopLeft ("Top Left", Vector) = (-0.5, 0.5, 0, 1)     // Default Top Left
        _TopRight ("Top Right", Vector) = (0.5, 0.5, 0, 1)    // Default Top Right
        _BottomLeft ("Bottom Left", Vector) = (-0.5, -0.5, 0, 1) // Default Bottom Left
        _BottomRight ("Bottom Right", Vector) = (0.5, -0.5, 0, 1)  // Default Bottom Right
        _Scale ("Scale", Float) = 1.0 // Scaling factor
    }
    SubShader
    {
        Tags {"Queue"="Overlay" "IgnoreProjector"="True" "RenderType"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR; // Pass color to fragment
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;

            float4 _TopLeft;
            float4 _TopRight;
            float4 _BottomLeft;
            float4 _BottomRight;
            float _Scale; // Scaling factor

            v2f vert (appdata_t v)
            {
                v2f o;

                // Interpolate the vertex position based on UV coordinates
                float2 uv = v.uv;

                float4 pos = lerp(
                    lerp(_BottomLeft, _BottomRight, uv.x),
                    lerp(_TopLeft, _TopRight, uv.x),
                    uv.y
                );

                // Apply scaling
                pos.xy *= _Scale;

                o.vertex = UnityObjectToClipPos(pos);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                // Pass the color to the fragment shader
                o.color = _Color;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the texture and multiply by the color
                fixed4 texColor = tex2D(_MainTex, i.uv);
                return texColor * i.color;
            }
            ENDCG
        }
    }
}