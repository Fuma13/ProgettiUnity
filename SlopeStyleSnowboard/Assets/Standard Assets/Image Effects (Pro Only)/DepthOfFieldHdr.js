
#pragma strict

@script ExecuteInEditMode
@script RequireComponent (Camera)
@script AddComponentMenu ("Image Effects/Depth of Field (HDR)") 

class DepthOfFieldHdr extends PostEffectsBase {	

	// TODO: we might also experiment with weird dividers (e.g. 1.5)
	enum DofHDRResolution {
		High = 1,
		Medium = 2,
		Low = 3,	
	}
	
	// sorted by cost
	enum DofHDRBlurType {
		CheapGauss = 0,
		OptimizedBokeh = 1,     // uses MRT to minimize passes and texture taps
		Bokeh = 2,              // no MRT support-fallback
		// BokehDoublePass = 3,	// MRT version of double pass hex-blur
	}	
	
	public var blurType : DofHDRBlurType = DofHDRBlurType.OptimizedBokeh;
	public var resolution : DofHDRResolution  = DofHDRResolution.High;
	public var gaussBlurIterations : int = 1;
    public var blurredDownsample : boolean = true;
	
	public var focalPoint : float = 1.0f;
	public var smoothness : float = 0.5f;
	public var foregroundCurve : float = 1.0f;
	public var backgroundCurve : float = 1.0f;

	public var focalTransform : Transform = null;
	public var focalSize : float = 0.0f;
	
	public var blurWidth : float = 1.75f;
	public var foregroundOverlap : float = 2.25f;
	public var dofHdrShader : Shader;		
	
	private var focalStartCurve : float = 2.0f;
	private var focalEndCurve : float = 2.0f;
	private var focalDistance01 : float = 0.1f;	
	
	private var dofHdrMaterial : Material = null;
    
    public var visualizeFocus : boolean = false;
		        
        	
	function CreateMaterialsIfNeeded () {		
		dofHdrMaterial = CheckShaderAndCreateMaterial (dofHdrShader, dofHdrMaterial);  
	}
	
	function Start () {
		CreateMaterialsIfNeeded ();
		// we need depth AND hdr support (and MRT, but this is an extra check which we'll perform later on)
		CheckSupport (true, true);
	}

	function OnEnable() {
		GetComponent.<Camera>().depthTextureMode |= DepthTextureMode.Depth;		
	}
	
	function FocalDistance01 (worldDist : float) : float {
		return GetComponent.<Camera>().WorldToViewportPoint((worldDist-GetComponent.<Camera>().nearClipPlane) * GetComponent.<Camera>().transform.forward + GetComponent.<Camera>().transform.position).z / (GetComponent.<Camera>().farClipPlane-GetComponent.<Camera>().nearClipPlane);	
	}
			
