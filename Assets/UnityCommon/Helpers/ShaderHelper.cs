using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class ShaderHelper
{
    private static Shader TransparentDiffuseShader;

    private static Shader TransparentDiffuseSpecular;

    private static Shader TransparentAlphaBlendShader;

    private static void InitTransparentShader()
    {
        if (TransparentDiffuseShader == null)
        {
            TransparentDiffuseShader = Shader.Find("Transparent/Diffuse");
            //TransparentDiffuseShader = Shader.Find("Legacy Shaders/Transparent/Specular");
        }

        if (TransparentDiffuseSpecular == null)
        {
            TransparentDiffuseSpecular = Shader.Find("Legacy Shaders/Transparent/Specular");
            //shader = Shader.Find("Transparent/Specular");
        }

        if (TransparentAlphaBlendShader == null)
        {
            TransparentAlphaBlendShader = Shader.Find("MMShader/TransparentAlphaBlend");
        }
    }

    public static Shader GetTransparentDiffuseShader()
    {
        InitTransparentShader();
        return TransparentDiffuseShader;
    }

    public static Shader GetTransparentAlphaBlendShader()
    {
        InitTransparentShader();
        return TransparentAlphaBlendShader;
    }

    public static Shader GetTransparentDiffuseSpecular()
    {
        InitTransparentShader();
        return TransparentDiffuseSpecular;
    }

    public static Shader GetTransparentShader(bool isSpecular)
    {
        InitTransparentShader();

        Shader shader = null;
        if (isSpecular)
        {
            //shader = Shader.Find("Transparent/Specular");
            shader = TransparentDiffuseSpecular;
        }
        else
        {
            shader = TransparentDiffuseShader;
        }
        return shader;
    }
}
