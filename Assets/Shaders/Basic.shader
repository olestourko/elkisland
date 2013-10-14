Shader "Cg basic shader"
{
	Properties
	{
		_Alpha("Alpha", Float) = 0.0
	}

	SubShader
	{	
		//Transparent Pass
		Tags { "Queue" = "Transparent" }
		Pass
		{
			Cull Back
			Blend SrcAlpha OneMinusSrcAlpha
			//ZWrite Off
			CGPROGRAM
			
			uniform float _Alpha;
			
			#pragma vertex vert
			#pragma fragment frag
			
			
			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};
			
			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float3 normal : TEXCOORD0;
				float3 viewDir: TEXCOORD1;
			};
			
			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;
				
				float4x4 modelMatrix = _Object2World;
				float4x4 modelMatrixInverse = _World2Object;
				output.normal = normalize(float3(
					mul(float4(input.normal, 0.0), modelMatrixInverse)));
				output.viewDir = normalize(_WorldSpaceCameraPos
					- float3(mul(modelMatrix, input.vertex)));
				output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
				return output;
			}
			
			float4 frag(vertexOutput input) : COLOR
			{			
				float3 normalDirection = normalize(input.normal);
				float3 viewDirection = normalize(input.viewDir);
				float alpha = dot(viewDirection, normalDirection);
				alpha = pow(alpha, 10.0) * _Alpha;
				return float4(0.0, 0.0, 0.0, alpha);
			}
			
			ENDCG
		}
	}
}