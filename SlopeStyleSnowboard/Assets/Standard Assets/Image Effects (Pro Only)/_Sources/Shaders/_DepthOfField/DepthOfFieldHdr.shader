 Shader "Hidden/Dof/DepthOfFieldHdr" {
	Properties {
		_MainTex ("-", 2D) = "" {}
		_MainTex2 ("-", 2D) = "" {}
		_TapLowA ("-", 2D) = "" {}
		_TapLowB ("-", 2D) = "" {}
		_TapLowC ("-", 2D) = "" {}
		_Reference ("-", 2D) = "" {}
	}

	CGINCLUDE
	
	#include "UnityCG.cginc"
	
	struct v2f {
		half4 pos : POSITION;
		half2 uv1 : TEXCOORD0;
	};
	
	struct v2fDofApply {
		half4 pos : POSITION;
		half2 uv : TEXCOORD0;
	};
	
	struct v2fRadius {
		half4 pos : POSITION;
		half2 uv : TEXCOORD0;
		half4 uv1[4] : TEXCOORD1;
	};
	
	struct v2fDown {
		half4 pos : POSITION;
		half2 uv0 : TEXCOORD0;
		half2 uv[2] : TEXCOORD1;
	};	 
	
	struct v2fBlur {
		half4 pos : POSITION;
		half2 uv : TEXCOORD0;
		half4 uv01 : TEXCOORD1;
		half4 uv23 : TEXCOORD2;
		half4 uv45 : TEXCOORD3;
		half4 uv67 : TEXCOORD4;
		half4 uv89 : TEXCOORD5;
	};	
	
	struct mrtOut {
		half4 color0 : COLOR0;
		half4 color1 : COLOR1;
	};
			
	sampler2D _MainTex;
	sampler2D _MainTex2;
	sampler2D _TapLowA;	
	sampler2D _TapLowB;
	sampler2D _TapLowC;
	sampler2D _CameraDepthTexture;
	sampler2D _Reference;
	
	half4 _CurveParams;
	uniform float4 _MainTex_TexelSize;
	uniform float2 _InvRenderTargetSize;
	
	uniform half4 _Offsets;
	uniform half4 _Offsets2;

	#define COC_SMALL_VALUE (0.0001)
	#define REJECT_THRESHHOLD 0.5
	#define VEC_ONE (half4(1,1,1,1))
	
	void RejectionWeights ( half4 sampleA, half4 sampleB, half4 sampleC, half4 sampleD, half4 sampleE,
				half4 sampleF, half4 sampleG, half4 sampleH, half4 sampleI, 
				out half4 weightsA, out half4 weightsB )
	{
		weightsA = saturate(5.0 * half4(sampleA.aa-(half2(sampleB.a, sampleC.a)+0.5*sampleA.a), (sampleA.aa-half2(sampleD.a, sampleE.a)+0.35*sampleA.a)));
		weightsB = saturate(10.0 * half4(sampleA.aa-(half2(sampleF.a, sampleG.a)+0.20*sampleA.a), (sampleA.aa-half2(sampleH.a, sampleI.a)+0.1*sampleA.a)));

		//weightsA = saturate(2.0 * half4(sampleA.aa-(half2(sampleB.a, sampleC.a)+0.75*sampleA.a), (sampleA.aa-half2(sampleD.a, sampleE.a)+0.5*sampleA.a)));
		//weightsB = saturate(5.0 * half4(sampleA.aa-(half2(sampleF.a, sampleG.a)+0.30*sampleA.a), (sampleA.aa-half2(sampleH.a, sampleI.a)+0.2*sampleA.a)));
				
		//weightsA = saturate(half4((half2(sampleB.a, sampleC.a)-0.15)*1.5, (half2(sampleD.a, sampleE.a)-0.35)*1.5 ));
		//weightsB = saturate(half4((half2(sampleF.a, sampleG.a)-0.65)*2.5, (half2(sampleH.a, sampleI.a)-0.85)*5.0 ));
		
		//weightsA = half4(step(0.1, sampleB.a), step(0.1, sampleC.a), step(0.3, sampleD.a), step(0.3, sampleE.a));
		//weightsB = half4(step(0.6, sampleF.a), step(0.6, sampleG.a), step(0.85, sampleH.a), step(0.85, sampleI.a));
	}

	v2f vert( appdata_img v ) {
		v2f o;
		o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
		o.uv1.xy = v.texcoord.xy;
		return o;
	} 

	v2fBlur vertBlur (appdata_img v) {
		v2fBlur o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv.xy = v.texcoord.xy;
		o.uv01 =  v.texcoord.xyxy + _Offsets.xyxy * half4(1,1, 2,2);//
		o.uv23 =  v.texcoord.xyxy + _Offsets.xyxy * half4(3,3, 4,4);// * 3.0;
		o.uv45 =  v.texcoord.xyxy + _Offsets.xyxy * half4(5,5, 6,6);// * 3.0;
		o.uv67 =  v.texcoord.xyxy + _Offsets.xyxy * half4(7,7, 8,8);// * 4.0;
		o.uv89 =  v.texcoord.xyxy + _Offsets.xyxy * half4(9,9, 10,10);// * 5.0;
		return o;  
	}

	v2fBlur vertBlurPlusMinus (appdata_img v) {
		v2fBlur o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv.xy = v.texcoord.xy;
		o.uv01 =  v.texcoord.xyxy + _Offsets.xyxy * half4(1,1, -1,-1);
		o.uv23 =  v.texcoord.xyxy + _Offsets.xyxy * half4(2,2, -2,-2);// * 3.0;
		o.uv45 =  v.texcoord.xyxy + _Offsets.xyxy * half4(3,3, -3,-3);// * 3.0;
		o.uv67 =  v.texcoord.xyxy + _Offsets.xyxy * half4(4,4, -4,-4);// * 4.0;
		o.uv89 =  v.texcoord.xyxy + _Offsets.xyxy * half4(5,5, -5,-5);// * 5.0;
		return o;  
	}

	v2fRadius vertWithRadius( appdata_img v ) {
		v2fRadius o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv.xy = v.texcoord.xy;

		const half2 blurOffsets[4] = {
			half2(-0.5, +1.5),
			half2(+0.5, -1.5),
			half2(+1.5, +0.5),
			half2(-1.5, -0.5)
		}; 	
				
		o.uv1[0].xy = v.texcoord.xy + 5.0 * _MainTex_TexelSize.xy * blurOffsets[0];
		o.uv1[1].xy = v.texcoord.xy + 5.0 * _MainTex_TexelSize.xy * blurOffsets[1];
		o.uv1[2].xy = v.texcoord.xy + 5.0 * _MainTex_TexelSize.xy * blurOffsets[2];
		o.uv1[3].xy = v.texcoord.xy + 5.0 * _MainTex_TexelSize.xy * blurOffsets[3];
		
		o.uv1[0].zw = v.texcoord.xy + 3.0 * _MainTex_TexelSize.xy * blurOffsets[0];
		o.uv1[1].zw = v.texcoord.xy + 3.0 * _MainTex_TexelSize.xy * blurOffsets[1];
		o.uv1[2].zw = v.texcoord.xy + 3.0 * _MainTex_TexelSize.xy * blurOffsets[2];
		o.uv1[3].zw = v.texcoord.xy + 3.0 * _MainTex_TexelSize.xy * blurOffsets[3];
		
		return o;
	} 
	
	v2fDofApply vertDofApply( appdata_img v ) {
		v2fDofApply o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv.xy = v.texcoord.xy;
		return o;
	} 	
		
	v2fDown vertDownsampleWithCocConserve(appdata_img v) {
		v2fDown o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);	
		o.uv0.xy = v.texcoord.xy;
		o.uv[0].xy = v.texcoord.xy + half2(-1.0,-1.0) * _InvRenderTargetSize;
		o.uv[1].xy = v.texcoord.xy + half2(1.0,-1.0) * _InvRenderTargetSize;		
		return o; 
	} 
	
	half4 BokehPrereqs (sampler2D tex, half4 uv1[4], half4 center, half considerCoc) {		
		
		// @NOTE 1:
		// we are checking for 3 things in order to create a bokeh.
		// goal is to get the highest bang for the buck.
		// 1.) contrast/frequency should be very high (otherwise bokeh mostly unvisible)
		// 2.) luminance should be high
		// 3.) no occluder nearby (stored in alpha channel)
		
		// @NOTE 2: about the alpha channel in littleBlur:
		// the alpha channel stores an heuristic on how likely it is 
		// that there is no bokeh occluder nearby.
		// if we didn't' check for that, we'd get very noise bokeh
		// popping because of the sudden contrast changes

		half4 sampleA = tex2D(tex, uv1[0].zw);
		half4 sampleB = tex2D(tex, uv1[1].zw);
		half4 sampleC = tex2D(tex, uv1[2].zw);
		half4 sampleD = tex2D(tex, uv1[3].zw);
		
		half4 littleBlur = 0.125 * (sampleA + sampleB + sampleC + sampleD);
		
		sampleA = tex2D(tex, uv1[0].xy);
		sampleB = tex2D(tex, uv1[1].xy);
		sampleC = tex2D(tex, uv1[2].xy);
		sampleD = tex2D(tex, uv1[3].xy);		

		littleBlur += 0.125 * (sampleA + sampleB + sampleC + sampleD);
				
		littleBlur = lerp (littleBlur, center, saturate(100.0 * considerCoc * abs(littleBlur.a - center.a)));
				
		return littleBlur;
	}	
	
	half4 fragDownsampleWithCocConserve(v2fDown i) : COLOR {
		half2 rowOfs[4];   
		
  		rowOfs[0] = half2(0.0, 0.0);  
  		rowOfs[1] = half2(0.0, _InvRenderTargetSize.y);  
  		rowOfs[2] = half2(0.0, _InvRenderTargetSize.y) * 2.0;  
  		rowOfs[3] = half2(0.0, _InvRenderTargetSize.y) * 3.0; 
  		
  		half4 color = tex2D(_MainTex, i.uv0.xy); 	
			
		half4 sampleA = tex2D(_MainTex, i.uv[0].xy + rowOfs[0]);  
		half4 sampleB = tex2D(_MainTex, i.uv[1].xy + rowOfs[0]);  
		half4 sampleC = tex2D(_MainTex, i.uv[0].xy + rowOfs[2]);  
		half4 sampleD = tex2D(_MainTex, i.uv[1].xy + rowOfs[2]);  
		
		color += sampleA + sampleB + sampleC + sampleD;
		color *= 0.2;
		
		// @NOTE we are doing max on the alpha channel for 2 reasons:
		// 1) foreground blur likes a slightly bigger radius
		// 2) otherwise we get an ugly outline between high blur- and medium blur-areas
		// drawback: we get a little bit of color bleeding  		
		
		color.a = max(max(sampleA.a, sampleB.a), max(sampleC.a, sampleD.a));
  		
		return color;
	}
	
	half4 fragBlur9Tap (v2fBlur i) : COLOR 
	{
		half4 blurredColor = half4 (0,0,0,0);

		half4 sampleA = tex2D(_MainTex, i.uv.xy);
		half4 sampleB = tex2D(_MainTex, i.uv01.xy);
		half4 sampleC = tex2D(_MainTex, i.uv01.zw);
		half4 sampleD = tex2D(_MainTex, i.uv23.xy);
		half4 sampleE = tex2D(_MainTex, i.uv23.zw);
		half4 sampleF = tex2D(_MainTex, i.uv45.xy);
		half4 sampleG = tex2D(_MainTex, i.uv45.zw);
		half4 sampleH = tex2D(_MainTex, i.uv67.xy);
		half4 sampleI = tex2D(_MainTex, i.uv67.zw);
		half4 sampleJ = tex2D(_MainTex, i.uv89.xy);
		half4 sampleK = tex2D(_MainTex, i.uv89.zw);
										
		blurredColor += sampleA;
		blurredColor += sampleB;
		blurredColor += sampleC; 
		blurredColor += sampleD; 
		blurredColor += sampleE; 
		blurredColor += sampleF; 
		blurredColor += sampleG; 
		blurredColor += sampleH; 
		blurredColor += sampleI; 
		blurredColor += sampleJ; 
		blurredColor += sampleK; 
								
		blurredColor = blurredColor / 9;
		
		return blurredColor;
	}	
	
	// probably the ugliest of our DOF blurs (but it's cheap!)
	// TODO: consider the # of taps that's really needed here
	// TODO: remove this rejection weights and make proper gauss
	// TODO: add a poisson disc blur type (nicer, variable radius)
	half4 fragBlur9TapGauss (v2fBlur i) : COLOR 
	{
		half4 blurredColor = half4 (0,0,0,0);

		half4 sampleA = tex2D(_MainTex, i.uv.xy);
		half4 sampleB = tex2D(_MainTex, i.uv01.xy);
		half4 sampleC = tex2D(_MainTex, i.uv01.zw);
		half4 sampleD = tex2D(_MainTex, i.uv23.xy);
		half4 sampleE = tex2D(_MainTex, i.uv23.zw);
		half4 sampleF = tex2D(_MainTex, i.uv45.xy);
		half4 sampleG = tex2D(_MainTex, i.uv45.zw);
		half4 sampleH = tex2D(_MainTex, i.uv67.xy);
		half4 sampleI = tex2D(_MainTex, i.uv67.zw);
			
		half4 weightsA = half4(1,1,1,1);
		half4 weightsB = half4(1,1,1,1);
		//RejectionWeights(sampleA,sampleB,sampleC,sampleD,sampleE,sampleF,sampleG,sampleH,sampleI, weightsA, weightsB);
					
		blurredColor += sampleA * 1.0;
		blurredColor += sampleB * weightsA.x;
		blurredColor += sampleC * weightsA.y; 
		blurredColor += sampleD * weightsA.z; 
		blurredColor += sampleE * weightsA.w;
		blurredColor += sampleF * weightsB.x;
		blurredColor += sampleG * weightsB.y;
		blurredColor += sampleH * weightsB.z;
		blurredColor += sampleI * weightsB.w;

		return blurredColor / (dot(weightsA, VEC_ONE) + dot(weightsB, VEC_ONE) + 1);
	}		
	
	half4 fragBlurWithOneThirdApply(v2fBlur i) : COLOR 
	{
		half4 blurredColor = half4 (0,0,0,0);

		half4 sampleA = tex2D(_MainTex, i.uv.xy);
		half4 sampleB = tex2D(_MainTex, i.uv01.xy);
		half4 sampleC = tex2D(_MainTex, i.uv01.zw);
		half4 sampleD = tex2D(_MainTex, i.uv23.xy);
		half4 sampleE = tex2D(_MainTex, i.uv23.zw);
		half4 sampleF = tex2D(_MainTex, i.uv45.xy);
		half4 sampleG = tex2D(_MainTex, i.uv45.zw);
		half4 sampleH = tex2D(_MainTex, i.uv67.xy);
		half4 sampleI = tex2D(_MainTex, i.uv67.zw);
		half4 sampleJ = tex2D(_MainTex, i.uv89.xy);
		half4 sampleK = tex2D(_MainTex, i.uv89.zw);
										
		blurredColor += sampleA;
		blurredColor += sampleB;
		blurredColor += sampleC; 
		blurredColor += sampleD; 
		blurredColor += sampleE; 
		blurredColor += sampleF; 
		blurredColor += sampleG; 
		blurredColor += sampleH; 
		blurredColor += sampleI; 
		blurredColor += sampleJ; 
		blurredColor += sampleK; 
								
		blurredColor = blurredColor / 11;
		
		return blurredColor / 3;
	}	
	
	mrtOut frag2BlurOpt2XPass1(v2fBlur i) 
	{
		mrtOut fragOut;
	
		fragOut.color0 = half4 (0,0,0,0);
		fragOut.color1 = half4 (0,0,0,0);

		// BLUR 1
		half4 sampleA = tex2D(_MainTex, i.uv.xy);
		half4 sampleB = tex2D(_MainTex, i.uv01.xy);
		half4 sampleC = tex2D(_MainTex, i.uv01.zw);
		half4 sampleD = tex2D(_MainTex, i.uv23.xy);
		half4 sampleE = tex2D(_MainTex, i.uv23.zw);
		half4 sampleF = tex2D(_MainTex, i.uv45.xy);
		half4 sampleG = tex2D(_MainTex, i.uv45.zw);
		half4 sampleH = tex2D(_MainTex, i.uv67.xy);
		half4 sampleI = tex2D(_MainTex, i.uv67.zw);
								
		fragOut.color0 += sampleA;
		fragOut.color0 += sampleB;
		fragOut.color0 += sampleC; 
		fragOut.color0 += sampleD; 
		fragOut.color0 += sampleE; 
		fragOut.color0 += sampleF; 
		fragOut.color0 += sampleG; 
		fragOut.color0 += sampleH; 
		fragOut.color0 += sampleI; 
		fragOut.color0 /= 9;
		
		// BLUR 2
		sampleA = tex2D(_MainTex, i.uv.xy);
		sampleB = tex2D(_MainTex, i.uv.xy + _Offsets2.xy*1.0f);
		sampleC = tex2D(_MainTex, i.uv.xy + _Offsets2.xy*2.0f);
		sampleD = tex2D(_MainTex, i.uv.xy + _Offsets2.xy*3.0f);
		sampleE = tex2D(_MainTex, i.uv.xy + _Offsets2.xy*4.0f);
		sampleF = tex2D(_MainTex, i.uv.xy + _Offsets2.xy*5.0f);
		sampleG = tex2D(_MainTex, i.uv.xy + _Offsets2.xy*6.0f);
		sampleH = tex2D(_MainTex, i.uv.xy + _Offsets2.xy*7.0f);
		sampleI = tex2D(_MainTex, i.uv.xy + _Offsets2.xy*8.0f);
								
		fragOut.color1 += sampleA;
		fragOut.color1 += sampleB;
		fragOut.color1 += sampleC; 
		fragOut.color1 += sampleD; 
		fragOut.color1 += sampleE; 
		fragOut.color1 += sampleF; 
		fragOut.color1 += sampleG; 
		fragOut.color1 += sampleH; 
		fragOut.color1 += sampleI; 
		fragOut.color1 /= 9;
		
		return fragOut;
	}	

	mrtOut frag2BlurOpt2XPass3(v2fBlur i) 
	{
		mrtOut fragOut;
	
		fragOut.color0 = half4 (0,0,0,0);
		fragOut.color1 = half4 (0,0,0,0);

		// BLUR 1
		half4 sampleA = tex2D(_MainTex, i.uv.xy);
		half4 sampleB = tex2D(_MainTex, i.uv01.xy);
		half4 sampleC = tex2D(_MainTex, i.uv01.zw);
		half4 sampleD = tex2D(_MainTex, i.uv23.xy);
		half4 sampleE = tex2D(_MainTex, i.uv23.zw);
		half4 sampleF = tex2D(_MainTex, i.uv45.xy);
		half4 sampleG = tex2D(_MainTex, i.uv45.zw);
		half4 sampleH = tex2D(_MainTex, i.uv67.xy);
		half4 sampleI = tex2D(_MainTex, i.uv67.zw);
								
		fragOut.color0 += sampleA;
		fragOut.color0 += sampleB;
		fragOut.color0 += sampleC; 
		fragOut.color0 += sampleD; 
		fragOut.color0 += sampleE; 
		fragOut.color0 += sampleF; 
		fragOut.color0 += sampleG; 
		fragOut.color0 += sampleH; 
		fragOut.color0 += sampleI; 
		fragOut.color0 /= 9;
		
		// BLUR 2
		sampleA = tex2D(_MainTex2, i.uv.xy);
		sampleB = tex2D(_MainTex2, i.uv.xy + _Offsets2.xy*1.0f);
		sampleC = tex2D(_MainTex2, i.uv.xy + _Offsets2.xy*2.0f);
		sampleD = tex2D(_MainTex2, i.uv.xy + _Offsets2.xy*3.0f);
		sampleE = tex2D(_MainTex2, i.uv.xy + _Offsets2.xy*4.0f);
		sampleF = tex2D(_MainTex2, i.uv.xy + _Offsets2.xy*5.0f);
		sampleG = tex2D(_MainTex2, i.uv.xy + _Offsets2.xy*6.0f);
		sampleH = tex2D(_MainTex2, i.uv.xy + _Offsets2.xy*7.0f);
		sampleI = tex2D(_MainTex2, i.uv.xy + _Offsets2.xy*8.0f);
								
		fragOut.color1 += sampleA;
		fragOut.color1 += sampleB;
		fragOut.color1 += sampleC; 
		fragOut.color1 += sampleD; 
		fragOut.color1 += sampleE; 
		fragOut.color1 += sampleF; 
		fragOut.color1 += sampleG; 
		fragOut.color1 += sampleH; 
		fragOut.color1 += sampleI; 
		fragOut.color1 /= 9;
		
		return fragOut;
	}	
		
	mrtOut frag2BlurOpt2XPass2(v2fBlur i) 
	{
		mrtOut fragOut;
	
		fragOut.color0 = half4 (0,0,0,0);
		fragOut.color1 = half4 (0,0,0,0);

		// BLUR 1
		half4 sampleA = tex2D(_MainTex, i.uv.xy);
		half4 sampleB = tex2D(_MainTex, i.uv01.xy);
		half4 sampleC = tex2D(_MainTex, i.uv01.zw);
		half4 sampleD = tex2D(_MainTex, i.uv23.xy);
		half4 sampleE = tex2D(_MainTex, i.uv23.zw);
		half4 sampleF = tex2D(_MainTex, i.uv45.xy);
		half4 sampleG = tex2D(_MainTex, i.uv45.zw);
		half4 sampleH = tex2D(_MainTex, i.uv67.xy);
		half4 sampleI = tex2D(_MainTex, i.uv67.zw);
								
		fragOut.color0 += sampleA;
		fragOut.color0 += sampleB;
		fragOut.color0 += sampleC; 
		fragOut.color0 += sampleD; 
		fragOut.color0 += sampleE; 
		fragOut.color0 += sampleF; 
		fragOut.color0 += sampleG; 
		fragOut.color0 += sampleH; 
		fragOut.color0 += sampleI; 
		fragOut.color0 /= 9;
		
		// BLUR 2
		sampleA = tex2D(_MainTex2, i.uv.xy);
		sampleB = tex2D(_MainTex2, i.uv.xy + _Offsets2.xy*1.0f);
		sampleC = tex2D(_MainTex2, i.uv.xy + _Offsets2.xy*2.0f);
		sampleD = tex2D(_MainTex2, i.uv.xy + _Offsets2.xy*3.0f);
		sampleE = tex2D(_MainTex2, i.uv.xy + _Offsets2.xy*4.0f);
		sampleF = tex2D(_MainTex2, i.uv.xy + _Offsets2.xy*5.0f);
		sampleG = tex2D(_MainTex2, i.uv.xy + _Offsets2.xy*6.0f);
		sampleH = tex2D(_MainTex2, i.uv.xy + _Offsets2.xy*7.0f);
		sampleI = tex2D(_MainTex2, i.uv.xy + _Offsets2.xy*8.0f);
								
		fragOut.color1 += sampleA;
		fragOut.color1 += sampleB;
		fragOut.color1 += sampleC; 
		fragOut.color1 += sampleD; 
		fragOut.color1 += sampleE; 
		fragOut.color1 += sampleF; 
		fragOut.color1 += sampleG; 
		fragOut.color1 += sampleH; 
		fragOut.color1 += sampleI; 
		fragOut.color1 /= 9;
		
		fragOut.color1 += fragOut.color0;
		
		return fragOut;
	}	
	
	mrtOut frag2BlurOptPass1(v2fBlur i) 
	{
		mrtOut fragOut;
	
		fragOut.color0 = half4 (0,0,0,0);
		fragOut.color1 = half4 (0,0,0,0);

		// BLUR 1
		half4 sampleA = tex2D(_MainTex, i.uv.xy);
		
		half2 coc = _Offsets.xy * max(0.0/8.0, tex2D(_Reference, i.uv.xy).a);

		half4 sampleB = tex2D(_MainTex, i.uv.xy + 1.0f* coc);
		half4 sampleC = tex2D(_MainTex, i.uv.xy + 2.0f* coc);
		half4 sampleD = tex2D(_MainTex, i.uv.xy + 3.0f* coc);
		half4 sampleE = tex2D(_MainTex, i.uv.xy + 4.0f* coc);
		half4 sampleF = tex2D(_MainTex, i.uv.xy + 5.0f* coc);
		half4 sampleG = tex2D(_MainTex, i.uv.xy + 6.0f* coc);
		half4 sampleH = tex2D(_MainTex, i.uv.xy + 7.0f* coc);
		half4 sampleI = tex2D(_MainTex, i.uv.xy + 8.0f* coc);
								
		fragOut.color0 += sampleA;
		fragOut.color0 += sampleB;
		fragOut.color0 += sampleC; 
		fragOut.color0 += sampleD; 
		fragOut.color0 += sampleE; 
		fragOut.color0 += sampleF; 
		fragOut.color0 += sampleG; 
		fragOut.color0 += sampleH; 
		fragOut.color0 += sampleI; 
		fragOut.color0 /= 9;
		
		coc = _Offsets2.xy * max(0.0/8.0, tex2D(_Reference, i.uv.xy).a);

		// BLUR 2
		// sampleA still valid
		sampleB = tex2D(_MainTex, i.uv.xy + 1.0f* coc);
		sampleC = tex2D(_MainTex, i.uv.xy + 2.0f* coc);
		sampleD = tex2D(_MainTex, i.uv.xy + 3.0f* coc);
		sampleE = tex2D(_MainTex, i.uv.xy + 4.0f* coc);
		sampleF = tex2D(_MainTex, i.uv.xy + 5.0f* coc);
		sampleG = tex2D(_MainTex, i.uv.xy + 6.0f* coc);
		sampleH = tex2D(_MainTex, i.uv.xy + 7.0f* coc);
		sampleI = tex2D(_MainTex, i.uv.xy + 8.0f* coc);
								
		fragOut.color1 += sampleA;
		fragOut.color1 += sampleB;
		fragOut.color1 += sampleC; 
		fragOut.color1 += sampleD; 
		fragOut.color1 += sampleE; 
		fragOut.color1 += sampleF; 
		fragOut.color1 += sampleG; 
		fragOut.color1 += sampleH; 
		fragOut.color1 += sampleI; 
		fragOut.color1 /= 9;
		
		fragOut.color1 += fragOut.color0;
		
		return fragOut;
	}	
	
	half4 frag2BlurOptPass2(v2fBlur i) : COLOR 
	{
		mrtOut fragOut;
	
		fragOut.color0 = half4 (0,0,0,0);
		fragOut.color1 = half4 (0,0,0,0);

		// BLUR 1
		half4 sampleA = tex2D(_MainTex, i.uv.xy);

		half2 coc = _Offsets.xy * max(0.0/8.0, tex2D(_Reference, i.uv.xy).a);

		half4 sampleB = tex2D(_MainTex, i.uv.xy + 1.0f* coc);
		half4 sampleC = tex2D(_MainTex, i.uv.xy + 2.0f* coc);
		half4 sampleD = tex2D(_MainTex, i.uv.xy + 3.0f* coc);
		half4 sampleE = tex2D(_MainTex, i.uv.xy + 4.0f* coc);
		half4 sampleF = tex2D(_MainTex, i.uv.xy + 5.0f* coc);
		half4 sampleG = tex2D(_MainTex, i.uv.xy + 6.0f* coc);
		half4 sampleH = tex2D(_MainTex, i.uv.xy + 7.0f* coc);
		half4 sampleI = tex2D(_MainTex, i.uv.xy + 8.0f* coc);
								
		fragOut.color0 += sampleA;
		fragOut.color0 += sampleB;
		fragOut.color0 += sampleC; 
		fragOut.color0 += sampleD; 
		fragOut.color0 += sampleE; 
		fragOut.color0 += sampleF; 
		fragOut.color0 += sampleG; 
		fragOut.color0 += sampleH; 
		fragOut.color0 += sampleI; 
		fragOut.color0 /= 9.0f;
		
		coc = _Offsets2.xy * max(0.0/8.0, tex2D(_Reference, i.uv.xy).a);

		// BLUR 2
		sampleA = tex2D(_MainTex2, i.uv.xy);
		sampleB = tex2D(_MainTex2, i.uv.xy + 1.0f* coc);
		sampleC = tex2D(_MainTex2, i.uv.xy + 2.0f* coc);
		sampleD = tex2D(_MainTex2, i.uv.xy + 3.0f* coc);
		sampleE = tex2D(_MainTex2, i.uv.xy + 4.0f* coc);
		sampleF = tex2D(_MainTex2, i.uv.xy + 5.0f* coc);
		sampleG = tex2D(_MainTex2, i.uv.xy + 6.0f* coc);
		sampleH = tex2D(_MainTex2, i.uv.xy + 7.0f* coc);
		sampleI = tex2D(_MainTex2, i.uv.xy + 8.0f* coc);
								
		fragOut.color1 += sampleA;
		fragOut.color1 += sampleB;
		fragOut.color1 += sampleC; 
		fragOut.color1 += sampleD; 
		fragOut.color1 += sampleE; 
		fragOut.color1 += sampleF; 
		fragOut.color1 += sampleG; 
		fragOut.color1 += sampleH; 
		fragOut.color1 += sampleI; 
		fragOut.color1 /= 9.0f;
		
		return 0.5f * (fragOut.color0 + fragOut.color1);
	}	

	// fragBlur2Gauss is mostly interested in the alpha channel and hence
	// uses gaussian for smoother foreground coc's
	
	half4 fragBlurForFgCoc (v2fBlur i) : COLOR 
	{
		half4 blurredColor = half4 (0,0,0,0);

		half4 sampleA = tex2D(_MainTex, i.uv.xy);
		half4 sampleB = tex2D(_MainTex, i.uv01.xy);
		half4 sampleC = tex2D(_MainTex, i.uv01.zw);
		half4 sampleD = tex2D(_MainTex, i.uv23.xy);
		half4 sampleE = tex2D(_MainTex, i.uv23.zw);
		half4 sampleF = tex2D(_MainTex, i.uv45.xy);
		half4 sampleG = tex2D(_MainTex, i.uv45.zw);
						
		blurredColor += sampleA;
		blurredColor += sampleB;
		blurredColor += sampleC; 
		blurredColor += sampleD; 
		blurredColor += sampleE; 
		blurredColor += sampleF; 
		blurredColor += sampleG; 

		blurredColor /= 7.0;
		
		half4 alphas = half4(sampleB.a, sampleC.a, sampleD.a, sampleE.a);
		//half4 alphas2 = half4(sampleE.a, sampleF.a, sampleG.a, 1.0);
		
		half overlapFactor = saturate(length(alphas-sampleA.aaaa)-0.5)*2.0;

		//half4 maxedColor = max(sampleA, sampleB);
		//maxedColor = max(maxedColor, sampleC);
		
		//blurredColor.a = saturate(blurredColor.a * 4.0f);
		// to do ot not to do
		
		blurredColor.a += overlapFactor; // max(maxedColor.a, blurredColor.a);
		
		return blurredColor;
	}	

	half4 frag4TapBlur (v2f i) : COLOR 
	{
		half4 tapA =  tex2D(_MainTex, i.uv1.xy + 0.5*_MainTex_TexelSize);
		half4 tapB =  tex2D(_MainTex, i.uv1.xy - 0.5*_MainTex_TexelSize);
		half4 tapC =  tex2D(_MainTex, i.uv1.xy + 0.5*_MainTex_TexelSize * half2(1,-1));
		half4 tapD =  tex2D(_MainTex, i.uv1.xy - 0.5*_MainTex_TexelSize * half2(1,-1));

		return (tapA+tapB+tapC+tapD)/4.0;
	}	
	
	half4 frag4TapMax (v2f i) : COLOR 
	{
		half4 tapA =  tex2D(_MainTex, i.uv1.xy + _MainTex_TexelSize);
		half4 tapB =  tex2D(_MainTex, i.uv1.xy - _MainTex_TexelSize);
		half4 tapC =  tex2D(_MainTex, i.uv1.xy + _MainTex_TexelSize * half2(1,-1));
		half4 tapD =  tex2D(_MainTex, i.uv1.xy - _MainTex_TexelSize * half2(1,-1));

		return max(max(max(tapA,tapB),tapC),tapD);
	}	
	
	float4 fragCombine3Taps(v2f i) : COLOR 
	{
		half4 tapA = tex2D(_TapLowA, i.uv1.xy);
		half4 tapB = tex2D(_TapLowB, i.uv1.xy);
		half4 tapC = tex2D(_TapLowC, i.uv1.xy);

		return (tapA + tapB + tapC) / 3.0;
	}

	float4 fragSharpen (v2f i) : COLOR 
	{
		half4 nonBlurred = tex2D(_MainTex, i.uv1.xy);
		half4 blurred = tex2D(_MainTex2, i.uv1.xy);

		return nonBlurred + saturate(blurred -nonBlurred)*0.14;
	}	
	
	
	float4 fragApply (v2fDofApply i) : COLOR 
	{		
		float4 tapHigh = tex2D (_MainTex, i.uv.xy);
		
		#if SHADER_API_D3D9
		if (_MainTex_TexelSize.y < 0)
			i.uv.xy = i.uv.xy * half2(1,-1)+half2(0,1);
		#endif

		float4 tapLow = tex2D (_TapLowA, i.uv.xy); 
		
		tapHigh.rgb = tapHigh.rgb/tapHigh.a;
		tapLow.rgb = tapLow.rgb/tapLow.a;
		
		float4 outColor = lerp(tapHigh, tapLow, saturate(tapHigh.a * 5.5));
		return outColor;
	}	
	
	half4 fragApplyDebug (v2fDofApply i) : COLOR 
	{		
		float4 tapHigh = tex2D (_MainTex, i.uv.xy);
		
		#if SHADER_API_D3D9
		if (_MainTex_TexelSize.y < 0)
			i.uv.xy = i.uv.xy * half2(1,-1)+half2(0,1);
		#endif
		
		float4 tapLow = tex2D (_TapLowA, i.uv.xy); 
		tapLow.rgb = half3(0,tapLow.a*0.5,0) + tapLow.rgb*0.5;

		tapHigh.rgb = tapHigh.rgb/tapHigh.a;
		tapLow.rgb = tapLow.rgb/tapLow.a;
		
		float4 outColor = lerp(tapHigh, tapLow, saturate(tapHigh.a * 5.5));
		return outColor;
	}		
		
	float4 fragCaptureBackgroundCoc (v2f i) : COLOR 
	{	
		float4 color = tex2D (_MainTex, i.uv1.xy);
		color.a = 0.0;
		float4 lowTap = tex2D(_TapLowA, i.uv1.xy);
		
		float d = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uv1.xy));
		d = Linear01Depth (d);
		
		float focalDistance01 = _CurveParams.w + _CurveParams.z;
		
		if (d > focalDistance01) 
			color.a = (d - focalDistance01);
	
		color.a = (color.a * _CurveParams.y);
		
		// we are mixing the newly calculated BG COC with the foreground COC
		// also, for foreground COC, let's scale the COC a bit to get nicer overlaps	
		color.a = max(lowTap.a + saturate(lowTap.a*2.0-1.0), color.a);
		
		color.a = saturate(color.a + COC_SMALL_VALUE);
		color.rgb *= color.a;
		
		return saturate(color);
	} 
	
	half4 fragCaptureForegroundCoc (v2f i) : COLOR 
	{		
		half4 color = tex2D (_MainTex, i.uv1.xy);
		color.a = 0.0;

		#if SHADER_API_D3D9
		if (_MainTex_TexelSize.y < 0)
			i.uv1.xy = i.uv1.xy * half2(1,-1)+half2(0,1);
		#endif

		float d = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uv1.xy));
		d = Linear01Depth (d);	
		
		float focalDistance01 = (_CurveParams.w - _CurveParams.z);	
		
		if (d < focalDistance01) 
			color.a = (focalDistance01 - d);
		
		color.a = saturate(color.a * _CurveParams.x);	
				
		return color;	
	}	
	
	half4 fragReturnMainTex (v2f i) : COLOR 
	{	
		return tex2D(_MainTex, i.uv1.xy);
	} 	
	
	// not being used atm
	
	half4 fragCaptureCenterDepth (v2f i) : COLOR 
	{
		return 0;
	}	
	
	// used for simple one one blend
	
	half4 fragAdd (v2f i) : COLOR {	
		half4 from = tex2D( _MainTex, i.uv1.xy );
		return from;
	}
		
	ENDCG
	
