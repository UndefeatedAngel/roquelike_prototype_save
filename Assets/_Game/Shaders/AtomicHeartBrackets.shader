Shader "UI/AtomicHeartBrackets"
{
    Properties
    {
        [PerRendererData] _MainTex ("Main Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _BracketColor ("Bracket Color", Color) = (1,0,0,1)
        _BracketWidth ("Bracket Width", Range(0, 0.2)) = 0.05
        _BracketSize ("Bracket Size", Range(0, 0.5)) = 0.2
        _BracketThickness ("Bracket Thickness", Range(0, 0.1)) = 0.02
        _AnimationSpeed ("Animation Speed", Range(0, 5)) = 1.0
        _PulseIntensity ("Pulse Intensity", Range(0, 1)) = 0.3
        
        [Header(Glow Effect)]
        _GlowIntensity ("Glow Intensity", Range(0, 5)) = 2.0
        _GlowWidth ("Glow Width", Range(0, 0.1)) = 0.02
        
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ PIXELSNAP_ON

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float2 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _BracketColor;
            float _BracketWidth;
            float _BracketSize;
            float _BracketThickness;
            float _AnimationSpeed;
            float _PulseIntensity;
            float _GlowIntensity;
            float _GlowWidth;

            // Функция для рисования одной скобки
            float DrawBracket(float2 uv, float2 center, float size, float thickness, int type)
            {
                // type: 0 = левая, 1 = правая, 2 = верхняя, 3 = нижняя
                
                float2 localUV = uv - center;
                float bracket = 0.0;
                
                if (type == 0) // Левая скобка [
                {
                    float vertical = step(abs(localUV.y), size) * step(-localUV.x, thickness) * step(localUV.x, 0);
                    float horizontal = step(abs(localUV.x), thickness) * step(localUV.y, -size + thickness) * step(-localUV.y - thickness, size);
                    bracket = max(vertical, horizontal);
                }
                else if (type == 1) // Правая скобка ]
                {
                    float vertical = step(abs(localUV.y), size) * step(localUV.x, thickness) * step(-localUV.x, 0);
                    float horizontal = step(abs(localUV.x), thickness) * step(localUV.y, -size + thickness) * step(-localUV.y - thickness, size);
                    bracket = max(vertical, horizontal);
                }
                else if (type == 2) // Верхняя скобка
                {
                    float horizontal = step(abs(localUV.x), size) * step(-localUV.y, thickness) * step(localUV.y, 0);
                    float vertical = step(abs(localUV.y), thickness) * step(localUV.x, -size + thickness) * step(-localUV.x - thickness, size);
                    bracket = max(horizontal, vertical);
                }
                else if (type == 3) // Нижняя скобка
                {
                    float horizontal = step(abs(localUV.x), size) * step(localUV.y, thickness) * step(-localUV.y, 0);
                    float vertical = step(abs(localUV.y), thickness) * step(localUV.x, -size + thickness) * step(-localUV.x - thickness, size);
                    bracket = max(horizontal, vertical);
                }
                
                return bracket;
            }

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                OUT.worldPos = IN.vertex.xy;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 color = tex2D(_MainTex, IN.texcoord) * IN.color;
                float2 uv = IN.texcoord;
                
                // Анимация пульсации
                float pulse = 1.0 + _PulseIntensity * sin(_Time.y * _AnimationSpeed * 3.14159);
                
                // Рисуем 4 скобки по углам
                float bracket = 0.0;
                
                // Левый верхний угол
                bracket = max(bracket, DrawBracket(uv, float2(_BracketWidth, _BracketWidth), _BracketSize * pulse, _BracketThickness, 0));
                bracket = max(bracket, DrawBracket(uv, float2(_BracketWidth, _BracketWidth), _BracketSize * pulse, _BracketThickness, 2));
                
                // Правый верхний угол
                bracket = max(bracket, DrawBracket(uv, float2(1.0 - _BracketWidth, _BracketWidth), _BracketSize * pulse, _BracketThickness, 1));
                bracket = max(bracket, DrawBracket(uv, float2(1.0 - _BracketWidth, _BracketWidth), _BracketSize * pulse, _BracketThickness, 2));
                
                // Левый нижний угол
                bracket = max(bracket, DrawBracket(uv, float2(_BracketWidth, 1.0 - _BracketWidth), _BracketSize * pulse, _BracketThickness, 0));
                bracket = max(bracket, DrawBracket(uv, float2(_BracketWidth, 1.0 - _BracketWidth), _BracketSize * pulse, _BracketThickness, 3));
                
                // Правый нижний угол
                bracket = max(bracket, DrawBracket(uv, float2(1.0 - _BracketWidth, 1.0 - _BracketWidth), _BracketSize * pulse, _BracketThickness, 1));
                bracket = max(bracket, DrawBracket(uv, float2(1.0 - _BracketWidth, 1.0 - _BracketWidth), _BracketSize * pulse, _BracketThickness, 3));
                
                // Добавляем свечение
                float glow = bracket * _GlowIntensity;
                
                // Смешиваем с основным цветом
                if (bracket > 0)
                {
                    color.rgb = lerp(color.rgb, _BracketColor.rgb, bracket);
                    color.a = max(color.a, bracket + glow);
                }
                
                return color;
            }
            ENDCG
        }
    }
}