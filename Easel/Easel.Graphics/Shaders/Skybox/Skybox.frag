#version 450

layout (location = 0) in vec3 frag_texCoords;

layout (location = 0) out vec4 out_color;

layout (binding = 1) uniform samplerCube uSkybox;

void main() 
{
    out_color = texture(uSkybox, frag_texCoords);
}