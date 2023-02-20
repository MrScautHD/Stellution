layout (location = 0) in vec2 aPosition;
layout (location = 1) in vec2 aTexCoords;
layout (location = 2) in vec4 aTint;
layout (location = 3) in float aRotation;
layout (location = 4) in vec2 aOrigin;
layout (location = 5) in vec2 aScale;
layout (location = 6) in vec4 aMeta1;
layout (location = 7) in vec4 aMeta2;

layout (location = 0) out vec2 frag_texCoords;
layout (location = 1) out vec4 frag_tint;
layout (location = 2) out vec4 frag_meta1;
layout (location = 3) out vec4 frag_meta2;

layout (binding = 0) uniform ProjView
{
    mat4 uProjView;
};

void main()
{
    float cosRot = cos(aRotation);
    float sinRot = sin(aRotation);
    
    vec2 vertexPos = aPosition - (aOrigin * aScale);
    mat2 rot = mat2(vec2(cosRot, sinRot), vec2(-sinRot, cosRot));
    vertexPos = rot * vertexPos;
    vertexPos += aOrigin * aScale;
    
    gl_Position = uProjView * vec4(vertexPos, 0.0, 1.0);
    frag_texCoords = aTexCoords;
    frag_tint = aTint;
    
    frag_meta1 = aMeta1;
    frag_meta2 = aMeta2;
}