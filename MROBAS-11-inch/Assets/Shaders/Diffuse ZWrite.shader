Shader "Transparent/Diffuse ZWrite" {
    Properties{
        _Color("Main Color", Color) = (1,1,1,1)
        _MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
        //_Transparency("Transparency", Range(0.0,1.0)) = 1.0
    }
        SubShader{
            Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout"}
            LOD 100

        // extra pass that renders to depth buffer only
        Pass {
            ZWrite On
            Blend SrcAlpha OneminusSrcAlpha
            ColorMask 0
        }

        // paste in forward rendering passes from Transparent/Diffuse
        UsePass "Transparent/Diffuse/FORWARD"
    }
        Fallback "Transparent/VertexLit"
}