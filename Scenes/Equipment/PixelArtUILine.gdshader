shader_type canvas_item;

// Line setup
uniform vec2 point_a = vec2(0.1, 0.1);
uniform vec2 point_b = vec2(0.9, 0.9);
uniform float thickness = 1.0;
uniform float pixel_size = 32.0;

// Gradient settings
uniform bool use_gradient = false;
uniform vec4 color : source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform vec4 gradient   : source_color = vec4(1.0, 0.0, 0.0, 1.0);
uniform float gradient_steps = 8.0; // how many color steps to quantize

// Texture options
uniform bool use_texture = false;
uniform sampler2D line_texture;
uniform float texture_scale = 1.0;
uniform float texture_offset = 0.0;

// Animation
uniform float time_factor = 0.0;

void fragment() {
    // Grid snapping
    vec2 grid_uv = floor(UV * pixel_size) / pixel_size;
    vec2 A = floor(point_a * pixel_size) / pixel_size;
    vec2 B = floor(point_b * pixel_size) / pixel_size;

    // Line math
    vec2 AB = B - A;
    vec2 AP = grid_uv - A;
    float len = length(AB);
    float t = clamp(dot(AP, AB) / dot(AB, AB), 0.0, 1.0);
    vec2 closest = A + AB * t;

    float dist = length((grid_uv - closest) * pixel_size);
    if (dist >= thickness) discard;

    // if use_gradient is true apply a Gradient to the pixels
    vec4 col;
    if (use_gradient) {
        // Quantize gradient position to discrete steps
        float quant_t = floor(t * gradient_steps) / (gradient_steps - 1.0);
        col = mix(color, gradient, quant_t);
    } else {
        col = color;
    }

    // same as gradient but it takes its color from a texture (works with animations)
    if (use_texture) {
        float tex_x = fract((t * texture_scale + texture_offset + time_factor));
        vec2 tex_uv = vec2(tex_x, 0.5);
        vec4 tex_col = texture(line_texture, tex_uv);
        col *= tex_col;
    }

    COLOR = col;
}
