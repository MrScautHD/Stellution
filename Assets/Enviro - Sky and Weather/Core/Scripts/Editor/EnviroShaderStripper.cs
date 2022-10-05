//#define SHADER_COMPILATION_LOGGING
//#define SKIP_SHADER_COMPILATION

using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

public class ShaderStripper : IPreprocessShaders
{
	private const string LOG_FILE_PATH = "Library/Shader Compilation Results.txt";

	private static readonly ShaderKeyword[] SKIPPED_VARIANTS = new ShaderKeyword[]
	{
		new ShaderKeyword( "ENVIROHDRP" ),
		new ShaderKeyword( "ENVIROURP" ),
	};

	public int callbackOrder { get { return 0; } }

	public void OnProcessShader( Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data )
	{
		// Don't strip essential shaders
		string shaderName = shader.name;

//URP Shader
#if !ENVIRO_LWRP
	if(shaderName == "Hidden/EnviroBlitThrough")
		data.Clear();
#endif

//HDRP Shaders
#if !ENVIRO_HDRP
		if(shaderName == "Hidden/Enviro/BlitTroughHDRP")
			data.Clear();
 
		if(shaderName == "Enviro/Pro/LightShafts")
			data.Clear();

		if(shaderName == "Enviro/HDRP/Skybox Lite")
			data.Clear();

		if(shaderName == "Enviro/HDRP/Skybox")
			data.Clear();

		if(shaderName == "Enviro/Pro/EnviroFogHDRP")
			data.Clear();

		if(shaderName == "Hidden/EnviroDistanceBlurHDRP")
			data.Clear();

		if(shaderName == "Enviro/Pro/CloudsBlit")
			data.Clear();

#endif
	}
}