	function OnRenderImage (source : RenderTexture, destination : RenderTexture) {	
		/*
		------------------------------------------------------------------------------------------
		------------------------------------------------------------------------------------------		
		HDR DEPTH OF FIELD -----------------------------------------------------------------------
		------------------------------------------------------------------------------------------
		OK the new algorithm is soooo much better & nicer and polished and shit!
		------------------------------------------------------------------------------------------
		------------------------------------------------------------------------------------------
		
		capturing circle of confusion & premultiply
		------------------------------------------------------------------------------------------
		[1a] IF NEEDED: calculate foreground coc into medium rez rt
		[1b] IF NEEDED: blur foreground coc a bit
		[1c] IF NEEDED: add foreground coc to background coc (max blend)
		[2] calculate background coc into alpha
		
		blurrrrr
		------------------------------------------------------------------------------------------
		[3] make sure colors are coc-premultiplied
		[4] blur is now super simple. chose one out of many possible blur techniques.
		
		final pass / apply
		------------------------------------------------------------------------------------------
		[5] apply blurred version onto screen via coc lookup
		
		------------------------------------------------------------------------------------------
		------------------------------------------------------------------------------------------			
		*/
		
		CreateMaterialsIfNeeded ();
		
		var i : int = 0;
		var internalBlurWidth : float = blurWidth;
		
		// clamp values so they make sense

		if (smoothness < 0.4f) smoothness = 0.4f;
		if (focalSize < 0.00001f) focalSize = 0.00001f;
		if (foregroundCurve < 0.01f) foregroundCurve = 0.0f;
		if (backgroundCurve < 0.01f) backgroundCurve = 0.0f;
		
		gaussBlurIterations = gaussBlurIterations < 1 ? 1 : gaussBlurIterations;
			
		// calculate needed focal parameters

		var div : int = resolution;		
		var focal01Size : float = focalSize / (GetComponent.<Camera>().farClipPlane - GetComponent.<Camera>().nearClipPlane);
		focalDistance01 = focalTransform ? (GetComponent.<Camera>().WorldToViewportPoint (focalTransform.position)).z / (GetComponent.<Camera>().farClipPlane) : FocalDistance01 (focalPoint);
		focalStartCurve = focalDistance01 * smoothness;
		focalEndCurve = focalStartCurve;
		
		// TODO: reduce # of HDR textures needed!
		
		var rtA : RenderTexture = RenderTexture.GetTemporary(source.width/div, source.height/div, 0, RenderTextureFormat.ARGBHalf);
		var rtB : RenderTexture = RenderTexture.GetTemporary(source.width/div, source.height/div, 0, RenderTextureFormat.ARGBHalf);
		var rtC : RenderTexture = RenderTexture.GetTemporary(source.width/div, source.height/div, 0, RenderTextureFormat.ARGBHalf);	
		var scene : RenderTexture = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGBHalf);	
		rtA.filterMode = FilterMode.Bilinear;
		rtB.filterMode = FilterMode.Bilinear;
		rtC.filterMode = FilterMode.Bilinear;
        source.filterMode = FilterMode.Bilinear;
		scene.filterMode = FilterMode.Bilinear;
	
		dofHdrMaterial.SetVector ("_CurveParams", Vector4 (foregroundCurve / focalStartCurve, backgroundCurve / focalEndCurve, focal01Size * 0.5, focalDistance01));
		dofHdrMaterial.SetVector ("_InvRenderTargetSize", Vector4 (1.0 / (1.0 * source.width), 1.0 / (1.0 * source.height),0.0,0.0));
		
		// Capture foreground CoC in alpha channel	
	
		Graphics.Blit (source /* need to bind source so we can detect screen flipiness */, scene, dofHdrMaterial, 5); 		
		Graphics.Blit (scene, rtA, dofHdrMaterial, 11);
		
		// Blur foreground coc a bit so we get nicer COC overlaps

		Blur (rtA, rtA, rtC, internalBlurWidth * foregroundOverlap, 11);	
		
		// capture background CoC and mix in with foreground coc

		dofHdrMaterial.SetTexture ("_TapLowA", rtA);
		Graphics.Blit (scene, source, dofHdrMaterial, 3); 	// capture (and multiply with .a)
        if(blurredDownsample)
            Graphics.Blit (source, rtB, dofHdrMaterial, 7);		// 4 tap blur 
        else
            Graphics.Blit (source, rtB);
		
		// the meat of our depth of field: "blurring"
		// they define the main performance ...
		// it's cheapest to use dual pass gaussian and lowest possible resolution
		// currently we also scale the blur size by the coc, so we get really nicely
		// looking transitions and animations. TODO: make cheap setting that doesn't do that

		var internalBlurType : DofHDRBlurType = blurType;

