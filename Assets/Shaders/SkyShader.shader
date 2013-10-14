Shader "SkyShader" {

	SubShader 
	{
		//Tags { "Queue"="Background" "RenderType"="Background" }
		Pass
		{
			Cull Front
			Fog { Mode Off }
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			
			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float4 col: TEXCOORD0;
			};
			
			vertexOutput vert(float4 vertexPos : POSITION)
			{
				vertexOutput output;
				output.pos = mul(UNITY_MATRIX_MVP, vertexPos);
				float4 col = float4(0.118, 0.294, 0.294, 1.0);
				
				float rg_offset = vertexPos.y * 1.5;
				float b_offset = rg_offset * 0.5;
				
				if(rg_offset > 0.9) 
				{
					rg_offset = 0.9;
					b_offset = 0.45;
				}
				
				col.r += rg_offset;
				col.g += rg_offset;
				col.b += b_offset;

				output.col = col;
				//output.col = float4(vertexPos.y, vertexPos.y, vertexPos.y, 1.0);
				
				return output;
			}
			
			float4 frag(vertexOutput input) : COLOR
			{
				float4 col = input.col;
				return col;
			}
			
			ENDCG
		}
	} 

}
