layout (location = 0) in vec2 frag_texCoords;
layout (location = 1) in vec4 frag_tint;
layout (location = 2) in vec4 frag_meta1;
layout (location = 3) in vec4 frag_meta2;

layout (location = 0) out vec4 out_color;

layout (binding = 1) uniform sampler2D uTexture;

void main()
{
    #ifdef BLUR
    
    vec4 color = vec4(0.0);
    vec2 direction = frag_meta1.xy;
    vec2 resolution = frag_meta1.zw;
    vec2 off1 = vec2(1.3846153846) * direction;
    vec2 off2 = vec2(3.2307692308) * direction;
    color += texture(uTexture, frag_texCoords) * 0.2270270270;
    color += texture(uTexture, frag_texCoords + (off1 / resolution)) * 0.3162162162;
    color += texture(uTexture, frag_texCoords - (off1 / resolution)) * 0.3162162162;
    color += texture(uTexture, frag_texCoords + (off2 / resolution)) * 0.0702702703;
    color += texture(uTexture, frag_texCoords - (off2 / resolution)) * 0.0702702703;
    out_color = color;
    
    #else
    out_color = texture(uTexture, frag_texCoords) * frag_tint;
    #endif
}