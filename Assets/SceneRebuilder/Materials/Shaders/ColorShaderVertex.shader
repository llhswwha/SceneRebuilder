Shader "Unlit/ColorShaderVertex"
{
    Properties
    {
        [MainColor] _BaseColor("BaseColor", Color) = (1,1,1,1)
        // _MainColor("MainColor",color)=(1,1,1,1)
        _Scale("Scale",range(0,10))=2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "unitycg.cginc"
            fixed4 _BaseColor;
            float _Scale;
            struct v2f{
                float4 pos:POSITION;
                // float3 normal:TEXCOORD0;
                // float4 vertex:TEXCOORD1;
                float4 color:COLOR;
            };
            v2f vert(appdata_base v){
                v2f o;
                o.pos=UnityObjectToClipPos(v.vertex);
                // o.vertex=v.vertex;
                // o.normal=v.normal;

                float3 N=mul(v.normal,(float3x3)unity_WorldToObject);
                N=normalize(N); //UnityObjectToWorldNormal
                
                //WorldSpaceViewDir
                float3 worldPos=mul(unity_ObjectToWorld,v.vertex).xyz;
                float3 V=_WorldSpaceCameraPos-worldPos;
                V=normalize(V);

                //float bright=1.0-saturate(dot(N,V));
                float bright=saturate(dot(N,V));
                bright=pow(bright,_Scale);
                o.color=_BaseColor*bright;

                return o;
            }
            fixed4 frag(v2f IN):COLOR
            {
                
                return IN.color;
            }
            ENDCG
        }
    }
}
