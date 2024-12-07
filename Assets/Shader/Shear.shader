Shader "Unlit/Shear"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _TopLeft ("Top Left", Vector) = (-0.5, 0.5, 0, 1)
        _TopRight ("Top Right", Vector) = (0.5, 0.5, 0, 1)
        _BottomLeft ("Bottom Left", Vector) = (-0.5, -0.5, 0, 1)
        _BottomRight ("Bottom Right", Vector) = (0.5, -0.5, 0, 1)
        _Scale ("Scale", Float) = 1.0
        _CanvasAlpha ("Canvas Alpha", Float) = 1.0 // Add a property for CanvasGroup alpha
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
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _Color;
            float _CanvasAlpha; // Add canvas alpha
            float4 _TopLeft;
            float4 _TopRight;
            float4 _BottomLeft;
            float4 _BottomRight;
            float _Scale;

            v2f vert (appdata_t v)
            {
                v2f o;

                // Calculate the UV-interpolated position
                float2 uv = v.uv;

                float4 pos = lerp(
                    lerp(_BottomLeft, _BottomRight, uv.x),
                    lerp(_TopLeft, _TopRight, uv.x),
                    uv.y
                );

                // Calculate the center of the four corners
                float2 center = 0.25 * (
                    _TopLeft.xy +
                    _TopRight.xy +
                    _BottomLeft.xy +
                    _BottomRight.xy
                );

                // Apply scaling relative to the center
                pos.xy = center + (pos.xy - center) * _Scale;

                o.vertex = UnityObjectToClipPos(pos);
                o.uv = uv; // Pass UV coordinates directly

                // Pass the color to the fragment shader
                o.color = _Color;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the texture and multiply by the color and canvas alpha
                fixed4 texColor = tex2D(_MainTex, i.uv);
                texColor.a *= i.color.a * _CanvasAlpha; // Modulate alpha by CanvasGroup.alpha
                return texColor * i.color;
            }
            ENDCG
        }
    }
}