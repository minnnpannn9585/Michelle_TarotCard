Shader "Custom/SpriteCursorReveal"
{
    Properties
    {
        [PerRendererData] _MainTex ("Dark Form", 2D) = "white" {}
        _LightTex ("Light Form", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _CursorPos ("Cursor World Position", Vector) = (0,0,0,0)
        _Radius ("Reveal Radius", Float) = 1.75
        _Softness ("Edge Softness", Float) = 1.1
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    CGINCLUDE
    #pragma vertex vert
    #pragma fragment frag
    #pragma target 2.0
    #pragma multi_compile_instancing
    #pragma multi_compile _ PIXELSNAP_ON
    #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
    #include "UnityCG.cginc"

    struct appdata_t
    {
        float4 vertex : POSITION;
        float4 color : COLOR;
        float2 texcoord : TEXCOORD0;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct v2f
    {
        float4 vertex : SV_POSITION;
        fixed4 color : COLOR;
        float2 texcoord : TEXCOORD0;
        float2 worldPos : TEXCOORD1;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    sampler2D _MainTex;
    sampler2D _LightTex;
    sampler2D _AlphaTex;
    float4 _MainTex_ST;
    float _EnableExternalAlpha;
    fixed4 _Color;
    fixed4 _RendererColor;
    float4 _CursorPos;
    float _Radius;
    float _Softness;

    v2f vert(appdata_t IN)
    {
        v2f OUT;
        UNITY_SETUP_INSTANCE_ID(IN);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

        OUT.vertex = UnityObjectToClipPos(IN.vertex);
        OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
        OUT.color = IN.color * _Color * _RendererColor;
        OUT.worldPos = mul(unity_ObjectToWorld, IN.vertex).xy;

        #ifdef PIXELSNAP_ON
        OUT.vertex = UnityPixelSnap(OUT.vertex);
        #endif

        return OUT;
    }

    fixed4 SampleSpriteTexture(float2 uv)
    {
        fixed4 color = tex2D(_MainTex, uv);
        #if ETC1_EXTERNAL_ALPHA
        fixed4 alpha = tex2D(_AlphaTex, uv);
        color.a = lerp(color.a, alpha.r, _EnableExternalAlpha);
        #endif
        return color;
    }

    fixed4 frag(v2f IN) : SV_Target
    {
        fixed4 darkCol = SampleSpriteTexture(IN.texcoord);
        fixed4 lightCol = tex2D(_LightTex, IN.texcoord);

        float dist = distance(IN.worldPos, _CursorPos.xy);
        float inner = max(_Radius - _Softness, 0.0);
        float reveal = 1.0 - smoothstep(inner, max(_Radius, inner + 0.0001), dist);

        fixed4 c = lerp(darkCol, lightCol, reveal) * IN.color;
        c.rgb *= c.a;
        return c;
    }
    ENDCG

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
            "RenderPipeline" = "UniversalPipeline"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            Name "Universal2D"
            Tags { "LightMode" = "Universal2D" }
            CGPROGRAM
            ENDCG
        }

        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode" = "UniversalForward" }
            CGPROGRAM
            ENDCG
        }
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            ENDCG
        }
    }
}
