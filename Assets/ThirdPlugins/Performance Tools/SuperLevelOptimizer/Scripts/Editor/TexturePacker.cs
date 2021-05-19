using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEditor;

using Object = UnityEngine.Object;

namespace NGS.SuperLevelOptimizer
{
    public enum AtlasSize { _512 = 512, _1024 = 1024, _2048 = 2048, _4096 = 4096, _8192 = 8192 };

    public static class TexturePacker
    {
        public static void CombineTextures(Renderer[] renderers, CoefficientTable coefficientTable, AtlasSize maxAtlasSize, string saveFolderPath)
        {
            CheckSaveFolderDirectory(saveFolderPath);

            Material[] materials = renderers.SelectMany(r => r.sharedMaterials).Where(m => m != null).Distinct().ToArray();

            List<MaterialGroup> materialGroups = GroupMaterials(materials, coefficientTable, maxAtlasSize);
            materialGroups = materialGroups.Where(g => g.UsedMaterials.Count > 1).ToList();

            if (materialGroups.Count == 0)
            {
                Debug.Log("Unable to create atlases. No matching materials found. Try to increase values in Coefficient Table");
                return;
            }

            Transform root = new GameObject("CombinedMaterials", typeof(SLO_CombinedMark)).transform;

            if (CreateMultimaterials(ref materialGroups))
            {
                int created = ApplyMultimaterials(root, renderers, materialGroups);
                Debug.Log("Created multimaterials for " + created + " objects!");
            }
            else
                OnCancelled(materialGroups);
        }

        private static void CheckSaveFolderDirectory(string saveFolderPath)
        {
            if (!saveFolderPath.EndsWith("/"))
                saveFolderPath += "/";

            if (!Directory.Exists(saveFolderPath))
            {
                Directory.CreateDirectory(saveFolderPath);
                AssetDatabase.Refresh();
            }
        }


        private static Renderer DublicateRenderer(Transform root, Renderer renderer)
        {
            Renderer copy = Object.Instantiate(renderer.gameObject, root).GetComponent<Renderer>();
            copy.gameObject.AddComponent<SLO_AtlasMark>();

            copy.transform.position = renderer.transform.position;
            copy.transform.rotation = renderer.transform.rotation;
            copy.transform.localScale = renderer.transform.lossyScale;

            renderer.gameObject.AddComponent<SLO_SourceMark>();
            renderer.gameObject.SetActive(false);

            return copy;
        }

        private static void SafelyRemoveDublicate(Renderer source, Renderer dublicate)
        {
            if (source != null)
            {
                SLO_SourceMark mark = source.GetComponent<SLO_SourceMark>();

                if (mark != null)
                    Object.DestroyImmediate(mark);

                source.gameObject.SetActive(true);
            }

            if (dublicate != null)
                Object.DestroyImmediate(dublicate.gameObject);
        }


        private static List<MaterialGroup> GroupMaterials(Material[] materials, CoefficientTable coefficientTable, AtlasSize maxAtlasSize)
        {
            
            List<MaterialGroup> materialGroups = new List<MaterialGroup>();

            int idx = 0, count = materials.Length;

            Debug.Log("GroupMaterials:"+materials.Length);

            foreach (var mat in materials)
            {
                try
                {
                    EditorUtility.DisplayProgressBar("Grouping materials...", idx + " of " + count, (float)idx / count);

                    bool isValid = false;

                    foreach (var materialGroup in materialGroups)
                    {
                        if (materialGroup.IsValidMaterial(mat))
                        {
                            isValid = true;

                            materialGroup.AddMaterial(mat);

                            break;
                        }
                    }

                    Debug.Log("mat:"+mat.name+"|"+isValid);

                    if (!isValid)
                        materialGroups.Add(new MaterialGroup(mat, coefficientTable, maxAtlasSize));
                }
                catch(Exception ex)
                {
                    Debug.Log("At runtime exception occurred. Don't worry, it didn't lead to critical consequences. Please inform the developer(e-mail: andre-orsk@yandex.ru)");
                    Debug.Log(ex.Message + " " + ex.StackTrace);
                }

                idx++;
            }

            EditorUtility.ClearProgressBar();

            Debug.Log("GroupMaterials end:"+materialGroups.Count);
            return materialGroups;
        }