		if (internalBlurType == DofHDRBlurType.OptimizedBokeh) {
			BlurPseudoBokehOptimized (rtB, rtA, rtC, internalBlurWidth);
		}
		else if (internalBlurType == DofHDRBlurType.Bokeh) {
			if(SystemInfo.supportedRenderTargetCount<2) {
				BlurPseudoBokeh (rtB, rtA, rtC, internalBlurWidth);
			}
			else {
				Blur (rtB, rtC, rtA, internalBlurWidth * 0.375f, 6);
				BlurPseudoBokehOptimized (rtC, rtA, rtB, internalBlurWidth);
			}
		}
		/*
		else if (internalBlurType == DofHDRBlurType.BokehDoublePass) {
			if(SystemInfo.supportedRenderTargetCount<2) {
				BlurPseudoBokeh (rtB, rtA, rtC, internalBlurWidth);
			}
			else {	
				// TODO: looks ugly atm, doesn't preserve the bokeh shape => investigate :(
				BlurPseudoBokehOptimizedDoublePass (rtB, rtA, rtC, internalBlurWidth);
			}
		}
		*/
		else {
			for(i = 0; i < gaussBlurIterations; i++)
				Blur (rtB, rtB, rtC, internalBlurWidth * 4.0f, 6);	
		}
		
		// final pass: blend previously executed blurs onto screen
		dofHdrMaterial.SetTexture ("_TapLowA", blurType == DofHDRBlurType.CheapGauss ? rtB : rtA);
		Graphics.Blit (source, destination, dofHdrMaterial, visualizeFocus ? 1 : 0);
		
