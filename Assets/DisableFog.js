private var revertFogState = false;
private var revertAmbientState = Color.white;

function OnPreRender () {
	revertFogState = RenderSettings.fog;
	revertAmbientState = RenderSettings.ambientLight;
	RenderSettings.fog = false;
	RenderSettings.ambientLight = Color.white;
}
 
function OnPostRender () {
	RenderSettings.fog = revertFogState;
	RenderSettings.ambientLight = revertAmbientState;
}
 
@script RequireComponent (Camera)