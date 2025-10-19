shader_type canvas_item;

// Line setup
uniform vec2 point_a = vec2(0.1, 0.1);
uniform vec2 point_b = vec2(0.9, 0.9);
uniform float thickness = 1.0;
uniform float pixel_size = 32.0;
uniform float node_scale = 1.0;

// Gradient settings
uniform bool use_gradient = false;
uniform vec4 color : source_color = vec4(1.0);
uniform vec4 gradient : source_color = vec4(1.0, 0.0, 0.0, 1.0);
uniform float gradient_steps = 8.0;

// Texture options
uniform bool use_texture = false;
uniform sampler2D line_texture;
uniform float texture_scale = 1.0;
uniform float texture_offset = 0.0;

// Animation
uniform float time_factor = 0.0;

void fragment() {
    // ❗ Scale compensation
    // When the node is scaled up, each UV covers more screen pixels.
    // Divide pixel_size by the node scale to preserve pixel density.
    float scaled_pixel_size = pixel_size / (1.0/node_scale);

    vec2 grid_uv = floor(UV * scaled_pixel_size) / scaled_pixel_size;
    vec2 A = floor(point_a * scaled_pixel_size) / scaled_pixel_size;
    vec2 B = floor(point_b * scaled_pixel_size) / scaled_pixel_size;

    // Line math
    vec2 AB = B - A;
    vec2 AP = grid_uv - A;
    float len = length(AB);
    float t = clamp(dot(AP, AB) / dot(AB, AB), 0.0, 1.0);
    vec2 closest = A + AB * t;

    // Divide by node_scale again to keep constant thickness in pixels
    float dist = length((grid_uv - closest) * pixel_size / (1.0/node_scale));
    if (dist >= thickness) discard;

    // Color / gradient
    vec4 col = color;
    if (use_gradient) {
        float quant_t = floor(t * gradient_steps) / (gradient_steps - 1.0);
        col = mix(color, gradient, quant_t);
    }

    // Texture option
    if (use_texture) {
        float tex_x = fract((t * texture_scale + texture_offset + time_factor));
        vec4 tex_col = texture(line_texture, vec2(tex_x, 0.5));
        col *= tex_col;
    }

    COLOR = col;
}
