Shader "Custom/AdditiveMask" {
	Properties{
	 _TintColor("Main Color", Color) = (1,1,1,1)
	 _MainTex("Base (RGB)", 2D) = "white" {}
	 _Mask("Culling Mask", 2D) = "white" {}
	 _Cutoff("Alpha cutoff", Range(0,1)) = 0.1
	}
		SubShader{
		 Tags { "QUEUE" = "Transparent" }
		 Pass {
		  Tags { "QUEUE" = "Transparent" }
		  Color[_TintColor]
		  ZWrite Off
		  Cull Off
		  Blend SrcAlpha OneMinusSrcAlpha
		  AlphaTest GEqual[_Cutoff]
		  SetTexture[_Mask] { ConstantColor[_TintColor] combine texture * constant }
		  SetTexture[_MainTex] { combine texture * previous double }
		 }
	 }
}