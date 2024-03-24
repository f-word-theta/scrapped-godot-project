shader_type canvas_item;

// Combined Version of 2 Shaders below + SCALING + WOBBLING + DIFF.SPEEDS
// Many Spinning Mono Star-Flowers
// https://www.shadertoy.com/view/3dsSWN
// Spinning Rubber Gold Star
// https://www.shadertoy.com/view/wdlSW4

uniform vec2 iResolution = vec2 (75.0, 75.0);
const float TWO_PI = 6.28318530718;

// Helper Functions for Perlin Noise
vec4 mod289(vec4 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
vec4 permute(vec4 x) { return mod289(((x*34.0)+1.0)*x); }
vec4 taylorInvSqrt(vec4 r) { return 1.79284291400159 - 0.85373472095314 * r; }
vec2 fade(vec2 t) { return t*t*t*(t*(t*6.0-15.0)+10.0); }

// Helper Functions for Simplex Noise
vec3 mod289S(vec3 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
vec2 mod289_A(vec2 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
vec3 permuteS(vec3 x) { return mod289S(((x*34.0)+1.0)*x); }

// Classic Perlin noise
float classicPerlin(vec2 P) {
    vec4 Pi = floor(P.xyxy) + vec4(0.0, 0.0, 1.0, 1.0);
    vec4 Pf = fract(P.xyxy) - vec4(0.0, 0.0, 1.0, 1.0);
    Pi = mod289(Pi); // To avoid truncation effects in permutation
    vec4 ix = Pi.xzxz;
    vec4 iy = Pi.yyww;
    vec4 fx = Pf.xzxz;
    vec4 fy = Pf.yyww;

    vec4 i = permute(permute(ix) + iy);

    vec4 gx = fract(i * (1.0 / 41.0)) * 2.0 - 1.0 ;
    vec4 gy = abs(gx) - 0.5 ;
    vec4 tx = floor(gx + 0.5);
    gx = gx - tx;

    vec2 g00 = vec2(gx.x,gy.x);
    vec2 g10 = vec2(gx.y,gy.y);
    vec2 g01 = vec2(gx.z,gy.z);
    vec2 g11 = vec2(gx.w,gy.w);

    vec4 norm = taylorInvSqrt(vec4(dot(g00, g00), dot(g01, g01), dot(g10, g10), dot(g11, g11)));
    g00 *= norm.x;
    g01 *= norm.y;
    g10 *= norm.z;
    g11 *= norm.w;

    float n00 = dot(g00, vec2(fx.x, fy.x));
    float n10 = dot(g10, vec2(fx.y, fy.y));
    float n01 = dot(g01, vec2(fx.z, fy.z));
    float n11 = dot(g11, vec2(fx.w, fy.w));

    vec2 fade_xy = fade(Pf.xy);
    vec2 n_x = mix(vec2(n00, n01), vec2(n10, n11), fade_xy.x);
    float n_xy = mix(n_x.x, n_x.y, fade_xy.y);
    return 2.3 * n_xy;
	}

// Description : GLSL 2D simplex noise function
//      Author : Ian McEwan, Ashima Arts
//  Maintainer : ijm
//     Lastmod : 20110822 (ijm)
//     License :
//  Copyright (C) 2011 Ashima Arts. All rights reserved.
//  Distributed under the MIT License. See LICENSE file.
//  https://github.com/ashima/webgl-noise
float simplex(vec2 v) {
    // Precompute values for skewed triangular grid
    const vec4 C = vec4(0.211324865405187,
                        // (3.0-sqrt(3.0))/6.0
                        0.366025403784439,
                        // 0.5*(sqrt(3.0)-1.0)
                        -0.577350269189626,
                        // -1.0 + 2.0 * C.x
                        0.024390243902439);
                        // 1.0 / 41.0
    // First corner (x0)
    vec2 i  = floor(v + dot(v, C.yy));
    vec2 x0 = v - i + dot(i, C.xx);
    // Other two corners (x1, x2)
    vec2 i1 = vec2(0.0);
    i1 = (x0.x > x0.y)? vec2(1.0, 0.0):vec2(0.0, 1.0);
    vec2 x1 = x0.xy + C.xx - i1;
    vec2 x2 = x0.xy + C.zz;
    // Do some permutations to avoid
    // truncation effects in permutation
    i = mod289_A(i);
    vec3 p = permuteS(
            permuteS( i.y + vec3(0.0, i1.y, 1.0))
                + i.x + vec3(0.0, i1.x, 1.0 ));
    vec3 m = max(0.5 - vec3(
                        dot(x0,x0),
                        dot(x1,x1),
                        dot(x2,x2)
                        ), 0.0);
    m = m*m ;
    m = m*m ;
    // Gradients:
    //  41 pts uniformly over a line, mapped onto a diamond
    //  The ring size 17*17 = 289 is close to a multiple
    //      of 41 (41*7 = 287)
    vec3 x = 2.0 * fract(p * C.www) - 1.0;
    vec3 h = abs(x) - 0.5;
    vec3 ox = floor(x + 0.5);
    vec3 a0 = x - ox;
    // Normalise gradients implicitly by scaling m
    // Approximation of: m *= inversesqrt(a0*a0 + h*h);
    m *= 1.79284291400159 - 0.85373472095314 * (a0*a0+h*h);
    // Compute final noise value at P
    vec3 g = vec3(0.0);
    g.x  = a0.x  * x0.x  + h.x  * x0.y;
    g.yz = a0.yz * vec2(x1.x,x2.x) + h.yz * vec2(x1.y,x2.y);
    return 130.0 * dot(m, g);
	}

vec3 hsb2rgb( in vec3 c ){
    vec3 rgb = clamp(abs(mod(c.x*6.0+vec3(0.0,4.0,2.0),
                             6.0)-3.0)-1.0,
                     0.0,
                     1.0 );
    rgb = rgb*rgb*(3.0-2.0*rgb);
    return c.z * mix(vec3(1.0), rgb, c.y);
	}
float smoothedge(float v, float res) {
	return smoothstep(0.0, res, v);
	}
vec2 rotate(vec2 v, float a) {
	float s = sin(a);
	float c = cos(a);
	mat2 m = mat2(vec2(c, -s), vec2(s, c));
	return m * v;
	}
float rect(vec2 p, vec2 size, vec2 uvMain, float coef) {
	p -= 0.5;
	uvMain = uvMain * coef + 1.0;
	float uvMainF = sqrt(floor(uvMain.x) + floor(uvMain.y));
	p = rotate(p, uvMainF * (cos(TWO_PI * abs(fract(TIME / 10. - 0.5))) - 2.0)) ;
	vec2 d = abs(p) - size * 0.25 * (sin(TIME) + 2.0) * uvMainF;
	return min(max(d.x, d.y), 0.0) + length(max(d,0.0));
	}
float triangle(vec2 p, float size, vec2 uvMain, float coef) {
	p -= 0.5;
	uvMain = uvMain * coef + 1.0;
	float uvMainF = sqrt(floor(uvMain.x) + floor(uvMain.y));
	p = rotate(p, uvMainF * (cos(TWO_PI * abs(fract(TIME / 20. - 0.5))) - 1.0)) ;
    vec2 q = abs(p) - size * 0.25 * (sin(TIME) + 2.0) * uvMainF;
    return max(q.x * 0.866025 + p.y * 0.5, -p.y * 0.5) - size * 0.5;
	}

void fragment() {
	vec2 uvMain = UV;
	float fraction = 5.0;
	vec2 uvFraction = fract(uvMain * fraction) + 0.5;
	float d = 0.09;
	d = min(d, rect(uvFraction - vec2(0.5, 0.5), vec2(0.15, .10), uvMain, fraction));
//	d = min(d, triangle(uvFraction - vec2(0.5, 0.5), 0.09, uvMain, fraction));
	
	float colorF = smoothedge(d, 1.0 / iResolution.x);
	float hsbRG = hsb2rgb(vec3(ceil((1.0 - uvMain.x) * fraction)) / fraction).r;
	vec2 hsbB = hsb2rgb(vec3(ceil((1.0 - uvMain.y) * fraction)) / fraction).gb;
	vec3 color = vec3(1.0 - colorF) 
						* vec3(hsbRG, hsbB) * (classicPerlin(uvMain * cos(TIME*1.)) + .5);
	
	d = triangle(uvFraction - vec2(0.5, 0.5), 0.09, (1. - uvMain), fraction);
	colorF = smoothedge(d, 1.0 / iResolution.x);
	hsbRG = hsb2rgb(vec3(ceil((uvMain.x) * fraction)) / fraction).r;
	hsbB = hsb2rgb(vec3(ceil((uvMain.y) * fraction)) / fraction).gb;
	color += vec3(1.0 - colorF)
						* vec3(hsbRG, hsbB) * (simplex(uvMain * sin(TIME*1.)) + .5);
						
	COLOR = vec4(color * 1.5, 1.0);
	
}