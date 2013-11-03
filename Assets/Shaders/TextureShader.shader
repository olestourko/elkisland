Shader "Custom/TextureShader" {
	Properties {
		_MainTex ("Diffuse", 2D) = "black" {}
		_AlphaTex ("Alpha", 2D) = "white" {}
	}
	SubShader {
		Tags { "Queue" = "Transparent" }
		Pass
		{
			Cull Off
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			uniform sampler2D _MainTex;
			uniform sampler2D _AlphaTex;
			
			struct vertexInput
			{
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
			};
			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float4 tex : TEXCOORD0;
			};
			
			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;
				output.tex = input.texcoord;
				output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
				return output;
			}
			
			float4 frag(vertexOutput input) : COLOR
			{
				float4 c = tex2D(_MainTex, float2(input.tex));
				float4 alpha = tex2D(_AlphaTex, float2(input.tex));
				c.w = alpha.r;
				return c;
				// look up the color of the texture image specified by
				// the uniform "_MainTex" at the position specified by
				// "input.tex.x" and "input.tex.y" and return it
			}
			ENDCG
		}
	}
}
