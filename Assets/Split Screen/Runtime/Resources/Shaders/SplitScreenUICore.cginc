#ifndef SPLIT_SCREEN
#define SPLIT_SCREEN

    sampler2D _Source1;
    float4 _Source1_TexelSize;

    sampler2D _Source2;
    float4 _Source2_TexelSize;

    float4 _CompareParams;
    float3 _TransitionData;
    #define TRANSITION_START_TIME _TransitionData.x
    #define TRANSITION_DURATION _TransitionData.y
    #define TRANSITION_TO_BOTH _TransitionData.z
    float3 _SplitLineColor;
    float2 _SplitLineCenter;


    struct appdata {
         float4 vertex : POSITION;
         float2 uv : TEXCOORD0;
    };

    struct v2f {
         float4 pos : SV_POSITION;
         float2 uv : TEXCOORD0;
    };

    v2f Vert(appdata v) {
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        o.uv = v.uv;
        return o;
    }

    float2 GetSeparatorLine(float2 uv) {
        // separator line + antialias
        float2 dd   = uv - _SplitLineCenter;
        float  co   = dot(_CompareParams.xy, dd);
        float  dist = distance( _CompareParams.xy * co, dd);
        float2 aa;
        aa.x        = saturate( (_CompareParams.w - dist) * _ScreenParams.y);
        aa.y        = dot(dd, _CompareParams.yz) < 0;
        return aa;
    }


    //#define DEBUG

	half4 FragComposeTwo(v2f i) : SV_Target {
        float2 uv     = i.uv;
        float4 color1 = tex2D(_Source1, uv);
        float4 color2 = tex2D(_Source2, uv);

        #if DEBUG
            if (uv.x < 0.15 && uv.y < 0.15) { 
                return tex2D(_Source1, uv / 0.15);
            } else if (uv.x > 0.85 && uv.y < 0.15) {
                return tex2D(_Source2, float2(uv.x - 0.85, uv.y) / 0.15);
            }
        #endif

        float2  aa = GetSeparatorLine(uv);
        // state transition
        float t = saturate( (_Time.y - TRANSITION_START_TIME) / TRANSITION_DURATION);
        if (TRANSITION_TO_BOTH) {
            aa.x *= t;
        } else {
            aa.x *= (1-t);
        }
        // split sides
        float4 pixel  = lerp(color1, color2, aa.y);
        pixel.rgb = lerp(pixel.rgb, _SplitLineColor.rgb, aa.x);
        return pixel;
	}



#endif // SPLIT_SCREEN