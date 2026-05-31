Shader "Custom/MoveRangeShader"
{
    Properties
    {   
        _MainTex ("Base Texture", 2D) = "white" {}
        _TileColor ("Tile Color", Color) = (1,0.2,0.4,0.6)
        _Density ("Grid Density", Float) = 8
    }


    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            float4 _TileColor;
            float _Density;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 baseCol = tex2D(_MainTex, i.uv);

                // ===== 1. TẠO VIỀN TRONG TILE =====
                float border = 0.08; // độ dày viền (0.05 - 0.12)

                // Nếu gần mép tile → trong suốt
                if (i.uv.x < border || i.uv.x > 1 - border ||
                    i.uv.y < border || i.uv.y > 1 - border)
                {
                    return float4(0,0,0,0);
                }

                // ===== 2. SCALE UV VÀO BÊN TRONG (TRÁNH DÍNH Ô) =====
                float2 innerUV = (i.uv - border) / (1 - border * 2);

                float2 uv = innerUV * _Density;
                float2 grid = frac(uv);

                float diag = abs(frac(grid.x + grid.y) - 0.5) * 2;
                float pattern = step(0.5, diag);

                float alpha = pattern * _TileColor.a * baseCol.a;

                return fixed4(_TileColor.rgb, alpha);
            }


            ENDCG
        }
    }
}
