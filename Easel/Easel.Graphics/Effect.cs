using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Easel.Core;
using Microsoft.VisualBasic.CompilerServices;
using Pie;
using Pie.ShaderCompiler;
using Utils = Easel.Core.Utils;

namespace Easel.Graphics;

/// <summary>
/// Represents a shader effect with a vertex and fragment/pixel shader.
/// </summary>
public class Effect : IDisposable
{
    private static bool _displayEffects;
    private static bool _hasCheckedForDisplayEffects;
    
    /// <summary>
    /// The native Pie <see cref="Shader"/> object.
    /// </summary>
    public readonly Shader PieShader;

    /// <summary>
    /// Create a new <see cref="Effect"/> with the given vertex and fragment/pixel shader.
    /// </summary>
    /// <param name="vertex">The vertex shader to use.</param>
    /// <param name="fragment">The fragment/pixel shader to use.</param>
    /// <param name="loadType">The <see cref="EffectLoadType"/> that both shaders will be loaded with</param>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public Effect(string vertex, string fragment, EffectLoadType loadType = EffectLoadType.EmbeddedResource, params string[] defines)
    {
        GraphicsDevice device = EaselGraphics.Instance.PieGraphics;
        switch (loadType)
        {
            case EffectLoadType.String:
                // Do nothing
                break;
            case EffectLoadType.File:
                vertex = File.ReadAllText(vertex);
                fragment = File.ReadAllText(fragment);
                break;
            case EffectLoadType.EmbeddedResource:
                vertex = Utils.LoadEmbeddedString(Assembly.GetExecutingAssembly(), vertex);
                fragment = Utils.LoadEmbeddedString(Assembly.GetExecutingAssembly(), fragment);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(loadType), loadType, null);
        }

        StringBuilder defineBuilder = new StringBuilder("#version 450\n");
        foreach (string define in defines)
            defineBuilder.AppendLine("#define " + define);

        vertex = vertex.Insert(0, defineBuilder.ToString());
        fragment = fragment.Insert(0, defineBuilder.ToString());

        vertex = PreProcess(vertex);
        fragment = PreProcess(fragment);

#if DEBUG
        if (!_hasCheckedForDisplayEffects)
        {
            _hasCheckedForDisplayEffects = true;
            Logger.Debug($"Checking for {EnvVars.PrintEffects}...");
            string? pr = Environment.GetEnvironmentVariable(EnvVars.PrintEffects);
            if (pr is "1")
            {
                _displayEffects = true;
                Logger.Info($"{EnvVars.PrintEffects} is enabled. Effects will be printed to console.");
            }
        }

        if (_displayEffects)
        {
            LogShader(ShaderStage.Vertex, vertex);
            LogShader(ShaderStage.Fragment, fragment);
        }
#endif
        
        Logger.Debug("Compiling shader...");
        
        PieShader = device.CreateCrossPlatformShader(
            new ShaderAttachment(ShaderStage.Vertex, vertex),
            new ShaderAttachment(ShaderStage.Fragment, fragment));
    }

    public void Dispose()
    {
        PieShader.Dispose();
        Logger.Debug("Effect disposed.");
    }

    private static string PreProcess(string shader)
    {
        string[] splitText = shader.Split('\n');

        bool hasIncluded = false;

        for (int i = 0; i < splitText.Length; i++)
        {
            string line = splitText[i];

            if (line.StartsWith("#include"))
            {
                shader = shader.Replace(line, Utils.LoadEmbeddedString(Assembly.GetExecutingAssembly(), line[("#include ".Length)..].Trim().Trim('"')));
                hasIncluded = true;
            }
        }
        
        return hasIncluded ? PreProcess(shader) : shader;
    }

    [Conditional("DEBUG")]
    private void LogShader(ShaderStage stage, string shader)
    {
        string[] shaderLines = shader.Split('\n');
        StringBuilder builder = new StringBuilder($"{stage} shader:\n");
        for (int i = 0; i < shaderLines.Length; i++)
            builder.AppendLine(i + 1 + ": " + shaderLines[i]);
        Logger.Debug(builder.ToString());
    }
}

/// <summary>
/// Represents the possible ways an effect can load its given shaders.
/// </summary>
public enum EffectLoadType
{
    /// <summary>
    /// The shader is stored as a string. (Provide shader code).
    /// </summary>
    String,
    
    /// <summary>
    /// The shader is stored as a file. (Provide path to shader file).
    /// </summary>
    File,
    
    /// <summary>
    /// The shader is stored as an embedded resource. (Provide an assembly namespace).
    /// </summary>
    EmbeddedResource
}