		RenderTexture.ReleaseTemporary(rtA);
		RenderTexture.ReleaseTemporary(rtB);
		RenderTexture.ReleaseTemporary(rtC);
		RenderTexture.ReleaseTemporary(scene);
	}
	
	function Blur (from : RenderTexture, to : RenderTexture, work : RenderTexture, spread : float, pass : int) {
		var oneOverWidth : float = 1.0f / (1.0f * to.width);
		var widthOverHeight : float = (1.0f * to.width) / (1.0f * to.height);

		dofHdrMaterial.SetVector ("_Offsets", Vector4 (0.0f, spread * oneOverWidth, 0.0f, 0.0f));
		Graphics.Blit (from, work, dofHdrMaterial, pass);
		dofHdrMaterial.SetVector ("_Offsets", Vector4 (oneOverWidth * spread / widthOverHeight,  0.0f, 0.0f, 0.0f));		
		Graphics.Blit (work, to, dofHdrMaterial, pass);	 				
	}	
	
	function BlurPseudoBokeh (from : RenderTexture, to : RenderTexture,  tmp : RenderTexture, spread : float) {		
		var blurPass : int = 10;
		var applyPass : int = 4;
		var dist : float = 1.0f;
		var shearedDistY : float = 0.75f;
		var shearedDistX : float = 1.0f;
		var shearedLen = Mathf.Sqrt(shearedDistY*shearedDistY + shearedDistX*shearedDistX);
		shearedDistY /= shearedLen;
		shearedDistX /= shearedLen;
		dist *= spread * (1.0f / (1.0f * to.height));
		shearedDistX *= spread * (1.0f /  (1.0f * to.width));
		shearedDistY *= spread * (1.0f /  (1.0f * to.height));
				
		var tmp2 : RenderTexture = RenderTexture.GetTemporary(tmp.width, tmp.height, 0, RenderTextureFormat.ARGBHalf);

		RenderTexture.active = to;
		GL.Clear(false, true, Color(0,0,0,0));
		
		// UP
		dofHdrMaterial.SetVector ("_Offsets", Vector4 (0.0f, dist, 0.0f, 0.0f));
		Graphics.Blit (from, tmp, dofHdrMaterial, blurPass);
		Graphics.Blit (tmp, tmp2, dofHdrMaterial, blurPass);

		// LD
		dofHdrMaterial.SetVector ("_Offsets", Vector4 (-shearedDistX, -shearedDistY, 0.0f, 0.0f));
		Graphics.Blit (tmp2, tmp, dofHdrMaterial, blurPass);
		Graphics.Blit (tmp, to, dofHdrMaterial, applyPass);

		// UP
		dofHdrMaterial.SetVector ("_Offsets", Vector4 (0.0f, dist, 0.0f, 0.0f));
		Graphics.Blit (from, tmp, dofHdrMaterial, blurPass);
		Graphics.Blit (tmp, tmp2, dofHdrMaterial, blurPass);

		// RD
		dofHdrMaterial.SetVector ("_Offsets", Vector4 (shearedDistX, -shearedDistY, 0.0f, 0.0f));
		Graphics.Blit (tmp2, tmp, dofHdrMaterial, blurPass);			
		Graphics.Blit (tmp, to, dofHdrMaterial, applyPass);			
	
		// LD
		dofHdrMaterial.SetVector ("_Offsets", Vector4 (-shearedDistX, -shearedDistY, 0.0f, 0.0f));
		Graphics.Blit (from, tmp, dofHdrMaterial, blurPass);	
		Graphics.Blit (tmp, tmp2, dofHdrMaterial, blurPass);	

		// RD
		dofHdrMaterial.SetVector ("_Offsets", Vector4 (shearedDistX, -shearedDistY, 0.0f, 0.0f));
		Graphics.Blit (tmp2, tmp, dofHdrMaterial, blurPass);	
		Graphics.Blit (tmp, to, dofHdrMaterial, applyPass);	

		RenderTexture.ReleaseTemporary(tmp2);

	}
	
	function BlurPseudoBokehOptimized (from : RenderTexture, to : RenderTexture,  tmp : RenderTexture, spread : float) {		
		var dist : float = 1.0f;
		var shearedDistY : float = 0.75f;
		var shearedDistX : float = 1.0f;
		var shearedLen = Mathf.Sqrt(shearedDistY*shearedDistY + shearedDistX*shearedDistX);
		shearedDistY /= shearedLen;
		shearedDistX /= shearedLen;
		dist *= spread * (1.0f / (1.0f * to.height));
		shearedDistX *= spread * (1.0f /  (1.0f * to.width));
		shearedDistY *= spread * (1.0f /  (1.0f * to.height));
		
		// WE ARE SAVING:
		// 2 blur passes
		// 1 apply pass
		// 1 clear pass
		// BUT:
		// optimized blur come at a cost of an additional render texture :(
		
		var tmp2 : RenderTexture = RenderTexture.GetTemporary(tmp.width, tmp.height, 0, RenderTextureFormat.ARGBHalf);

		dofHdrMaterial.SetTexture("_Reference", from);
		
		// UP
		dofHdrMaterial.SetVector ("_Offsets", Vector4 (0.0f, dist, 0.0f, 0.0f));
		// LD
		dofHdrMaterial.SetVector ("_Offsets2", Vector4 (-shearedDistX, -shearedDistY, 0.0f, 0.0f));
		
		var dests : RenderBuffer[] = [tmp.colorBuffer, tmp2.colorBuffer];
		Graphics.SetRenderTarget(dests, to.depthBuffer);
		dofHdrMaterial.SetTexture ("_MainTex", from);	 		
		DofBlitMRT(dofHdrMaterial, 13);	
	
		// LD
		dofHdrMaterial.SetVector ("_Offsets", Vector4 (-shearedDistX, -shearedDistY, 0.0f, 0.0f));
		// RD
		dofHdrMaterial.SetVector ("_Offsets2", Vector4 (shearedDistX, -shearedDistY, 0.0f, 0.0f));

		Graphics.SetRenderTarget(to.colorBuffer, to.depthBuffer);
		dofHdrMaterial.SetTexture ("_MainTex", tmp);	 		
		dofHdrMaterial.SetTexture ("_MainTex2", tmp2);	 
		DofBlitMRT(dofHdrMaterial, 14);	// actually renders to a simple SRT
		
		RenderTexture.ReleaseTemporary(tmp2);
	}	
	
	function BlurPseudoBokehOptimizedDoublePass (from : RenderTexture, to : RenderTexture,  tmp : RenderTexture, spread : float) {		
		var dist : float = 1.0f;
		var shearedDistY : float = 0.75f;
		var shearedDistX : float = 1.0f;
		var shearedLen = Mathf.Sqrt(shearedDistY*shearedDistY + shearedDistX*shearedDistX);
		shearedDistY /= shearedLen;
		shearedDistX /= shearedLen;
		dist *= spread * (1.0f / (1.0f * to.height));
		shearedDistX *= spread * (1.0f /  (1.0f * to.width));
		shearedDistY *= spread * (1.0f /  (1.0f * to.height));
		
		// WE ARE SAVING:
		// 2 blur passes
		// 1 apply pass 
		// 1 clear pass
		// BUT:
		// optimized blur come at a cost of an additional render texture :(
		
		var tmp2 : RenderTexture = RenderTexture.GetTemporary(tmp.width, tmp.height, 0, RenderTextureFormat.ARGBHalf);
		
		// UP
		dofHdrMaterial.SetVector ("_Offsets", Vector4 (0.0f, dist, 0.0f, 0.0f));
		// LD
		dofHdrMaterial.SetVector ("_Offsets2", Vector4 (-shearedDistX, -shearedDistY, 0.0f, 0.0f));
		
		Graphics.SetRenderTarget([tmp.colorBuffer, tmp2.colorBuffer], tmp2.depthBuffer);
		dofHdrMaterial.SetTexture ("_MainTex", from);	 		
		DofBlitMRT(dofHdrMaterial, 15);	

		// UP
		dofHdrMaterial.SetVector ("_Offsets", Vector4 (0.0f, dist, 0.0f, 0.0f));
		// LD
		dofHdrMaterial.SetVector ("_Offsets2", Vector4 (-shearedDistX, -shearedDistY, 0.0f, 0.0f));
		
		Graphics.SetRenderTarget([from.colorBuffer, to.colorBuffer], to.depthBuffer);
		dofHdrMaterial.SetTexture ("_MainTex", tmp);	 		
		dofHdrMaterial.SetTexture ("_MainTex2", tmp2);	 		
		DofBlitMRT(dofHdrMaterial, 16);			
	
		// LD
		dofHdrMaterial.SetVector ("_Offsets", Vector4 (-shearedDistX, -shearedDistY, 0.0f, 0.0f));
		// RD
		dofHdrMaterial.SetVector ("_Offsets2", Vector4 (shearedDistX, -shearedDistY, 0.0f, 0.0f));
		
		Graphics.SetRenderTarget([tmp.colorBuffer, tmp2.colorBuffer], tmp2.depthBuffer);
		dofHdrMaterial.SetTexture ("_MainTex", from);	 		
		dofHdrMaterial.SetTexture ("_MainTex2", to);	 		
		DofBlitMRT(dofHdrMaterial, 17);			

		// LD
		dofHdrMaterial.SetVector ("_Offsets", Vector4 (-shearedDistX, -shearedDistY, 0.0f, 0.0f));
		// RD
		dofHdrMaterial.SetVector ("_Offsets2", Vector4 (shearedDistX, -shearedDistY, 0.0f, 0.0f));

		Graphics.SetRenderTarget(to.colorBuffer, to.depthBuffer);
		dofHdrMaterial.SetTexture ("_MainTex", tmp);	 		
		dofHdrMaterial.SetTexture ("_MainTex2", tmp2);	 
		DofBlitMRT(dofHdrMaterial, 14);	// actually renders to a simple SRT
		
		RenderTexture.ReleaseTemporary(tmp2);
	}		
	
	static function DofBlitMRT (fxMaterial : Material, passNr : int) {
		// var invertY : boolean = source.texelSize.y < 0.0f;
		GL.PushMatrix ();
		GL.LoadOrtho ();
		fxMaterial.SetPass (passNr);	
	    	GL.Begin (GL.QUADS);				
		GL.MultiTexCoord2 (0, 0.0f, 0.0f); 
		GL.Vertex3 (0.0f, 0.0f, 0.0f); // BL
		GL.MultiTexCoord2 (0, 1.0f, 0.0f); 
		GL.Vertex3 (1.0f, 0.0f, 0.0f); // BR
		GL.MultiTexCoord2 (0, 1.0f, 1.0f); 
		GL.Vertex3 (1.0f, 1.0f, 0.0f); // TR
		GL.MultiTexCoord2 (0, 0.0f, 1.0f); 
		GL.Vertex3 (0.0f, 1.0f, 0.0); // TL
		GL.End ();
	    	GL.PopMatrix ();
	}	
}
