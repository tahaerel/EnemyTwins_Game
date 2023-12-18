Shader "Kronnect/ScreenSplitPro/SplitScreenUI" {
Properties {
    [HideInInspector] _MainTex ("Texture", 2D) = "black" {}
    [HideInInspector] _Color ("Tint", Color) = (1,1,1,1)
}

SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
        }

        ZTest Always
        Cull Off
        Lighting Off
        ZWrite Off
        Blend One Zero

        Pass {
            CGPROGRAM
            #pragma vertex Vert
            #pragma fragment FragComposeTwo
            #include "SplitScreenUICore.cginc"
        ENDCG
        }
    }
}
