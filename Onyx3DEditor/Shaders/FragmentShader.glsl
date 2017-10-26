﻿#version 330

in vec3 o_color;
in vec3 o_normal;
in vec3 o_fragpos;

out vec4 fragColor;

vec3 lightdir = vec3(0,1,2);

void main()
{
    float nDotL = dot(normalize(o_normal), normalize(lightdir));
    fragColor = vec4(vec3(nDotL),1);
}
