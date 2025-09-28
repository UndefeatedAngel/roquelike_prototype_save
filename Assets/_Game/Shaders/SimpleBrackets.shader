Shader "UI/SimpleBrackets"
{
    Properties
    {
        [PerRendererData] _MainTex ("Main Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _BracketColor ("Bracket Color", Color) = (1,0,0,1)
        _BracketThickness ("Thickness", Range(0, 0.1)) = 0.02
        _BracketLength ("Length", Range(0, 0.3)) = 0.15
        _PulseSpeed ("Pulse Speed", Range(0, 3)) = 1.0
    }

    SubShader
    {
        // ... (такие же настройки как выше)

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _BracketColor;
            float _BracketThickness;
            float _BracketLength;
            float _PulseSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                float2 uv = i.uv;
                
                // Пульсация
                float pulse = 0.5 + 0.5 * sin(_Time.y * _PulseSpeed);
                
                // Угловые скобки
                float bracket = 0.0;
                
                // Левый верхний
                if (uv.x < _BracketThickness && uv.y < _BracketLength * pulse) bracket = 1.0;
                if (uv.y < _BracketThickness && uv.x < _BracketLength * pulse) bracket = 1.0;
                
                // Правый верхний
                if (uv.x > 1.0 - _BracketThickness && uv.y < _BracketLength * pulse) bracket = 1.0;
                if (uv.y < _BracketThickness && uv.x > 1.0 - _BracketLength * pulse) bracket = 1.0;
                
                // Левый нижний
                if (uv.x < _BracketThickness && uv.y > 1.0 - _BracketLength * pulse) bracket = 1.0;
                if (uv.y > 1.0 - _BracketThickness && uv.x < _BracketLength * pulse) bracket = 1.0;
                
                // Правый нижний
                if (uv.x > 1.0 - _BracketThickness && uv.y > 1.0 - _BracketLength * pulse) bracket = 1.0;
                if (uv.y > 1.0 - _BracketThickness && uv.x > 1.0 - _BracketLength * pulse) bracket = 1.0;
                
                if (bracket > 0)
                {
                    col.rgb = _BracketColor.rgb;
                    col.a = _BracketColor.a;
                }
                
                return col;
            }
            ENDCG
        }
    }
}