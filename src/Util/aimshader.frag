#version 330 core

in vec2 uv;

out vec4 outColor;

uniform float time;
uniform float xs;
uniform float ys; // texture resolution
uniform int isWhite;
uniform sampler2D colorTexture;

void main () {
    vec4 color = texture2D(colorTexture, uv);

    if (uv[0] * xs < (xs/2) - (ys/2) - 2 || uv[0] * xs > (xs/2) + (ys/2) + 2) {
        color = color * 0.5;
    }

    outColor = (isWhite == 1) ? vec4(1, 1, 1, 1) : color;
}