        private static bool CreateMultimaterials(ref List<MaterialGroup> materialGroups)
        {
            List<MaterialGroup> computedGroups = new List<MaterialGroup>();

            for (int i = 0; i < materialGroups.Count; i++)
            {
                try
                {
                    if (EditorUtility.DisplayCancelableProgressBar("Creating multimaterials...", i + " of " + materialGroups.Count, (float)i / materialGroups.Count))
                    {
                        EditorUtility.ClearProgressBar();

                        return false;
                    }

                    materialGroups[i].CreateMultimaterial();

                    computedGroups.Add(materialGroups[i]);
                }
                catch (Exception ex)
                {
                    if (materialGroups[i].Multimaterial != null)
                        MaterialUtil.DeleteMultimaterialData(materialGroups[i].Multimaterial);

                    Debug.Log("At runtime exception occurred. Don't worry, it didn't lead to critical consequences. Please inform the developer(e-mail: andre-orsk@yandex.ru)");
                    Debug.Log(ex.Message + "\n" + ex.StackTrace);
                }
            }

            EditorUtility.ClearProgressBar();

            materialGroups = computedGroups;

            return true;
        }

        private static int ApplyMultimaterials(Transform root, Renderer[] renderers, List<MaterialGroup> groups)
        {
            int createdMultimaterials = 0;

            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer sourceRenderer = null, dublicate = null;

                try
                {
                    EditorUtility.DisplayProgressBar("Applying multimaterials...", i + " of " + renderers.Length, (float)i / renderers.Length);

                    sourceRenderer = renderers[i];
                    dublicate = DublicateRenderer(root, sourceRenderer);

                    Material[] materials = dublicate.sharedMaterials;

                    bool modified = false;
                    for (int c = 0; c < materials.Length; c++)
                    {
                        foreach (var group in groups)
                            if (group.UsedMaterials.Contains(materials[c]))
                            {
                                if (group.ApplyMultimaterial(dublicate, materials[c]))
                                {
                                    materials[c] = group.Multimaterial;
                                    modified = true;
                                }

                                break;
                            }
                    }

                    if (modified)
                    {
                        dublicate.sharedMaterials = materials;
                        createdMultimaterials++;
                    }
                    else
                        SafelyRemoveDublicate(sourceRenderer, dublicate);
                }
                catch (Exception ex)
                {
                    Debug.Log("At runtime exception occurred. Don't worry, it didn't lead to critical consequences. Please inform the developer(e-mail: andre-orsk@yandex.ru)");
                    Debug.Log(ex.Message + "\n" + ex.StackTrace);

                    SafelyRemoveDublicate(sourceRenderer, renderers[i]);
                }
            }

            EditorUtility.ClearProgressBar();

            return createdMultimaterials;
        }


