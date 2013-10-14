Shader "Cg basic shader"
{
	SubShader
	{
		//Main pass
		Pass
		{
			Cull Back
			ZWrite Off
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			
			float4 vert(float4 vertexPos : POSITION) : SV_POSITION
			{
				return mul(UNITY_MATRIX_MVP, vertexPos);
			}
			
			float4 frag() : COLOR
			{
				discard;
				return float4(1.0, 0.5, 0.0, 1.0);
			}
			ENDCG
		}
	
	
		//Transparent Pass
		Tags { "Queue" = "Transparent" }
		Pass
		{
			Cull Back
			Blend SrcAlpha OneMinusSrcAlpha
			//ZWrite Off
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			
			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float4 col : TEXCOORD0;
			};
			
			vertexOutput vert(float4 vertexPos : POSITION)
			{
				vertexOutput output;
				float4 factor = float4(4.0, 4.0, 4.0, 1.0);
				float4 pos = mul(UNITY_MATRIX_MVP, vertexPos * factor);
				float4 original_pos = mul(UNITY_MATRIX_MVP, vertexPos);
				output.col = original_pos - pos;
				output.pos = pos;
				return output;
			}
			
			float4 frag(vertexOutput input) : COLOR
			{
				//get distance from "real" vertex
				float alpha = sqrt(
					(input.col.x * input.col.x) +
					(input.col.y * input.col.y) +
					(input.col.z * input.col.z)
					) * 2.65;
				alpha = pow(alpha, 30);
				//float alpha = input.pos - input.original_pos;
				return float4(0.064, 0.064, 0.0936, 1/alpha);
				//return float4(alpha, alpha, alpha, 1.0);
			}
			
			ENDCG
		}
	}
}