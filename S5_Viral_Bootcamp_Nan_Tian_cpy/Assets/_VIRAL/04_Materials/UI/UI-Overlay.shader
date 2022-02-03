// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "UI/Unlit/Overlay"
{
    Properties
    {
        _MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }
    
    SubShader {
    
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
       
        ZTest Always
       
        Lighting Off
        Fog { Mode Off }
 
        Blend SrcAlpha OneMinusSrcAlpha
 
        Pass {
            Color [_Color]
            SetTexture [_MainTex] { combine texture * primary }
        }
    }

    
    FallBack "UI/Default"
}