        private static void OnCancelled(List<MaterialGroup> materialGroups)
        {
            foreach (var group in materialGroups)
                if (group.Multimaterial != null)
                    MaterialUtil.DeleteMultimaterialData(group.Multimaterial);

            Debug.Log("Canceled");
        }
    }

    public class MaterialGroup
    {
        public Material SourceMaterial { get; private set; }
        public List<Material> UsedMaterials { get { return _sourceMaterials; } }
        public CoefficientTable CoefficientTable { get; private set; }

        public Material Multimaterial { get; private set; }
        public Rect[] AtlasRects { get; private set; }

        public int TexturesCount { get { return _shaderTexturePropNames.Count; } }
        public Vector2 FreeSpace { get; private set; }

        private List<Material> _sourceMaterials = new List<Material>();
        private List<Texture2D[]> _sourceTextures = new List<Texture2D[]>();

        private List<string> _shaderPropNames = new List<string>();
        private List<ShaderUtil.ShaderPropertyType> _shaderPropTypes = new List<ShaderUtil.ShaderPropertyType>();
        private List<string> _shaderTexturePropNames;


        public MaterialGroup(Material sourceMaterial, CoefficientTable coefficientTable, AtlasSize maxAtlasSize)
        {
            SourceMaterial = sourceMaterial;

            CoefficientTable = coefficientTable;

            FreeSpace = new Vector2(4096, 4096);

            Shader shader = SourceMaterial.shader;
            for (int i = 0; i < ShaderUtil.GetPropertyCount(shader); i++)
            {
                _shaderPropNames.Add(ShaderUtil.GetPropertyName(shader, i));
                _shaderPropTypes.Add(ShaderUtil.GetPropertyType(shader, i));
            }

            AddMaterial(SourceMaterial);
        }


        public bool IsValidMaterial(Material material)
        {
            if (material.shader != SourceMaterial.shader)
                return false;

            for (int i = 0; i < _shaderPropNames.Count; i++)
            {
                string propertyName = _shaderPropNames[i];
                ShaderUtil.ShaderPropertyType propertyType = _shaderPropTypes[i];

                if (propertyType == ShaderUtil.ShaderPropertyType.Float)
                {
                    if (!CoefficientTable.IsEqual(SourceMaterial.GetFloat(propertyName), material.GetFloat(propertyName)))
                        return false;
                }
                else if (propertyType == ShaderUtil.ShaderPropertyType.Vector)
                {
                    if (!CoefficientTable.IsEqual(SourceMaterial.GetVector(propertyName), material.GetVector(propertyName)))
                        return false;
                }
                else if (propertyType == ShaderUtil.ShaderPropertyType.Color)
                {
                    if (!CoefficientTable.IsEqual(SourceMaterial.GetColor(propertyName), material.GetColor(propertyName)))
                        return false;
                }
                else if (propertyType == ShaderUtil.ShaderPropertyType.TexEnv)
                {
                    if ((SourceMaterial.GetTexture(propertyName) == null) != (material.GetTexture(propertyName) == null))
                        return false;
                }
            }

            Texture2D[] textures = MaterialUtil.GetMaterialTextures(material, ref _shaderTexturePropNames);

            if (textures != null && textures.Length > 0)
            {
                Vector2 freeSpace = FreeSpace - new Vector2(textures.Max(t => t.width), textures.Max(t => t.height));

                if (freeSpace.x < 0 || freeSpace.y < 0)
                    return false;
            }

            return true;
        }

        public void AddMaterial(Material material)
        {
            if (_sourceMaterials.Contains(material))
                return;

            Texture2D[] textures = MaterialUtil.GetMaterialTextures(material, ref _shaderTexturePropNames);

            if (textures != null && textures.Length > 0)
            {
                Vector2 size = new Vector2(textures.Max(t => t.width), textures.Max(t => t.height));
                FreeSpace -= size;
            }

            _sourceMaterials.Add(material);

            _sourceTextures.Add(textures);
        }


        public void CreateMultimaterial()
        {
            Multimaterial = AssetsManager.CreateAsset(new Material(SourceMaterial), SuperLevelOptimizer.DataFolder + "Multimaterials/", "mul.mat_" + SourceMaterial.GetHashCode());

            if (TexturesCount > 0)
            {
                NormalizeTextures();

                foreach (var texName in _shaderTexturePropNames)
                {
                    Multimaterial.SetTextureOffset(texName, Vector2.zero);
                    Multimaterial.SetTextureScale(texName, Vector2.one);
                }

                CreateAtlases();
            }
        }
      
        public bool ApplyMultimaterial(Renderer renderer, Material sourceMaterial)
        {
            if (!UsedMaterials.Contains(sourceMaterial))
                return false;

            if (TexturesCount == 0)
                return true;

            int subMeshIndex = renderer.sharedMaterials.IndexOf(sourceMaterial);

            MeshFilter filter = renderer.GetComponent<MeshFilter>();
            filter.sharedMesh = Object.Instantiate(filter.sharedMesh);

            if (filter.sharedMesh.subMeshCount <= subMeshIndex)
                return false;

            List<Vector2> uv = new List<Vector2>();
            for (int i = 0; i < SuperLevelOptimizer.UVsCount; i++)
            {
                filter.sharedMesh.GetUVs(i, uv);

                if (uv.Count != 0)
                    break;

                if (i == SuperLevelOptimizer.UVsCount - 1)
                {
                    Debug.Log("Impossible to create atlas for GameObject : " + renderer.gameObject.name + " because of UV size is zero");
                    return false;
                }
            }

            Rect rect = AtlasRects[UsedMaterials.IndexOf(sourceMaterial)];

            int[] triangles = filter.sharedMesh.GetTriangles(subMeshIndex).Distinct().ToArray();
            foreach (var t in triangles)
            {
                if (uv[t].x > 1.00001 || uv[t].x < -0.00001 || uv[t].y > 1.00001 || uv[t].y < -0.00001)
                {
                    Debug.Log("Impossible to create atlas for GameObject : " + renderer.gameObject.name + " because of non normalized UV coordinates");
                    return false;
                }

                uv[t] = new Vector2((uv[t].x * rect.width) + rect.x, (uv[t].y * rect.height) + rect.y);
            }

            filter.sharedMesh.uv = uv.ToArray();

            return true;
        }


        private void NormalizeTextures()
        {
            for (int i = 0; i < _sourceTextures.Count; i++)
            {
                foreach(var tex in _sourceTextures[i])
                    TextureUtil.AllowAccessToTexture(tex);

                _sourceTextures[i] = TextureUtil.NormalizeTexturesSize(_sourceTextures[i]);

                _sourceTextures[i] = TextureUtil.TileTextures(_sourceTextures[i], _shaderTexturePropNames.ToArray(), _sourceMaterials[i]);
            }
        }

        private void CreateAtlases()
        {
            AtlasRects = new Rect[0];

            for (int i = 0; i < _sourceTextures[0].Length; i++)
            {
                Texture2D[] texturesForAtlas = new Texture2D[_sourceTextures.Count];

                for (int c = 0; c < _sourceTextures.Count; c++)
                    texturesForAtlas[c] = _sourceTextures[c][i];

                Texture2D atlas = new Texture2D(0, 0);

                Rect[] rects = atlas.PackTextures(texturesForAtlas, 4, 8192, false);

                atlas = TextureUtil.CreatePNG(atlas, SuperLevelOptimizer.DataFolder + "Atlases/", "mul.tex_" + atlas.GetInstanceID());

                TextureUtil.CopyTextureParametrs(_sourceTextures[0][i], atlas);

                Multimaterial.SetTexture(_shaderTexturePropNames[i], atlas);

                if (AtlasRects.Length == 0)
                    AtlasRects = rects;
            }
        }
    }

    public static class MaterialUtil
    {
        public static Texture2D[] GetMaterialTextures(Material material, ref List<string> textureNames)
        {
            List<Texture2D> textures = new List<Texture2D>();

            Shader shader = material.shader;

            if (textureNames == null || textureNames.Count == 0)
            {
                textureNames = new List<string>();

                for (int i = 0; i < ShaderUtil.GetPropertyCount(shader); i++)
                {
                    if (ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
                        if (ShaderUtil.GetTexDim(shader, i) == UnityEngine.Rendering.TextureDimension.Tex2D)
                        {
                            string texName = ShaderUtil.GetPropertyName(shader, i);

                            if (material.GetTexture(texName) as Texture2D != null)
                                textureNames.Add(texName);
                        }
                }

            }

            foreach (var texName in textureNames)
                textures.Add(material.GetTexture(texName) as Texture2D);

            return textures.ToArray();
        }

        public static void DeleteMultimaterialData(Material material)
        {
            if (material == null || !material.name.StartsWith("mul.mat_"))
                return;

            if (!AssetsManager.ContainsAsset(material))
                return;

            List<string> texNames = null;
            Texture2D[] textures = GetMaterialTextures(material, ref texNames);

            for (int i = 0; i < textures.Length; i++)
                if(textures[i].name.StartsWith("mul.tex_"))
                    AssetsManager.DeleteAsset(textures[i]);

            AssetsManager.DeleteAsset(material);
        }

        public static int IndexOf(this Material[] materials, Material mat)
        {
            for (int i = 0; i < materials.Length; i++)
                if (materials[i] == mat)
                    return i;

            return -1;
        }
    }

    public static class TextureUtil
    {
        private static List<Texture2D> resizedOriginals = new List<Texture2D>();
        private static List<Texture2D> resizedTexturesCache = new List<Texture2D>();
        private static List<Vector2> sizes = new List<Vector2>();


        private static List<Texture2D> tiledOriginals = new List<Texture2D>();
        private static List<Texture2D> tiledTexturesCache = new List<Texture2D>();
        private static List<Vector2> tiling = new List<Vector2>();


        public static void AllowAccessToTexture(Texture2D texture)
        {
            string texturePath = AssetDatabase.GetAssetPath(texture);

            if (texturePath == null || texturePath == "")
                return;

            TextureImporter importer = (TextureImporter) AssetImporter.GetAtPath(texturePath);

            if (importer.isReadable && importer.textureCompression == TextureImporterCompression.Uncompressed)
                return;

            importer.isReadable = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;

            importer.SaveAndReimport();
        }

        public static void CopyTextureParametrs(Texture2D source, Texture2D copy)
        {
            string sourceTextureFilePath = AssetDatabase.GetAssetPath(source);
            string copyTextureFilePath = AssetDatabase.GetAssetPath(copy);

            if (sourceTextureFilePath == "" || copyTextureFilePath == "")
                return;

            TextureImporter sourceImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(source)) as TextureImporter;
            TextureImporter copyImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(copy)) as TextureImporter;

            if (sourceImporter == null || copyImporter == null)
                return;

            copyImporter.textureType = sourceImporter.textureType;

            TextureImporterSettings settings = new TextureImporterSettings();

            sourceImporter.ReadTextureSettings(settings);
            copyImporter.SetTextureSettings(settings);

            copyImporter.SaveAndReimport();
        }

        public static Texture2D CreatePNG(Texture2D texture, string folderPath, string textureName)
        {
            if (!folderPath.EndsWith("/"))
                folderPath += "/";

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string fullPath = folderPath + (textureName.EndsWith(".png") ? textureName : textureName + ".png");

            byte[] bytes = texture.EncodeToPNG();

            using (var stream = new FileStream(fullPath, FileMode.Create, FileAccess.ReadWrite))
            using (BinaryWriter writer = new BinaryWriter(stream))
                writer.Write(bytes);

            AssetDatabase.ImportAsset(fullPath);
            AllowAccessToTexture(AssetDatabase.LoadAssetAtPath(fullPath, typeof(Texture2D)) as Texture2D);

            return (Texture2D) AssetDatabase.LoadAssetAtPath(fullPath, typeof(Texture2D));
        }


        public static Texture2D[] NormalizeTexturesSize(Texture2D[] textures)
        {
            int width = textures[0].width;
            int height = textures[0].height;

            for (int i = 0; i < textures.Length; i++)
                textures[i] = ResizeTexture(textures[i], width, height);

            return textures;
        }

        public static Texture2D ResizeTexture(Texture2D texture, int width, int height)
        {
            if (texture.width == width && texture.height == height)
                return texture;

            Texture2D result = GetResizedTextureFromCache(texture, width, height);

            if (result != null)
                return result;

            result = new Texture2D(width, height, TextureFormat.RGBA32, false);
            Color[] rpixels = result.GetPixels();

            float incX = (1.0f / width);
            float incY = (1.0f / height);

            for (int px = 0; px < rpixels.Length; px++)
                rpixels[px] = texture.GetPixelBilinear(incX * ((float)px % width), incY * (Mathf.Floor(px / width)));

            result.SetPixels(rpixels);
            result.Apply(true, false);

            SetResizedTextureToCache(texture, result, width, height);

            return result;
        }


        public static Texture2D[] TileTextures(Texture2D[] textures, string[] textureNames, Material material)
        {
            Vector2 tiling = material.shader.name.Contains("Standard") ? material.GetTextureScale("_MainTex") : Vector2.one;

            for (int i = 0; i < textures.Length; i++)
            {
                if (!material.shader.name.Contains("Standard"))
                    tiling = material.GetTextureScale(textureNames[i]);

                else if (textureNames[i] == "_DetailAlbedoMap")
                    tiling = material.GetTextureScale(textureNames[i]);

                if (Mathf.Approximately(tiling.x, 1) && Mathf.Approximately(tiling.y, 1))
                    continue;

                textures[i] = TileTexture(textures[i], tiling);
            }

            return textures;
        }

        public static Texture2D TileTexture(Texture2D texture, Vector2 tiling)
        {
            if (Mathf.RoundToInt(tiling.x) == 0 || Mathf.RoundToInt(tiling.y) == 0)
                return texture;

            Texture2D tex = GetTiledTextureFromCache(texture, tiling);

            if (tex != null)
                return tex;

            tex = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, true);

            int t_width = texture.width / Mathf.RoundToInt(tiling.x);
            int t_height = texture.height / Mathf.RoundToInt(tiling.y);

            int offset_x = 0, offset_y = 0;
            for (int i = 0; i <= Mathf.RoundToInt(tiling.x); i++)
            {
                for (int c = 0; c <= Mathf.RoundToInt(tiling.y); c++)
                {
                    int x = 0;
                    for (int p = 0; p < texture.width; p += Mathf.RoundToInt(tiling.x))
                    {
                        int y = 0;
                        for (int j = 0; j < texture.height; j += Mathf.RoundToInt(tiling.y))
                        {
                            tex.SetPixel(x + offset_x, y + offset_y, texture.GetPixel(p, j));
                            y++;
                        }
                        x++;
                    }
                    offset_y += t_height;
                }
                offset_x += t_width;
            }

            tex.Apply(true, false);

            SetTiledTextureToCache(texture, tex, tiling);

            return tex;
        }


        private static Texture2D GetResizedTextureFromCache(Texture2D original, int width, int height)
        {
            for (int i = 0; i < resizedOriginals.Count; i++)
            {
                if (resizedOriginals[i] == original)
                    if (Mathf.Approximately(width, sizes[i].x) && Mathf.Approximately(height, sizes[i].y))
                        return resizedTexturesCache[i];
            }

            return null;
        }

        private static void SetResizedTextureToCache(Texture2D original, Texture2D resized, int width, int height)
        {
            resizedOriginals.Add(original);

            resizedTexturesCache.Add(resized);

            sizes.Add(new Vector2(width, height));
        }


        private static Texture2D GetTiledTextureFromCache(Texture2D original, Vector2 tile)
        {
            for (int i = 0; i < tiledOriginals.Count; i++)
            {
                if (tiledOriginals[i] == original)
                    if (tiling[i] == tile)
                        return tiledTexturesCache[i];
            }

            return null;
        }

        private static void SetTiledTextureToCache(Texture2D original, Texture2D tiled, Vector2 tile)
        {
            tiledOriginals.Add(original);

            tiledTexturesCache.Add(tiled);

            tiling.Add(tile);
        }
    }
}
