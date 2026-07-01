Shader "Custom/StarBackground"
{
    Properties
    {
        _MainTex ("Star Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "Queue"="Background"
            "RenderType"="Transparent"
        }

        Cull Front
        Lighting Off
        ZWrite Off
        ZTest LEqual

        Pass
        {
            SetTexture [_MainTex]
            {
                ConstantColor [_Color]
                Combine texture * constant
            }
        }
    }
}