Subshader {
 
 // pass 0
 
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  ColorMask RGB
	  Fog { Mode off }      

      CGPROGRAM
      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vertDofApply
      #pragma fragment fragApply
      
      ENDCG
  	}

 // pass 1
 
 Pass 
 {
	  ZTest Always Cull Off ZWrite Off
	  ColorMask RGB
	  Fog { Mode off }      

      CGPROGRAM
      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vertDofApply
      #pragma fragment fragApplyDebug

      ENDCG
  	}

 // pass 2

 Pass {
	  ZTest Always Cull Off ZWrite Off
	  ColorMask RGB
	  Fog { Mode off }      

      CGPROGRAM
      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vertDofApply
      #pragma fragment fragApplyDebug

      ENDCG
  	}
  	
  	
 
 // pass 3
 
 Pass 
 {
	  ZTest Always Cull Off ZWrite Off
	  // ColorMask A
	  Fog { Mode off }      

      CGPROGRAM
      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragCaptureBackgroundCoc

      ENDCG
  	}  
  	 	
	
 // pass 4

 Pass 
 {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      
	  Blend One One

      CGPROGRAM
      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vertBlur
      #pragma fragment fragBlurWithOneThirdApply
      
      ENDCG
  	}  	

 // pass 5
  
 Pass 
 {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragCaptureForegroundCoc

      ENDCG
  	} 

 // pass 6
 
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vertBlurPlusMinus
      #pragma fragment fragBlur9TapGauss

      ENDCG
  	} 

 // pass 7
 
 Pass { 
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM

      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment frag4TapBlur

      ENDCG
  	} 

 // pass 8
 
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Blend One One
	  ColorMask RGB
  	  Fog { Mode off }      

      CGPROGRAM
      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragAdd

      ENDCG
  	} 
  	
 // pass 9 
 // TODO: make max blend work
 
 Pass 
 {
	  ZTest Always Cull Off ZWrite Off
	  ColorMask A
	  Blend One One
	  Fog { Mode off }       

      CGPROGRAM
      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragReturnMainTex
      ENDCG
  	} 
  	
 // pass 10
 
 Pass 
 {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vertBlur
      #pragma fragment fragBlur9Tap

      ENDCG
  	}   	
  	
 // pass 11
 
 Pass 
 {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vertBlurPlusMinus
      #pragma fragment fragBlurForFgCoc

      ENDCG
  	}   	
  
 // pass 12
 
 Pass 
 {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM

      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment frag4TapMax

      ENDCG
 }   	
 
 // pass 13
 
 Pass 
 {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vertBlur
      #pragma fragment frag2BlurOptPass1

      ENDCG
  	}   
  	
 // pass 14
 
 Pass 
 {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vertBlur
      #pragma fragment frag2BlurOptPass2

      ENDCG
  	}    	
  	
 // pass 15
 
 Pass 
 {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }       

      CGPROGRAM
      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vertBlur
      #pragma fragment frag2BlurOpt2XPass1

      ENDCG
  	}     	
  	
 // pass 16
 
 Pass 
 {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vertBlur
      #pragma fragment frag2BlurOpt2XPass2

      ENDCG
  	}    

 // pass 17
 
 Pass 
 {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vertBlur
      #pragma fragment frag2BlurOpt2XPass3

      ENDCG
  	}  

// pass 18

 Pass 
 {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }       

      CGPROGRAM
      #pragma glsl
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragSharpen
      ENDCG
  	} 
  	  	  	
}
  
Fallback off

}