using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Easel.Core;
using Easel.Graphics.Materials;
using Silk.NET.Assimp;
using Material = Easel.Graphics.Materials.Material;

namespace Easel.Graphics;

/// <summary>
/// Represents a model and scene graph.
/// </summary>
public unsafe class Model
{
    private static Assimp _assimp;

    public readonly ModelMesh[] Meshes;

    public readonly Material[] Materials;

    public Model(string path)
    {
        _assimp ??= Assimp.GetApi();

        Scene* scene = _assimp.ImportFile(path,
            (uint) PostProcessSteps.Triangulate |
            (uint) PostProcessSteps.GenerateUVCoords | (uint) PostProcessSteps.JoinIdenticalVertices |
            (uint) PostProcessSteps.CalculateTangentSpace | (uint) PostProcessSteps.PreTransformVertices |
            (uint) PostProcessSteps.MakeLeftHanded | (uint) PostProcessSteps.FlipUVs | (uint) PostProcessSteps.GenerateSmoothNormals);

        if (scene == null || (scene->MFlags & Assimp.SceneFlagsIncomplete) != 0 ||
            scene->MRootNode == null)
        {
            throw new EaselException("Failed to load assimp: " + _assimp.GetErrorStringS());
        }

        Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

        Texture2D[] LoadTexturesForMatType(Silk.NET.Assimp.Material* material, TextureType type)
        {
            Texture2D[] texts = new Texture2D[_assimp.GetMaterialTextureCount(material, type)];
            
            for (int i = 0; i < texts.Length; i++)
            {
                if (i > 0)
                    throw new NotImplementedException("Currently materials can only support one texture per type.");
                
                AssimpString aPath;
                TextureMapping mapping;
                uint uvIndex;
                float blend;
                TextureOp op;
                TextureMapMode mode;
                uint flags;

                _assimp.GetMaterialTexture(material, type, (uint) i, &aPath, &mapping, &uvIndex, &blend, &op, &mode,
                    &flags);

                if (!textures.TryGetValue(aPath.AsString, out Texture2D texture))
                {
                    string fullPath = Path.Combine(Path.GetDirectoryName(path), aPath.AsString);
                    Console.WriteLine(fullPath);
                    texture = new Texture2D(fullPath);
                    textures.Add(aPath.AsString, texture);
                }

                texts[i] = texture;
            }

            return texts;
        }

        Materials = new Material[scene->MNumMaterials];
        for (int i = 0; i < scene->MNumMaterials; i++)
        {
            Silk.NET.Assimp.Material* material = scene->MMaterials[i];
            
            Texture2D[] albedo = LoadTexturesForMatType(material, TextureType.Diffuse);
            Texture2D[] normal = LoadTexturesForMatType(material, TextureType.Normals);
            Texture2D[] metallic = LoadTexturesForMatType(material, TextureType.Metalness);
            Texture2D[] roughness = LoadTexturesForMatType(material, TextureType.DiffuseRoughness);
            Texture2D[] ao = LoadTexturesForMatType(material, TextureType.AmbientOcclusion);

            Materials[i] = new StandardMaterial(albedo.Length > 0 ? albedo[0] : Texture2D.White,
                normal.Length > 0 ? normal[0] : Texture2D.EmptyNormal,
                metallic.Length > 0 ? metallic[0] : Texture2D.Black,
                roughness.Length > 0 ? roughness[0] : Texture2D.Black, ao.Length > 0 ? ao[0] : Texture2D.White);
        }

        List<VertexPositionTextureNormalTangent> vptnts = new List<VertexPositionTextureNormalTangent>();
        List<uint> indices = new List<uint>();

        Mesh[] meshes = new Mesh[scene->MNumMeshes];
        for (int i = 0; i < scene->MNumMeshes; i++)
        {
            vptnts.Clear();
            indices.Clear();
            
            Silk.NET.Assimp.Mesh* mesh = scene->MMeshes[i];

            for (int v = 0; v < mesh->MNumVertices; v++)
                vptnts.Add(new VertexPositionTextureNormalTangent(mesh->MVertices[v], mesh->MTextureCoords[0][v].ToVector2(), mesh->MNormals == null ? Vector3.Zero : mesh->MNormals[v], mesh->MTangents == null ? Vector3.Zero : mesh->MTangents[0]));

            for (int f = 0; f < mesh->MNumFaces; f++)
            {
                Face face = mesh->MFaces[f];
                for (int t = 0; t < face.MNumIndices; t++)
                    indices.Add(face.MIndices[t]);
            }

            meshes[i] = new Mesh(vptnts.ToArray(), indices.ToArray(), Materials[mesh->MMaterialIndex]);
        }

        List<ModelMesh> mmeshes = new List<ModelMesh>();
        ProcessNode(scene->MRootNode, mmeshes, meshes, Matrix4x4.Identity);
        Meshes = mmeshes.ToArray();
    }

    private void ProcessNode(Node* node, List<ModelMesh> mmeshes, Mesh[] allMeshes, Matrix4x4 transform)
    {
        Mesh[] meshes = new Mesh[node->MNumMeshes];
        for (int i = 0; i < node->MNumMeshes; i++)
            meshes[i] = allMeshes[node->MMeshes[i]];
        transform = node->MTransformation * transform;
        mmeshes.Add(new ModelMesh(meshes, Matrix4x4.Identity));
        
        for (int i = 0; i < node->MNumChildren; i++)
            ProcessNode(node->MChildren[i], mmeshes, allMeshes, transform);
    }
}