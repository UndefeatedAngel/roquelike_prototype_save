Shader "UI/GradientFadeLeftToRightAdvanced"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _GradientColor ("Gradient Color", Color) = (1,1,1,1)
        
        [Header(Gradient Settings)]
        _GradientPower ("Gradient Power", Range(0.1, 5)) = 1.0
        _GradientOffset ("Gradient Offset", Range(-1, 1)) = 0.0
        
        [Header(Pinch Settings)]
        _PinchStrength ("Pinch Strength", Range(0, 1)) = 0.0
        _PinchWidth ("Pinch Width", Range(0.1, 1)) = 0.5
        _PinchPosition ("Pinch Position", Range(0, 1)) = 1.0
        
        [Space]
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
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
            Name "Default"
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _GradientColor;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;

            // Параметры градиента
            float _GradientPower;
            float _GradientOffset;
            
            // Параметры сужения
            float _PinchStrength;
            float _PinchWidth;
            float _PinchPosition;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                OUT.color = v.color * _Color;
                return OUT;
            }

            // Функция для расчета сужения
            float ApplyPinch(float uvX, float pinchStrength, float pinchWidth, float pinchPosition)
            {
                // Рассчитываем расстояние от текущей позиции до позиции сужения
                float distanceToPinch = abs(uvX - pinchPosition);
                
                // Нормализуем расстояние относительно ширины сужения
                float normalizedDistance = saturate(distanceToPinch / pinchWidth);
                
                // Применяем кривую сужения (квадратичная для плавности)
                float pinchEffect = 1.0 - (normalizedDistance * normalizedDistance);
                
                // Умножаем на силу сужения
                return 1.0 - (pinchEffect * pinchStrength);
            }

            // Функция для расчета градиента с сужением
            float CalculateAdvancedGradient(float2 texcoord)
            {
                // Базовый градиент слева направо
                float baseGradient = 1.0 - (texcoord.x + _GradientOffset);
                baseGradient = saturate(baseGradient);
                
                // Применяем мощность градиента
                baseGradient = pow(baseGradient, _GradientPower);
                
                // Применяем сужение если оно активно
                if (_PinchStrength > 0)
                {
                    float pinchEffect = ApplyPinch(texcoord.x, _PinchStrength, _PinchWidth, _PinchPosition);
                    baseGradient *= pinchEffect;
                }
                
                return baseGradient;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
                
                // Рассчитываем расширенный градиент с сужением
                float gradient = CalculateAdvancedGradient(IN.texcoord);
                
                // Применяем градиент к альфа-каналу
                color.a *= gradient;
                
                // Применяем градиентный цвет
                color.rgb = lerp(color.rgb, _GradientColor.rgb, _GradientColor.a);
                
                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                return color;
            }
        ENDCG
        }
    }
}