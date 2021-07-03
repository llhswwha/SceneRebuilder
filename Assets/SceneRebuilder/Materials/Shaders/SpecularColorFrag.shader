Shader "Unlit/SpecularColorFrag"
{
    Properties
    {
        _MainColor("MainColor",color)=(1,1,1,1)
        _SpecularColor("SpecularColor",color)=(1,1,1,1)
        _Shininess("Sphininess",range(1,8))=4

        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            //#pragma multi_compile_fwdbase
            #pragma vertex vert
            #pragma fragment frag
            #include "unitycg.cginc"
            #include "Lighting.cginc"
            float4 _MainColor;
            float4 _SpecularColor;
            float _Shininess;
            struct v2f 
            {
                float4 pos:POSITION;
                float3 normal:TEXCOORD0;
                fixed4 vertex:COLOR;
            };
            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos=UnityObjectToClipPos(v.vertex);
                o.normal=v.normal;//法向量
                o.vertex=v.vertex;//原始坐标
                return o;
            }
            fixed4 frag(v2f IN):COLOR
            {
                //fixed4 color=UNITY_LIGHTMODEL_AMBIENT;//环境光

                //diffuse Color
                float3 N= UnityObjectToWorldNormal(IN.normal);
                float3 L= normalize(WorldSpaceLightDir(IN.vertex));
                float diffuseScale=saturate(dot(N,L));
                fixed4 color=_MainColor*_LightColor0*diffuseScale;

                // //Specular Color
                // float3 V= normalize(WorldSpaceViewDir(IN.vertex));
                // float3 R= 2*dot(N,L)*N-L;
                // float specularScale=saturate(dot(R,V));
                // color+=_SpecularColor*pow(specularScale,_Shininess);

                // //compute 4 points lighting;
                // float3 wpos=mul(unity_ObjectToWorld,IN.vertex).xyz;
                // color.rgb+=Shade4PointLights(unity_4LightPosX0,unity_4LightPosY0,unity_4LightPosZ0,
                //     unity_LightColor[0].rgb,unity_LightColor[1].rgb,unity_LightColor[2].rgb,unity_LightColor[3].rgb,
                //     unity_4LightAtten0,wpos,N);//+点光源

                return color;
            }
            ENDCG
        }
    }
}
