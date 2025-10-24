shader_type canvas_item;
render_mode unshaded;

// --- Shape & pixelation ---
uniform float radius = 0.38;                 // Fixed circle radius in UV (0..0.5 ≈ half side)
uniform vec2  pixel_grid = vec2(24.0, 24.0); // pixelation for the whole sprite (edge looks chunky)
uniform vec2  cell_grid  = vec2(14.0, 14.0); // coarse cells for interior "flame pixels"
uniform float aspect = 1.0;                  // width/height of the node (keep circle round on rectangles)

// --- Animation / look ---
uniform float speed   = 1.0;   // flame rise speed
uniform float freq    = 5.0;   // interior detail frequency
uniform float vigor   = 0.9;   // how strong the interior animation reads
uniform int   levels  = 4;     // number of band steps (palette steps)
uniform float core_bias = 0.25;// pushes center hotter (0..1)

// Choose “up” for the licks (0,1) = up, (1,0) = right, etc.
uniform vec2 up_dir = vec2(0.0, 1.0);

// --- Palette (hot center -> cooler edge) ---
uniform vec4 col_core : source_color = vec4(1.00, 0.98, 0.82, 1.0);
uniform vec4 col_hot  : source_color = vec4(1.00, 0.60, 0.12, 1.0);
uniform vec4 col_warm : source_color = vec4(0.93, 0.31, 0.08, 1.0);
uniform vec4 col_cool : source_color = vec4(0.20, 0.08, 0.10, 1.0);

uniform float alpha_boost = 1.0;
uniform bool  USE_ADDITIVE = false;

// --- Cheap noise ---
float hash(vec2 p){
    p = vec2(dot(p, vec2(127.1, 311.7)),
             dot(p, vec2(269.5, 183.3)));
    return fract(sin(p.x + p.y) * 43758.5453);
}
float noise(vec2 p){
    vec2 i = floor(p);
    vec2 f = fract(p);
    float a = hash(i);
    float b = hash(i + vec2(1.0, 0.0));
    float c = hash(i + vec2(0.0, 1.0));
    float d = hash(i + vec2(1.0, 1.0));
    vec2 u = f*f*(3.0 - 2.0*f);
    return mix(mix(a,b,u.x), mix(c,d,u.x), u.y);
}
float fbm(vec2 p){
    float v = 0.0, a = 0.5, f = 1.0;
    for (int i = 0; i < 4; i++){
        v += noise(p * f) * a;
        f *= 2.0; a *= 0.5;
    }
    return v;
}

// 4-color palette blend
vec4 ramp4(float t){
    t = clamp(t, 0.0, 1.0);
    vec4 c01 = mix(col_core, col_hot,  smoothstep(0.0, 0.45, t));
    vec4 c12 = mix(col_hot,  col_warm, smoothstep(0.25, 0.75, t));
    vec4 c23 = mix(col_warm, col_cool, smoothstep(0.55, 1.00, t));
    return mix(mix(c01, c12, smoothstep(0.2, 0.6, t)), c23, smoothstep(0.6, 1.0, t));
}

// Hard step quantization for pixel-art bands
float quantize(float v, int n){
    if (n <= 1) return step(0.5, v);
    float k = float(n - 1);
    return floor(v * k + 0.0001) / k;
}

void fragment(){
    // Global chunky pixel look
    vec2 gUV = floor(UV * pixel_grid) / pixel_grid;

    // Local coords centered, keep circle round
    vec2 p = gUV - 0.5;
    p.x *= aspect;

    // Circle mask with FIXED edge (no wobble)
    float r = length(p);
    float circle = step(r, radius);

    // Interior "flame pixels": coarse grid sampled in a flowing field
    vec2 cUV = floor(UV * cell_grid) / cell_grid;  // coarse cell centers
    vec2 up = normalize(up_dir);
    float t = TIME * speed;

    // Flowing noise that moves along 'up'
    float flow = fbm(vec2(cUV.x * freq, (cUV.y + dot(up, vec2(0.0,1.0)) * t) * freq));

    // Second moving layer for sparkle
    float flow2 = fbm(vec2((cUV.x + t * 0.15) * (freq*0.7),
                           (cUV.y - t * 0.25) * (freq*0.9)));

    float flame = mix(flow, flow2, 0.5);

    // Bias center hotter; radial falloff from center to edge
    float center_grad = 1.0 - smoothstep(0.0, radius, r);
    flame = clamp(flame * (0.65 + vigor * 0.35) + center_grad * core_bias, 0.0, 1.0);

    // Quantize into banded “flame tiles”
    float band = quantize(flame, max(levels, 2));

    // Slight edge feather so the circle stays stable but not harsh
    float soft = 0.002 + 0.02 / max(max(pixel_grid.x, pixel_grid.y), 1.0);
    float edge = 1.0 - smoothstep(radius - soft, radius + soft, r);

    // Combine: outside circle => alpha 0
    float coverage = circle * max(edge, 0.98);

    // Color by palette from hot center to cool edge using a radial + band mix
    float grad = clamp(1.0 - center_grad * 0.9, 0.0, 1.0);
    vec4 col = ramp4(mix(grad, 1.0 - band, 0.55));

    float a = coverage * alpha_boost;

    if (USE_ADDITIVE) {
        COLOR = vec4(col.rgb * a, a);
    } else {
        COLOR = vec4(col.rgb, a);
    }
}
