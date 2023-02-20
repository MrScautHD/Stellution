#version 450

layout (location = 0) in vec3 aPosition;

layout (location = 0) out vec3 frag_texCoords;

layout (binding = 0) uniform CameraInfo
{
    mat4 uProjection;
    mat4 uView;
};

void main() 
{
    frag_texCoords = aPosition;
    gl_Position = (uProjection * uView * vec4(aPosition, 1.0)).xyww;
}